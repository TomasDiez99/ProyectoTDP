using System.Collections.Generic;
using Geometry;
using Terrain.Structures;
using Tools;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace Terrain.Generation
{
    
    [CreateAssetMenu(fileName = "Perlin Chunk Factory", menuName = "Tomi/Chunk Factory Perlin")]
    public class PerlinChunkBuilderAsset : ChunkBuilderAsset
    {
        
        const int VerticesPerFace = 6;
        const int MaxFacesCount = 5;
        const int VerticesPerCube = MaxFacesCount * VerticesPerFace;

        static readonly Vector2Int UVOffsetBottomSide = new Vector2Int(1, 0);
        static readonly Vector2Int UVOffsetMiddleSide = new Vector2Int(0, 1);
        static readonly Vector2Int UVOffsetTopFace    = new Vector2Int(0, 0);
        static readonly Vector2Int UVOffsetTopSide    = new Vector2Int(1, 1);
        
        readonly List<Vector2> uvs = new List<Vector2>();
        readonly List<Vector3> vertices = new List<Vector3>();
        readonly List<int> geometryTrianglesIndices = new List<int>();
        readonly List<Triangle> geometryTriangles = new List<Triangle>();
        
        ChunkIndex currentChunkIndex;
        Mesh mesh;
        Chunk currentChunk;
        GameObject chunkGameObject;
        Random random;
        Vector2Int chunkTextureOffset;
        
        
        public override void SetChunkIndex(ChunkIndex index)
        {
            currentChunkIndex = index;
        }

        public override Chunk GetChunk()
        {
            return currentChunk;
            
        }

        //We use same material because
        //it is all a single mesh
        public override void CreateMaterial()
        {
            const int maxX = 40_000;
            //To have same texture for whole chunk deterministically
            random = new Random(currentChunkIndex.x * maxX + currentChunkIndex.z);
            chunkTextureOffset = TextureFromSeed(random.Next(), random.Next());
        }

        public override void PrepareGeometryGeneration()
        {
            var trianglesListCapacity = Mathf.FloorToInt(ChunkSize * ChunkSize * VerticesPerCube);
            geometryTriangles.ResetWithCapacity(trianglesListCapacity);
            uvs.ResetWithCapacity(trianglesListCapacity * 3);
            geometryTrianglesIndices.ResetWithCapacity(trianglesListCapacity * 3);
            vertices.ResetWithCapacity(trianglesListCapacity * 3);
        }

        public override void CreateChunkComponent()
        {
            currentChunk = chunkGameObject.AddComponent<TerrainChunk>();
            currentChunk.Index = currentChunkIndex;
        }

        public override void CreateCollider()
        {
            chunkGameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        public override void InitializeModelMatrix()
        {
            var chunkSize = ChunkSize * cubeScale;
            chunkGameObject.transform.position = SpaceMathService.GetChunkPosition(currentChunkIndex.x, currentChunkIndex.z, chunkSize);
            chunkGameObject.transform.localScale = Vector3.one * cubeScale;
        }

        void InitMeshFilter()
        {
            mesh = new Mesh();
            chunkGameObject.AddComponent<MeshFilter>().mesh = mesh;
        }

        public override void CreateRenderingComponents()
        {
            InitMeshRenderer();
            InitMeshFilter();
        }

        void InitMeshRenderer()
        {
            var meshRenderer = chunkGameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            meshRenderer.receiveShadows = true;
        }

        public override void CreateGameObject()
        {
            chunkGameObject = new GameObject($"Chunk at {currentChunkIndex.x} , {currentChunkIndex.z}");
            chunkGameObject.SetActive(false);
            chunkGameObject.layer = ChunksData.ChunkLayer;
        }

        MeshGeneration meshGeneration = new MeshGeneration();
        const int TextureRows = 4;
        const int TextureColumns = 4;
        const int SingleTextureSquareSize = 2;

        void AddUV(Vector2Int offset, TileData tileData)
        {
            
            offset += tileData.textureOffset;
            var quad = meshGeneration.SelectTextureQuad(TextureRows, TextureColumns, offset.x, offset.y);
            uvs.Add(quad.t1.a);
            uvs.Add(quad.t1.b);
            uvs.Add(quad.t1.c);
            uvs.Add(quad.t2.a);
            uvs.Add(quad.t2.b);
            uvs.Add(quad.t2.c);
        }

        void AddQuad(Vector3 translation, Quaternion rotation)
        {
            var quad = rotation * Quad.OneByOne + translation;
            geometryTriangles.Add(quad.t1);
            geometryTriangles.Add(quad.t2);
        }

        Vector3 GridLocationToTranslation(TilePosition tile)
        {
            return (Vector3Int) tile;
        }

        Vector2Int TextureFromSeed(int seed1, int seed2)
        {
            var row = seed1 % TextureRows;
            var col = seed2 % TextureColumns;
            return new Vector2Int(row,col) * SingleTextureSquareSize;
        }
        
        void DrawCube(TilePosition tile)
        {
            tile.Y = GetTileHeight(tile);
            var translation = GridLocationToTranslation(tile);
            
            //Add to geometry the top face
            AddQuad(translation, SpaceMathService.LooksUp);

            var tileData = new TileData{textureOffset = chunkTextureOffset };
            
            AddUV(UVOffsetTopFace, tileData);
            foreach (var pair in SpaceMathService.DirectionToRotation)
            {
                var gridDirection = pair.Key;
                var rot = pair.Value;
                DrawDirectionFace(tile, gridDirection, rot, tileData);
            }
        }

        void DrawDirectionFace(TilePosition currentTile, TilePosition gridDirection, Quaternion rot, TileData tileData)
        {
            var gridNeighbourY = GetTileHeight(currentTile + gridDirection);
            var facesToDraw = currentTile.Y - gridNeighbourY;

            var step = TilePosition.Zero;
            
            for (step.Y = 0 ; step.Y < facesToDraw ; step.Y++) 
            {
                AddQuad(GridLocationToTranslation(currentTile - step), rot);
                AddUV(SelectTextureOffset(step.Y, 0, facesToDraw - 1), tileData);
            }
        }

        Vector2Int SelectTextureOffset(int yStep, int firstStep, int lastStep)
        {
            if (yStep == firstStep)
            {
                return UVOffsetTopSide;
            }
            if (yStep == lastStep)
            {
                return UVOffsetBottomSide;
            }
            return UVOffsetMiddleSide;
        }


        public override void GenerateGeometry()
        {
            for (var x = 0; x < ChunkSize; x++)
            for (var z = 0; z < ChunkSize; z++)
                DrawCube(new TilePosition(x,z));
            LoadMesh();
        }

        void LoadMesh()
        {
            FillVerticesAndTrianglesLists();
            InitializeMesh();
        }
        
        
        void InitializeMesh()
        {
            LoadMeshAttributes();
            RecalculateMeshData();
        }

        void RecalculateMeshData()
        {
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }
        void LoadMeshAttributes()
        {
            mesh.vertices = vertices.ToArray();
            mesh.triangles = geometryTrianglesIndices.ToArray();
            mesh.uv = uvs.ToArray();
        }

        void FillVerticesAndTrianglesLists()
        {
            var indexCount = 0;

            foreach (var triangle in geometryTriangles)
            {
                vertices.Add(triangle.a);
                vertices.Add(triangle.b);
                vertices.Add(triangle.c);
                geometryTrianglesIndices.Add(indexCount++);
                geometryTrianglesIndices.Add(indexCount++);
                geometryTrianglesIndices.Add(indexCount++);
            }
        }


        int GetTileHeight(TilePosition tile)
        {
            if (IsOutsideChunk(tile)) return -1; //To handle border cases
            var sample = SamplePerlin(tile);
            return Mathf.CeilToInt(maxHeight * sample);
        }

        bool IsOutsideChunk(TilePosition tile)
        {
            return tile.X < 0 || tile.Z < 0 || tile.X >= ChunkSize || tile.Z >= ChunkSize;
        }

        float SamplePerlin(TilePosition tile)
        {
            var u = tile.X / ChunkSize + currentChunkIndex.x;
            var v = tile.Z / ChunkSize + currentChunkIndex.z;
            return Mathf.PerlinNoise(u, v);
        }
    }
}