using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurveThreaded = new AnimationCurve(heightCurve.keys);//prevent AnimationCurve from returning wrong values, each Thread got his own heightCurve
        int borderedSize = heightMap.GetLength(0);
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int meshSize = borderedSize - 2 * meshSimplificationIncrement;
        int meshSizeUnsimplefied = borderedSize - 2;
        //meshSizeUnsimplefied so the LODVersions will not change in Size when used for topLeftX&Y
        float topLeftX = (meshSizeUnsimplefied - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplefied - 1) / 2f;

        //handles increament of the mesh simplification(LOD), if editorPreviewLOD is 0, then it is 1, else it is editorPreviewLOD * 2
        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);
        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;
                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                int vertexIndex = vertexIndicesMap[x, y];
                Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
                float height = heightCurveThreaded.Evaluate(heightMap[x, y]) * heightMultiplier;

                #region lock the heightCurve to stop threads form multithreading
                //lock (heightCurve){
                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplefied, height, topLeftZ - percent.y * meshSizeUnsimplefied); //x = x-coordinate, y = coordinate of the heightMap, z = y-coordinate
                #endregion

                meshData.AddVertex(vertexPosition, percent, vertexIndex);

                if (x < borderedSize - 1 && y < borderedSize - 1)//ignoring right and bottom verticies of the map
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
                    meshData.AddTriangle(a, d, c);//creates first triangle in the given order clockwise
                    meshData.AddTriangle(d, a, b);//creates the second triangle in the given order clockwise
                }

                vertexIndex++; //Index to keep track, where we are in the 1D-Array
            }
        }
        return meshData; //important for later threading
    }
}
public class MeshData
{
    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector2[] _uvs;

    private Vector3[] _borderVerticies;
    private int[] _borderTriangles;

    private int _triangleIndex;
    private int _borderTriangleIndex;

    public MeshData(int verticesPerLine)
    {
        _vertices = new Vector3[verticesPerLine * verticesPerLine];
        _uvs = new Vector2[verticesPerLine * verticesPerLine];
        _triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        _borderVerticies = new Vector3[verticesPerLine * 4 + 4];//multiplyed by 4 because we have 4 borders, 4 for each corner
        _borderTriangles = new int[24 * verticesPerLine];//24 for all triangles
    }
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            _borderVerticies[-vertexIndex - 1] = vertexPosition;
        }
        else //if the verticies or uvs greater than 0, they are no borderPositions wich are not part of actuall mesh
        {
            _vertices[vertexIndex] = vertexPosition;
            _uvs[vertexIndex] = uv;
        }
    }
    /// <summary>
    /// Adds an triangle to the mesh
    /// if a,b or c are smaller than 0
    /// they belong to the borders, wich are not part
    /// of the actuall mesh but are nessescary for the endless terrain seamlessly blending
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void AddTriangle(int a, int b, int c) // a,b,c are the verticies
    {
        if (a < 0 || b < 0 || c < 0)
        {
            _borderTriangles[_borderTriangleIndex] = a;
            _borderTriangles[_borderTriangleIndex + 1] = b;
            _borderTriangles[_borderTriangleIndex + 2] = c;
            _borderTriangleIndex += 3;
        }
        else
        {
            _triangles[_triangleIndex] = a;
            _triangles[_triangleIndex + 1] = b;
            _triangles[_triangleIndex + 2] = c;
            _triangleIndex += 3;
        }
    }

    /// <summary>
    /// Own immplementation of RecalculateNormals() to display the normals of neighbouring 
    /// triangles correctly of the chunks.
    /// </summary>
    /// <returns></returns>
    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[_vertices.Length];
        int triangleCount = _triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3; //index of the triangle in the array
            int vertexIndexA = _triangles[normalTriangleIndex];
            int vertexIndexB = _triangles[normalTriangleIndex + 1];
            int vertexIndexC = _triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }
        int borderTriangleCount = _borderTriangles.Length / 3;
        for (int j = 0; j < borderTriangleCount; j++)
        {
            int normalTriangleIndex = j * 3; //index of the triangle in the array
            int vertexIndexA = _borderTriangles[normalTriangleIndex];
            int vertexIndexB = _borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = _borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }
    /// <summary>
    /// This function calculates the surface normal of a triangle formed by the three points, 
    /// represented by the indices passed as arguments.
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    /// <param name="indexC"></param>
    /// <returns></returns>
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? _borderVerticies[-indexA - 1] : _vertices[indexA];
        Vector3 pointB = (indexB < 0) ? _borderVerticies[-indexB - 1] : _vertices[indexB];
        Vector3 pointC = (indexC < 0) ? _borderVerticies[-indexC - 1] : _vertices[indexC];

        Vector3 sideAb = pointB - pointA;
        Vector3 sideAc = pointC - pointA;
        return Vector3.Cross(sideAb, sideAc).normalized;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices; //equals to vertices array
        mesh.triangles = _triangles; //equals to triangles array
        mesh.uv = _uvs; //equals to uvs array
        mesh.normals = CalculateNormals();
        return mesh;
    }
}
