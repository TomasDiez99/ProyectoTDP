using Terrain.Structures;
using UnityEngine;

namespace Terrain
{
    public interface Chunk
    {
        ChunkIndex Index { get; set; }
        void Dispose();
        void SetVisible(bool visible);
    }
}