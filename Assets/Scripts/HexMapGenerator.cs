using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class HexMapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera mainCamera;
    public HexPlayerMovement player;

    public TileBase grassTile;
    public TileBase waterTile;
    public TileBase mountainTile;

    public float noiseScale = 0.15f;
    public float waterThreshold = 0.35f;
    public float mountainThreshold = 0.7f;
    public int seed;

    public int minWalkableTiles = 100;

    public int mapWidthInCells = 100;
    public int mapHeightInCells = 100;

    void Start()
    {
        seed = Random.Range(0, 99999);
        Generate();
    }

    void Generate()
    {
        tilemap.ClearAllTiles();

        if (!mainCamera) mainCamera = Camera.main;

        int width = mapWidthInCells;
        int height = mapHeightInCells;

        for (int x = -width / 2; x < width / 2; x++)
            for (int y = -height / 2; y < height / 2; y++)
            {
                float nx = (x + seed) * noiseScale;
                float ny = (y + seed) * noiseScale;
                float noise = Mathf.PerlinNoise(nx, ny);

                TileBase tile =
                    noise < waterThreshold ? waterTile :
                    noise > mountainThreshold ? mountainTile :
                    grassTile;

                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }

        SpawnPlayerSafe();
    }

    void SpawnPlayerSafe()
    {
        int attempts = 1000;

        for (int i = 0; i < attempts; i++)
        {
            Vector3Int candidatePos = new Vector3Int(
                Random.Range(-mapWidthInCells / 4, mapWidthInCells / 4),
                Random.Range(-mapHeightInCells / 4, mapHeightInCells / 4),
                0
            );

            if (IsWalkable(candidatePos))
            {
                int reachableCount = GetReachableTilesCount(candidatePos);

                if (reachableCount >= minWalkableTiles)
                {
                    Debug.Log($"Spawn found at {candidatePos}. Reachable tiles: {reachableCount}");
                    player.spawnCell = candidatePos;
                    player.SnapToSpawn();
                    return;
                }
            }
        }

        Debug.LogError("Could not find a safe spawn point with " + minWalkableTiles + " reachable tiles!");
    }

    public bool IsWalkable(Vector3Int cell)
    {
        TileBase tile = tilemap.GetTile(cell);
        return tile == grassTile;
    }

    int GetReachableTilesCount(Vector3Int startNode)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        int count = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            count++;

            if (count >= minWalkableTiles) return count;

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && IsWalkable(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return count;
    }

    List<Vector3Int> GetNeighbors(Vector3Int center)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        bool isEvenRow = center.y % 2 == 0;

        Vector3Int[] offsets = isEvenRow ?
            new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0) } :
            new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0) };

        foreach (var offset in offsets)
        {
            neighbors.Add(center + offset);
        }

        return neighbors;
    }
}