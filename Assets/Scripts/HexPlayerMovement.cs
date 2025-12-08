using UnityEngine;
using UnityEngine.Tilemaps;

public class HexPlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveSpeed = 5f;

    public Vector3Int spawnCell;

    private Vector3 targetWorldPos;
    private bool isMoving = false;
    private Vector3Int currentCell;


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
                isMoving = false;
                currentCell = tilemap.WorldToCell(transform.position);
            }
        }
    }

    public void MoveToCell(Vector3Int targetCell)
    {
        if (!tilemap.HasTile(targetCell)) return;

        targetWorldPos = tilemap.GetCellCenterWorld(targetCell);
        isMoving = true;
    }

    public void SnapToSpawn()
    {
        if (tilemap == null) return;

        transform.position = tilemap.GetCellCenterWorld(spawnCell);
        currentCell = spawnCell;
        targetWorldPos = transform.position; 
        isMoving = false;
    }
}