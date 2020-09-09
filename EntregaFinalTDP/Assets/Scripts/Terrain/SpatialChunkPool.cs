using System.Collections.Generic;
using Terrain.Generation;
using Terrain.Structures;
using UnityEngine;

namespace Terrain
{
    public class SpatialChunkPool : ChunkPool
    {
        readonly TerrainGenerationDirector director;
        public SpatialChunkPool(ChunkBuilder chunkBuilder)
        {
            director = new TerrainGenerationDirector(chunkBuilder);
        }

        Dictionary<ChunkIndex, Chunk> Chunks { get; } = new Dictionary<ChunkIndex, Chunk>();

        public IEnumerable<Chunk> GetChunks()
        {
            return Chunks.Values;
        }

        public int PoolCount => Chunks.Count;

        public bool EnsureActive(ChunkIndex index)
        {
            bool res = false;
            if (!Chunks.TryGetValue(index, out var chunk))
            {
                chunk = Create(index);
                res= true;
            }
            chunk.SetVisible(true);
            return res;
        }

        public void EnsureHidden(ChunkIndex index)
        {
            if (!Chunks.TryGetValue(index, out var chunk))
            {
                chunk = Create(index);
            }

            chunk.SetVisible(false);
        }

        public void EnsureDeleted(ChunkIndex index)
        {
            if (Chunks.TryGetValue(index, out var chunk))
            {
                chunk.Dispose();
                Chunks.Remove(index);
            }
        }

        Chunk Create(ChunkIndex index)
        {
            var chunk = director.Construct(index);
            Chunks[index] = chunk;
            chunk.SetVisible(true);
            return chunk;
        }
    }
}