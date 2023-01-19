using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class EndlessTerrain : MonoBehaviour
{
    public LODInfo[] detailLevels;
    const float viewerMoveForUpdate = 25f;
    const float sqrviewerMoveForUpdate = viewerMoveForUpdate * viewerMoveForUpdate;
    public static float maxViewDst;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    private Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    private int chunkSize;
    private int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();//stores all visited MapChunks
    List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        UpdateVisableChunks();

    }
    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveForUpdate) //threshold to stop chunks updating every frame
        {
            viewerPositionOld = viewerPosition;
            UpdateVisableChunks();
        }
    }

    private void UpdateVisableChunks()
    {
        for (int i = 0; i < terrainChunkVisibleLastUpdate.Count; i++)
        {
            terrainChunkVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunkVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())//adds visibile terrainChunks to the List
                    {
                        terrainChunkVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }

        }
    }
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);//
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }
        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {

                float viewerDstFromNearestEdges = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdges <= maxViewDst;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdges > detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                }
                SetVisible(visible);
            }
        }
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;//read only returns the active state as a bool
        }

        class LODMesh //stores the mesh and the LOD
        {
            public Mesh mesh;
            public bool hasRequestedMesh;
            public bool hasMesh;
            int lod;
            System.Action updateCallback;

            public LODMesh(int lod, System.Action updateCallback)
            {
                this.lod = lod;
                this.updateCallback = updateCallback;
            }

            void OnMeshDataReceived(MeshData meshData)
            {
                mesh = meshData.CreateMesh();
                hasMesh = true;

                updateCallback();
            }

            public void RequestMesh(MapData mapData)
            {
                hasRequestedMesh = true;
                mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
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
