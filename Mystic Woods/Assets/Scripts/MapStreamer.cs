using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapStreamer : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Tilemap floorTilemap;
    public TileBase floorTile; // RuleTile or single Tile

    [Header("Streaming")]
    public int chunkSize = 16;          // tiles per side
    public int viewRadiusChunks = 2;    // how many chunks out to keep loaded
    public int seed = 12345;            // set useSeed = false for random each run
    public bool useSeed = true;

    [Header("Generation")]
    [Range(0f, 1f)] public float noiseThreshold = 0.45f; // lower = more floor
    public float noiseScale = 0.12f; // larger = smoother blobs

    // Tracks which chunks exist; value = list of painted cell positions for fast culling
    private readonly Dictionary<Vector2Int, List<Vector3Int>> _chunkCells = new();

    void Start()
    {
        if (!player || !floorTilemap || !floorTile)
        {
            Debug.LogError("Assign Player, FloorTilemap, and FloorTile.");
            enabled = false;
            return;
        }
        if (!useSeed) seed = Random.Range(int.MinValue, int.MaxValue);
    }

    void Update()
    {
        var playerChunk = WorldToChunk(player.position);
        // Generate nearby
        for (int cy = -viewRadiusChunks; cy <= viewRadiusChunks; cy++)
        {
            for (int cx = -viewRadiusChunks; cx <= viewRadiusChunks; cx++)
            {
                var c = new Vector2Int(playerChunk.x + cx, playerChunk.y + cy);
                if (!_chunkCells.ContainsKey(c))
                    GenerateChunk(c);
            }
        }
        // Cull far
        CullFarChunks(playerChunk);
    }

    Vector2Int WorldToChunk(Vector3 worldPos)
    {
        // Convert world to tile cell, then to chunk coords
        var cell = (Vector3Int)floorTilemap.WorldToCell(worldPos);
        int cx = Mathf.FloorToInt((float)cell.x / chunkSize);
        int cy = Mathf.FloorToInt((float)cell.y / chunkSize);
        return new Vector2Int(cx, cy);
    }

    void GenerateChunk(Vector2Int chunk)
    {
        var painted = new List<Vector3Int>();
        var origin = new Vector3Int(chunk.x * chunkSize, chunk.y * chunkSize, 0);

        // Use seeded Perlin so chunks are deterministic and stitch seamlessly
        float baseX = (seed * 0.013123f) + 10000f;
        float baseY = (seed * 0.017777f) - 5000f;

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                var cell = new Vector3Int(origin.x + x, origin.y + y, 0);

                // Perlin noise in world space (so chunks match at borders)
                float wx = (cell.x + baseX) * noiseScale;
                float wy = (cell.y + baseY) * noiseScale;
                float n = Mathf.PerlinNoise(wx, wy);

                if (n > noiseThreshold)
                {
                    floorTilemap.SetTile(cell, floorTile);
                    painted.Add(cell);
                }
            }
        }

        floorTilemap.RefreshAllTiles();
        _chunkCells[chunk] = painted;
    }

    void CullFarChunks(Vector2Int playerChunk)
    {
        // Anything outside (viewRadiusChunks + 1) is culled
        int limit = viewRadiusChunks + 1;
        var toRemove = new List<Vector2Int>();

        foreach (var kv in _chunkCells)
        {
            var c = kv.Key;
            if (Mathf.Abs(c.x - playerChunk.x) > limit || Mathf.Abs(c.y - playerChunk.y) > limit)
            {
                // Clear painted cells
                foreach (var cell in kv.Value)
                    floorTilemap.SetTile(cell, null);
                toRemove.Add(c);
            }
        }

        foreach (var c in toRemove)
            _chunkCells.Remove(c);
    }
}
