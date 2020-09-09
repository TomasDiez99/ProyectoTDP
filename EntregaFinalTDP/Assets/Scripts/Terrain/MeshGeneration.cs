using Geometry;
using UnityEngine;

namespace Terrain
{
    public class MeshGeneration
    {
        static readonly Vector2 Center = new Vector2(0.5f,0.5f);

        Vector3 ScaleWith(Vector3 point , Vector2 scale)
        {
            point.x *= scale.x;
            point.y *= scale.y;
            return point;
        }
        
        public Quad SelectTextureQuad(int rows, int cols, int x, int y)
        {
            var multiplier = new Vector2(1f / cols,1f / rows);
            var offset = new Vector2(x/(float)cols, y/(float)rows);
            var quad = Quad.OneByOne;
            
            Vector3 normalizedOffset = Center * multiplier + offset;
            quad.t1.a = ScaleWith(quad.t1.a , multiplier) + normalizedOffset;
            quad.t1.b = ScaleWith(quad.t1.b , multiplier) + normalizedOffset;
            quad.t1.c = ScaleWith(quad.t1.c , multiplier) + normalizedOffset;
            quad.t2.a = ScaleWith(quad.t2.a , multiplier) + normalizedOffset;
            quad.t2.b = ScaleWith(quad.t2.b , multiplier) + normalizedOffset;
            quad.t2.c = ScaleWith(quad.t2.c , multiplier) + normalizedOffset;
            return quad;
        }
    }
}