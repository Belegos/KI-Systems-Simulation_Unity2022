using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;
using UnityEditor.ShaderGraph.Internal;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;
    public const int MapChunkSize = 239;//239^2 is the max size of a mesh, can have "LODs" of i=2,4,6,8,10,12

    [Range(0, 6)]
    public int editorPreviewLOD;//increment of Level Of Detail
    public float noiseScale;

    public int octaves;
    [Range(0, 1)] public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool useFalloffMap;
    public bool autoUpdate;
    public bool enableThreading;
    public Shader shader;

    public TerrainTypes[] regions;
    public float[,] FalloffMap;

    [SerializeField] private Texture2D _texture;
    private EndlessTerrain _endlessTerrain;

    Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        _endlessTerrain = FindObjectOfType<EndlessTerrain>();
        _texture = Resources.Load("Visuals/Material/WorldGeneration/Material/NoiseMap") as Texture2D;
        GenerateNoiseTexture2D(_texture);
        FalloffMap = FalloffGenerator.GenerateFalloffMap(MapChunkSize);
    }

    void Update()
    {
        GenerateNoiseTexture2D(_texture);

        if (enableThreading)
        {

            if (_mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
            if (_meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>(); //find Object in Scene
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.HeightMap));//hand over the noiseMap
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));//hand over the ColorMap and MapSize
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD),
                TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));//hand over meshData and ColorMap and Size
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(MapChunkSize)));
        }
    }

    private MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, seed, noiseScale, octaves, persistence, lacunarity, center + offset, normalizeMode);
        //mapChunkSize + 2 because of the border to compensate

        Color[] colorMap = new Color[MapChunkSize * MapChunkSize];

        for (int y = 0; y < MapChunkSize; y++)
        {
            for (int x = 0; x < MapChunkSize; x++)
            {
                if (useFalloffMap)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - FalloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colorMap[y * MapChunkSize + x] = regions[i].color; //saves the current color in the 1D Array for further usage
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    private void GenerateNoiseTexture2D(Texture2D _texture)
    {
        Vector2 center = Vector2.zero;
        //gets the values of the noiseMap and sets the pixels of the texture to the values of the noiseMap for usage in shaders
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, seed, noiseScale, octaves, persistence, lacunarity, center + offset, normalizeMode);
        _texture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                _texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, noiseMap[x, y]));
            }
        }
        _texture.Apply();
        _endlessTerrain.MapShaderMaterial = new Material(shader);
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
        FalloffMap = FalloffGenerator.GenerateFalloffMap(MapChunkSize);
    }
    #region Multithreading
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
        lock (_mapDataThreadInfoQueue) //lock, to prevent multiple threads to access the same data
        {
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
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

    private void MeshDataThreaded(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }
    #endregion

    #region Structs
    struct MapThreadInfo<T> //generic struct to handle both kinds of mapData(colorMap and heightMap)
    {
        public readonly Action<T> Callback; //readonly because structs should be immutable
        public readonly T Parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.Callback = callback;
            this.Parameter = parameter;
        }

    }

    public struct MapData //stores the Data for the Map (heightMap and colorMap) for usage in other methods
    {
        public readonly float[,] HeightMap;
        public readonly Color[] ColorMap;
        //public readonly Texture2D Texture;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.HeightMap = heightMap;
            this.ColorMap = colorMap;
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
