using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class HexPlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public HexMapGenerator mapGenerator;
    public float moveSpeed = 5f;

    public Vector3Int spawnCell;

    private Vector3 targetWorldPos;
    private bool isMoving = false;
    private Vector3Int currentCell;
    private Queue<Vector3Int> currentPath = new Queue<Vector3Int>();

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWorldPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
            {
                transform.position = targetWorldPos;
                currentCell = tilemap.WorldToCell(transform.position);

                if (currentPath.Count > 0)
                {
                    Vector3Int nextCell = currentPath.Dequeue();
                    targetWorldPos = tilemap.GetCellCenterWorld(nextCell);
                    isMoving = true;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
    }

    public void MoveToCell(Vector3Int targetCell)
    {
        currentPath.Clear();
        currentPath = FindPath(currentCell, targetCell);

        if (currentPath.Count > 0)
        {
            Vector3Int nextCell = currentPath.Dequeue();
            targetWorldPos = tilemap.GetCellCenterWorld(nextCell);
            isMoving = true;
        }
    }

    public void SnapToSpawn()
    {
        if (tilemap == null) return;
        transform.position = tilemap.GetCellCenterWorld(spawnCell);
        currentCell = spawnCell;
        targetWorldPos = transform.position;
        isMoving = false;
    }

    Queue<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        if (mapGenerator == null || !mapGenerator.IsWalkable(end))
        {
            return new Queue<Vector3Int>();
        }

        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(start);

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        cameFrom.Add(start, start);

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            if (current.Equals(end))
            {
                return ReconstructPath(cameFrom, start, end);
            }

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (mapGenerator.IsWalkable(neighbor) && !cameFrom.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom.Add(neighbor, current);
                }
            }
        }

        return new Queue<Vector3Int>();
    }

    Queue<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int end)
    {
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        Vector3Int current = end;

        while (!current.Equals(start))
        {
            stack.Push(current);
            current = cameFrom[current];
        }

        Queue<Vector3Int> path = new Queue<Vector3Int>();
        while (stack.Count > 0)
        {
            path.Enqueue(stack.Pop());
        }
        return path;
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