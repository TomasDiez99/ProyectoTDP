using Terrain.Structures;
using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain
{
    public abstract class ChunkBuilderAsset : ScriptableObject , ChunkBuilder
    {
        //Default value = 1
        public float cubeScale = 1;
        public Material material;
        public int maxHeight;
        [FormerlySerializedAs("size"), SerializeField] int gridSize;
        public float ChunkSize => gridSize;

        public abstract void SetChunkIndex(ChunkIndex index);
        public abstract void CreateMaterial();
        public abstract void CreateGameObject();
        public abstract void CreateRenderingComponents();
        public abstract void InitializeModelMatrix();
        public abstract void CreateCollider();
        public abstract void CreateChunkComponent();
        public abstract void PrepareGeometryGeneration();
        public abstract void GenerateGeometry();
        public abstract Chunk GetChunk();
    }
}

