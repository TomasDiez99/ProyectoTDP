using Terrain.Structures;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunk : MonoBehaviour, Chunk
    {
        MeshRenderer meshRender;
        public ChunkIndex Index { get; set; }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        void Start()
        {
            meshRender = GetComponent<MeshRenderer>();
        }
    }
}