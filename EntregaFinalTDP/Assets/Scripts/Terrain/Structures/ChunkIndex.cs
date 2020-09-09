using UnityEngine;

namespace Terrain.Structures
{
    public struct ChunkIndex
    {
        public int x;
        public int z;
        
        public ChunkIndex(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static ChunkIndex operator +(ChunkIndex leftChunk, ChunkIndex rightChunk)
        {
            return new ChunkIndex(leftChunk.x + rightChunk.x, leftChunk.z + rightChunk.z);
        }
        
        public static ChunkIndex operator -(ChunkIndex leftChunk, ChunkIndex rightChunk)
        {
            return new ChunkIndex(leftChunk.x - rightChunk.x, leftChunk.z - rightChunk.z);
        }

        public static implicit operator Vector2Int(ChunkIndex index)
        {
            return new Vector2Int(index.x, index.z);
        }
        
        public static implicit operator ChunkIndex(Vector2Int vector)
        {
            return new ChunkIndex(vector.x,vector.y);
        }

        bool Equals(ChunkIndex other)
        {
            return x == other.x && z == other.z ;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkIndex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ((x * 397) ^ z) * 397;
        }
    }
}