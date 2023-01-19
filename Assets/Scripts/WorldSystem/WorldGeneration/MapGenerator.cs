using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public DrawMode drawMode;

    //public const int mapChunkSize = 241;

    public const int mapChunkSize = 241;//241^2 is the max size of a mesh, can have "LODs" of i=2,4,6,8,10,12

    [Range(0, 6)]
    public int editorPreviewLOD;//incement of LevelDetail
    public float noiseScale;

    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainTypes[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>(); //find Object in Scene
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));//hand over the noiseMap
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));//hand over the ColorMap and MapSize
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD),
                TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));//hand over meshData and ColorMap and Size
        }
    }

    private MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color; //saves the current color in the 1D Array for further usage
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }
    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (noiseScale < 0.00003f)
        {
            noiseScale = 0.00004f;
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThreaded(center, callback);
        };
        new Thread(threadStart).Start();

    }
    private void MapDataThreaded(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue) //lock, to prevent multiple threads to access the same data
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThreaded(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThreaded(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    #region Structs
    struct MapThreadInfo<T> //generic struct to handle both kinds of mapData(colorMap and heightMap)
    {
        public readonly Action<T> callback; //readonly because structs should be immutable
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }

    public struct MapData //stores the Data for the Map (heightMap and colorMap) for usage in other methodes
    {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
        }
    }
    [System.Serializable]
    public struct TerrainTypes //not readonly, if readonly var don't show up in inspector
    {
        public string name;
        public float height;
        public Color color;
    }
    #endregion
}
