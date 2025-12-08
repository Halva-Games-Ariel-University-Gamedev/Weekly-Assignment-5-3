using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class HexClickManager : MonoBehaviour
{
    public Tilemap tilemap;
    public HexPlayerMovement player;
    public HexMapGenerator mapGenerator;

    public Tilemap hoverTilemap;
    public TileBase hoverTile;

    private Vector3Int lastHoverCell = Vector3Int.zero;

    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindObjectOfType<HexMapGenerator>();
    }

    void Update()
    {
        if (Pointer.current == null) return;

        Vector2 screen = Pointer.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(screen);
        world.z = 0;

        Vector3Int currentCell = tilemap.WorldToCell(world);

        if (currentCell != lastHoverCell)
        {
            UpdateHover(currentCell);
            lastHoverCell = currentCell;
        }

        if (Pointer.current.press.wasPressedThisFrame)
        {
            if (tilemap.HasTile(currentCell))
            {
                player.MoveToCell(currentCell);
            }
        }
    }

    void UpdateHover(Vector3Int cell)
    {
        if (lastHoverCell != Vector3Int.zero && hoverTilemap != null)
        {
            hoverTilemap.SetTile(lastHoverCell, null);
        }

        if (mapGenerator == null || hoverTilemap == null || hoverTile == null) return;

        if (mapGenerator.IsWalkable(cell))
        {
            hoverTilemap.SetTile(cell, hoverTile);
        }
    }
}