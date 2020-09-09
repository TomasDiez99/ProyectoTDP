using System.Collections.Generic;
using Terrain.Structures;
using UnityEngine;

namespace Geometry
{
    public static class SpaceMathService
    {
        public static readonly Quaternion LooksDown = Quaternion.LookRotation(Vector3.down);
        public static readonly Quaternion LooksUp = Quaternion.LookRotation(Vector3.up);
        
        static readonly Quaternion LooksLeft = Quaternion.LookRotation(Vector3.left);
        static readonly Quaternion LooksRight = Quaternion.LookRotation(Vector3.right);
        static readonly Quaternion LooksBack = Quaternion.LookRotation(Vector3.back);
        static readonly Quaternion LooksForward = Quaternion.LookRotation(Vector3.forward);

        static SpaceMathService()
        {
            DirectionToRotation[new TilePosition(-1, 0)] = LooksLeft;
            DirectionToRotation[new TilePosition(1, 0)] = LooksRight;
            DirectionToRotation[new TilePosition(0, 1)] = LooksForward;
            DirectionToRotation[new TilePosition(0, -1)] = LooksBack;
        }

        public static Dictionary<TilePosition, Quaternion> DirectionToRotation { get; } =
            new Dictionary<TilePosition, Quaternion>();
        
        public static Vector3 GetChunkPosition(int x, int y, float chunkSize)
        {
            return new Vector3(x, 0, y) * chunkSize;
        }
        
    }
}