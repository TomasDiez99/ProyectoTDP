using UnityEngine;

namespace Geometry
{
    public struct Quad
    {
        public Triangle t1;
        public Triangle t2;
        
        public static readonly Quad OneByOne = new Quad
        {
            //With center on (0,0) each triangle counterclockwise
            t1 = new Triangle
            {
                a = new Vector3(0.5f, 0.5f, 0.5f),
                b = new Vector3(-0.5f, -0.5f, 0.5f),
                c = new Vector3(0.5f, -0.5f, 0.5f)
            },
            t2 = new Triangle
            {
                a = new Vector3(0.5f, 0.5f, 0.5f),
                b = new Vector3(-0.5f, 0.5f, 0.5f),
                c = new Vector3(-0.5f, -0.5f, 0.5f)
            }
        };
        
        public static Quad operator *(Quaternion quaternion, Quad quad)
        {
            return new Quad
            {
                t1 = quaternion * quad.t1,
                t2 = quaternion * quad.t2,
            };
        }

        public static Quad operator +(Quad quad, Vector3 translation)
        {
            return new Quad
            {
                t1 = quad.t1 + translation,
                t2 = quad.t2 + translation,
            };

        }
        
    }
}