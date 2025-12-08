using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class HexClickManager : MonoBehaviour
{
    public Tilemap tilemap;
    public HexPlayerMovement player;
    public HexMapGenerator mapGenerator;

    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindObjectOfType<HexMapGenerator>();
    }

    void Update()
    {
        if (Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screen = Pointer.current.position.ReadValue();
            Vector3 world = Camera.main.ScreenToWorldPoint(screen);
            world.z = 0;

            Vector3Int cell = tilemap.WorldToCell(world);

            if (tilemap.HasTile(cell))
            {
                
                if (mapGenerator != null && mapGenerator.IsWalkable(cell))
                {
                    player.MoveToCell(cell);
                }
                else
                {
                    Debug.Log("Cannot move here: It is an obstacle (Mountain/Water) or Generator is missing.");
                }
            }
        }
    }
}