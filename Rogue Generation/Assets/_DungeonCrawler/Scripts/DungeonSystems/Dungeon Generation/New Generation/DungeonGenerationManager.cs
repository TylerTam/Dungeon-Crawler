using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationManager : MonoBehaviour
{


    [Header("Unchanged Variables")]
    public float m_cellWorldSize = 1;
    public UnityEngine.Tilemaps.Tilemap m_floorTilemap, m_wallTilemap;


    [Header("Dungeon Type")]
    public DungeonGeneration_Theme m_dungeonTheme;

    [Header("Generated Data")]
    public FloorData m_floorData;
    public bool m_generate = false;
    public List<DungeonCellGridRow> m_dungeonCellGrid;

    [Header("Debugging")]
    public bool m_debug;
    public Gradient m_xColor, m_yColor;
    public bool m_debugGeneralFloorSize;
    public Vector2Int m_generalFloorSize;
    public Color m_generalFloorDebugColor;

    [Header("Runtime Variables")]
    public int m_currentFloor;

    private void Update()
    {
        if (m_generate)
        {
            GenerateFloor();
            m_generate = false;
        }
    }

    public void GenerateFloor()
    {
        m_floorTilemap.ClearAllTiles();
        m_wallTilemap.ClearAllTiles();
        m_floorData = m_dungeonTheme.GenerateFloor(m_currentFloor);
        m_dungeonTheme.PaintDungeon(m_floorData.m_floorLayout, m_wallTilemap, m_floorTilemap);
    }


    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        if (m_dungeonTheme != null)
        {
            if (m_currentFloor < m_dungeonTheme.m_floorData.Count)
            {
                Vector2Int cellAmount = m_dungeonTheme.m_floorData[m_currentFloor].m_cellAmount;
                Vector2Int cellSize = m_dungeonTheme.m_floorData[m_currentFloor].m_cellSize;
                for (int x = 0; x < cellAmount.x; x++)
                {
                    for (int y = 0; y < cellAmount.y; y++)
                    {
                        Gizmos.color = Color.Lerp(m_xColor.Evaluate((float)(x + 1) / (float)cellAmount.x), m_yColor.Evaluate((float)(x + 1) / (float)cellAmount.x), (float)(y + 1) / (float)cellAmount.y);
                        Gizmos.DrawCube(new Vector3(cellSize.x * x + (int)(cellSize.x / 2) + (cellSize.x % 2f) * 0.5f, -(cellSize.y * y + (int)(cellSize.y / 2) + (cellSize.y % 2f) * 0.5f)), new Vector3(cellSize.x, cellSize.y, 1));
                    }
                }
            }
        }

        if (m_debugGeneralFloorSize)
        {
            Gizmos.color = m_generalFloorDebugColor;
            Gizmos.DrawCube(new Vector3(m_generalFloorSize.x / 2, -m_generalFloorSize.y / 2, 10), new Vector3(m_generalFloorSize.x, m_generalFloorSize.y, .5f));
        }
    }
}

[System.Serializable]
public class FloorData
{
    public int[,] m_floorLayout;
    public List<RoomData> m_allRooms;
    public List<DungeonCellGridRow> m_cellGrid;

}
public class RoomData
{
    public int m_roomIndex;
    public List<Vector2Int> m_enemySpawnLocations;
}

[System.Serializable]
public class CellGridData
{
    public Vector2Int m_gridIndex;
    public bool m_isRoom = false;
    public bool m_isConnectionPoint;
    public List<Vector2Int> m_connectedCells = new List<Vector2Int>();
    public List<Vector2Int> m_hallwayToCell = new List<Vector2Int>();

    public Vector2Int m_northConnectionPoint, m_southConnectionPoint, m_eastConnectionPoint, m_westConnectionPoint;

    public DungeonGeneration_RoomLayout m_roomLayout;
    public DungeonGeneration_RoomLayout m_connectionLayout;
}

[System.Serializable]
public class DungeonCellGridRow
{
    public List<CellGridData> m_gridColumn;
}

