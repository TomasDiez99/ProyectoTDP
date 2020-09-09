using UnityEngine;

namespace Geometry
{
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public static Triangle operator *(Quaternion quaternion, Triangle triangle)
        {
            return new Triangle
            {
                a = quaternion * triangle.a,
                b = quaternion * triangle.b,
                c = quaternion * triangle.c
            };
        }

        public static Triangle operator +(Triangle triangle, Vector3 translation)
        {
            return new Triangle
            {
                a = triangle.a + translation,
                b = triangle.b + translation,
                c = triangle.c + translation
            };

        }
    }
}