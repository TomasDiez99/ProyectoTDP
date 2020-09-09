using System.Collections.Generic;
using Terrain.Structures;
using UnityEngine;

namespace Terrain
{
    public interface ChunkPool
    {
        bool EnsureActive(ChunkIndex index);
        void EnsureHidden(ChunkIndex index);
        void EnsureDeleted(ChunkIndex index);

        IEnumerable<Chunk> GetChunks();
        int PoolCount { get; }
    }
}