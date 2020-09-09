using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Terrain.Structures;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain
{
    public class ChunkGeneration : MonoBehaviour
    {
        const float MaxTimeBetweenRecalculation = 20;

        readonly RaycastHit[] results = new RaycastHit[20];

        [FormerlySerializedAs("chunkBuilder")] [FormerlySerializedAs("chunkFactory")] public ChunkBuilderAsset chunkBuilderAsset;

        public LayerMask chunksLayerMask;
        Coroutine currentRoutine;

        Vector3 lastPlayerPosition;
        
        float lastRecalculationTimeStamp;
        
        public int maxRadius = 5;


        ChunkPool memoryManager;

        public int minRadius = 2;
        public Transform player;

        
        float ChunkSize => chunkBuilderAsset.ChunkSize;

        
        readonly List<ChunkIndex> removeQueue = new List<ChunkIndex>();
        
        void Start()
        {
            memoryManager = new SpatialChunkPool(chunkBuilderAsset);
            InitializeFirstChunks();
        }

        ChunkIndex GetIndex(Vector3 position)
        {
            const float topY = 100000;
            const float rayLen = topY * 2;
            var ini = position;
            ini.y = topY;
            var hitCount = Physics.RaycastNonAlloc(ini, Vector3.down, results, rayLen, chunksLayerMask);
            for (var i = 0; i < hitCount; i++)
            {
                var chunk = results[i].collider.GetComponent<Chunk>();
                if (chunk != null) return chunk.Index;
            }

            var indexX = position.x / ChunkSize;
            var indexY = position.z / ChunkSize;
            return new ChunkIndex(Mathf.RoundToInt(indexX), Mathf.RoundToInt(indexY));
        }


        Vector2Int GetSubChunk(Vector3 point, float subDivisionSize)
        {
            point.y = 0;
            var indexX = point.x / (ChunkSize / subDivisionSize);
            var indexY = point.z / (ChunkSize / subDivisionSize);
            return new Vector2Int(Mathf.RoundToInt(indexX), Mathf.RoundToInt(indexY));
        }

        void Update()
        {
            lastRecalculationTimeStamp += Time.deltaTime;
            var currentPosition = player.position;


            const float subdivisionChunk = 4;
            if (GetSubChunk(currentPosition, subdivisionChunk) != GetSubChunk(lastPlayerPosition, subdivisionChunk) ||
                lastRecalculationTimeStamp > MaxTimeBetweenRecalculation)
            {
                lastRecalculationTimeStamp = 0;
                UpdateMemory();
            }

            lastPlayerPosition = currentPosition;
        }

        void UpdateMemory()
        {
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(UpdateMemoryCoroutine());
        }
        
        void InitializeFirstChunks()
        {
            for (var x = 0; x < maxRadius; x++)
            for (var y = 0; y < maxRadius; y++)
            {
                CheckEnable(new ChunkIndex(x, y));
                CheckEnable(new ChunkIndex(-x, y));
                CheckEnable(new ChunkIndex(x, -y));
                CheckEnable(new ChunkIndex(-x, -y));
            }
        }
        
        IEnumerator UpdateMemoryCoroutine()
        {
            var currentChunkIndex = GetIndex(player.position);

            // primero chequeo donde está el jugador parado
            CheckEnable(currentChunkIndex);
            //chequeamos los vecinos
            for (var passCount = 1; passCount < maxRadius; passCount++)
            for (var i = 0; i <= passCount; i++) // el <= es para que chequee las esquinas tambien
            {
                var meshesInstantiated = CheckSideMirrored(passCount, i, currentChunkIndex);
                meshesInstantiated |= CheckSideMirrored(i,passCount, currentChunkIndex);
                if (meshesInstantiated) // if meshes have been instantiated
                    yield return null; // <- devuelve el flow a unity hasta el siguiente update
            }

            yield return null;
            
            removeQueue.ResetWithCapacity(memoryManager.PoolCount);
            
            LoadGarbageChunks();

            yield return null;
            
            foreach (var index in removeQueue)
            {
                memoryManager.EnsureDeleted(index);
            }
            currentRoutine = null;
        }

        void LoadGarbageChunks()
        {
            foreach (var chunk in memoryManager.GetChunks())
            {
                var distance = DistanceToPlayer(chunk.Index);
                if (distance > maxRadius * ChunkSize)
                {
                    removeQueue.Add(chunk.Index);
                }
                else
                {
                    if (distance > minRadius * ChunkSize) memoryManager.EnsureHidden(chunk.Index);
                }
            }
        }


        bool CheckSideMirrored(int x, int y, ChunkIndex currentChunkIndex)
        {
            // operates in each quadrant
            var generates = false;
            var direction = new ChunkIndex(x, y);//quadrant + +
            generates |= CheckEnable(currentChunkIndex + direction);
            direction.x = -direction.x;//quadrant - +
            generates |= CheckEnable(currentChunkIndex + direction);
            direction.z = -direction.z;//quadrant - -
            generates |= CheckEnable(currentChunkIndex + direction);
            direction.x = -direction.x;//quadrant + -
            generates |= CheckEnable(currentChunkIndex + direction);
            return generates;
        }

        bool CheckEnable(ChunkIndex index)
        {
            var distance = DistanceToPlayer(index);
            if (distance < minRadius * ChunkSize) return memoryManager.EnsureActive(index);
            return false;
        }


        float DistanceToPlayer(ChunkIndex index)
        {
            var chunkPosition = GetChunkCenter(index);
            var playerPos = player.position;
            chunkPosition.y = 0;
            playerPos.y = 0;
            return Vector3.Distance(playerPos, chunkPosition);
        }

        Vector3 GetChunkCenter(ChunkIndex index)
        {
            var offset = new Vector3(ChunkSize * 0.5f, 0, ChunkSize * 0.5f);
            return SpaceMathService.GetChunkPosition(index.x,index.z, ChunkSize) - offset;
        }
    }
}