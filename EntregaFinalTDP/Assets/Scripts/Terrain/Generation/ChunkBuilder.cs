using Terrain.Structures;

namespace Terrain
{
    public interface ChunkBuilder
    {
        void SetChunkIndex(ChunkIndex index);
        void CreateMaterial();
        void CreateGameObject();
        void CreateRenderingComponents();
        void InitializeModelMatrix();
        void CreateCollider();
        void CreateChunkComponent();
        void PrepareGeometryGeneration();
        void GenerateGeometry();
        Chunk GetChunk();
    }
}