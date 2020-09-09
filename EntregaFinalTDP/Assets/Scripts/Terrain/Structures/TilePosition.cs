using System;
using UnityEngine;

namespace Terrain.Structures
{
    /// <summary>
    /// el chiquito
    /// </summary>
    public struct TilePosition : IEquatable<TilePosition>
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
        
        public static readonly TilePosition Zero  = new TilePosition(0,0,0);

        public TilePosition(int gridX, int height, int gridZ)
        {
            X = gridX;
            Y = height;
            Z = gridZ;
        }

        public TilePosition(int gridX, int gridZ)
        {
            X = gridX;
            Y = 0;
            Z = gridZ;
        }

        public static TilePosition operator +(TilePosition leftTile, TilePosition rightTile)
        {
            return (Vector3Int) leftTile + (Vector3Int) rightTile;
        }
        
        public static TilePosition operator -(TilePosition leftTile, TilePosition rightTile)
        {
            return (Vector3Int) leftTile - (Vector3Int) rightTile;
        }

        public static implicit operator Vector3Int(TilePosition position)
        {
            return new Vector3Int(position.X,position.Y,position.Z);
        }
        
        public static implicit operator TilePosition(Vector3Int vector)
        {
            return new TilePosition(vector.x,vector.y,vector.z);
        }
        
        public bool Equals(TilePosition other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            //Good way to hash
            return (((X * 397) ^ Y) * 397) ^ Z;
        }
    }
}