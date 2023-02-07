using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class EndlessTerrain : MonoBehaviour
{
    public LODInfo[] detailLevels;
    private const float scale = 4f; //scales whole map
    private const float ViewerMoveForUpdate = 25f;
    private const float SqrviewerMoveForUpdate = ViewerMoveForUpdate * ViewerMoveForUpdate;
    public static float MaxViewDst;
    public Transform viewer;
    [SerializeField]private Material mapShaderMaterial;
    public Material mapColorMaterial;
    private Material _mapMaterial;

    public static Vector2 ViewerPosition;
    private Vector2 _viewerPositionOld;
    static MapGenerator _mapGenerator;
    private int _chunkSize;
    private int _chunksVisibleInViewDst;
    public bool enableShaderMaterial = false;
    private bool prevBool;

    public Material mapMaterial
    {
        get { return _mapMaterial; }
    }

    public Material MapShaderMaterial { get => mapShaderMaterial; set => mapShaderMaterial = value; }

    Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();//stores all visited MapChunks
    static List<TerrainChunk> _terrainChunkVisibleLastUpdate = new List<TerrainChunk>();
    private void OnEnable()
    {
        prevBool = enableShaderMaterial;
        if (!enableShaderMaterial) 
        {
            _mapMaterial = mapColorMaterial;
        }
        if (enableShaderMaterial) 
        {
            _mapMaterial = MapShaderMaterial;
        }
    }
    private void Start()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();

        MaxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        _chunkSize = MapGenerator.MapChunkSize - 1;
        _chunksVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / _chunkSize);
        UpdateVisableChunks();

    }
    private void Update()
    {
        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if (enableShaderMaterial != prevBool) 
        {
            prevBool = !prevBool;
            UpdateVisableChunks();
        }
        if ((_viewerPositionOld - ViewerPosition).sqrMagnitude > SqrviewerMoveForUpdate) //threshold to stop chunks updating every frame
        {
            _viewerPositionOld = ViewerPosition;
            UpdateVisableChunks();
        }
    }

    private void UpdateVisableChunks()
    {
        for (int i = 0; i < _terrainChunkVisibleLastUpdate.Count; i++)
        {
            _terrainChunkVisibleLastUpdate[i].SetVisible(false);
        }
        _terrainChunkVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

        for (int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();//updates all visible chunks
                }
                else
                {
                    _terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, detailLevels, transform, _mapMaterial));
                }
            }

        }
    }
    public class TerrainChunk
    {
        GameObject _meshObject;
        Vector2 _position;
        Bounds _bounds;

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;
        MeshCollider _meshCollider;

        LODInfo[] _detailLevels;
        LODMesh[] _lodMeshes;

        MapData _mapData;
        bool _mapDataReceived;
        int _previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this._detailLevels = detailLevels;
            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);//
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;

            _meshObject.transform.position = positionV3 * scale;
            _meshObject.transform.parent = parent;
            _meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            _mapGenerator.RequestMapData(_position, OnMapDataReceived);
        }
        void OnMapDataReceived(MapData mapData)
        {
            this._mapData = mapData;
            _mapDataReceived = true;
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
            _meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (_mapDataReceived)
            {

                float viewerDstFromNearestEdges = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
                bool visible = viewerDstFromNearestEdges <= MaxViewDst;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < _detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdges > _detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != _previousLODIndex)
                    {
                        LODMesh lodMesh = _lodMeshes[lodIndex];
                        if (lodMesh.HasMesh)
                        {
                            _previousLODIndex = lodIndex;
                            _meshFilter.mesh = lodMesh.Mesh;
                            _meshCollider.sharedMesh = lodMesh.Mesh;
                        }
                        else if (!lodMesh.HasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_mapData);
                        }
                    }
                    _terrainChunkVisibleLastUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }
        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;//read only returns the active state as a bool
        }

        class LODMesh //stores the mesh and the LOD
        {
            public Mesh Mesh;
            public bool HasRequestedMesh;
            public bool HasMesh;
            int _lod;
            System.Action _updateCallback;

            public LODMesh(int lod, System.Action updateCallback)
            {
                this._lod = lod;
                this._updateCallback = updateCallback;
            }

            void OnMeshDataReceived(MeshData meshData)
            {
                Mesh = meshData.CreateMesh();
                HasMesh = true;

                _updateCallback();
            }

            public void RequestMesh(MapData mapData)
            {
                HasRequestedMesh = true;
                _mapGenerator.RequestMeshData(mapData, _lod, OnMeshDataReceived);
            }
        }
    }
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;

        public LODInfo(int lod, float visibleDstThreshold)
        {
            this.lod = lod;
            this.visibleDstThreshold = visibleDstThreshold;
        }
    }
}
