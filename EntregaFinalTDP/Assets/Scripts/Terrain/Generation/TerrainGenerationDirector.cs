using Terrain.Structures;
using UnityEngine;

namespace Terrain.Generation
{
    public class TerrainGenerationDirector
    {
        ChunkBuilder currentBuilder;

        public TerrainGenerationDirector(ChunkBuilder chunkBuilder)
        {
            currentBuilder = chunkBuilder;
        }

        public Chunk Construct(ChunkIndex index)
        {
            currentBuilder.SetChunkIndex(index);
            currentBuilder.CreateMaterial();
            currentBuilder.CreateGameObject();
            currentBuilder.CreateRenderingComponents();
            currentBuilder.InitializeModelMatrix();
            currentBuilder.CreateCollider();
            currentBuilder.CreateChunkComponent();
            currentBuilder.PrepareGeometryGeneration();
            currentBuilder.GenerateGeometry();
            return currentBuilder.GetChunk();
        }

        public void SetBuilder(ChunkBuilder chunkBuilder)
        {
            currentBuilder = chunkBuilder;
        }
    }
}