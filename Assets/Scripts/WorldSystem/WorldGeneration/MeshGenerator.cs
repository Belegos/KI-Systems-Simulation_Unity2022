using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurveThreaded = new AnimationCurve(_heightCurve.keys);//prevent AnimationCurve from returning wrong values, each Thread got his own heightCurve
        int width = heightMap.GetLength(0); //first dimension of the array to get the width
        int height = heightMap.GetLength(0);//second dimension of the array to get the height
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        //handles increament of the mesh simplification(LOD), if editorPreviewLOD is 0, then it is 1, else it is editorPreviewLOD * 2
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
#region lock the heightCurve to stop threads form multithreading
                //lock (heightCurve){
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurveThreaded.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y); //x = x-coordinate, y = coordinate of the heightMap, z = y-coordinate
                //}
#endregion
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)//ignoring right and bottom verticies of the map
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    //i = vertexIndex 
                    //i + width + 1 = vertexIndex of the vertex to the right and down 
                    //i + width = vertexIndex of the vertex down
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++; //Index to keep track, where we are in the 1D-Array
            }
        }
        return meshData; //important for later threading
    }
}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c) // a,b,c are the verticies
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    /// <summary>
    /// Own immplementation of RecalculateNormals() to display the normals of neighbouring 
    /// triangles correctly of the chunks.
    /// </summary>
    /// <returns></returns>
    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3; //index of the triangle in the array
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices; //equals to vertices array
        mesh.triangles = triangles; //equals to triangles array
        mesh.uv = uvs; //equals to uvs array
        mesh.normals = CalculateNormals();
        return mesh;
    }
}
