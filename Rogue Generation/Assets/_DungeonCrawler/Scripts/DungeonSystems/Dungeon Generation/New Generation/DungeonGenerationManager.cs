using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationManager : MonoBehaviour
{
    public DungeonGeneration_Theme m_dungeonTheme;
    public DungeonGeneration_GenerationLayout m_floorLayout;

    [Header("Unchanged Variables")]
    public float m_cellWorldSize = 1;
    public Vector2Int m_cellAmount;
    public Vector2Int m_cellSize;

    public UnityEngine.Tilemaps.Tilemap m_floorTilemap, m_wallTilemap;


    public FloorData m_floorData;
    public bool m_generate = false;

    public List<DungeonCellGridRow> m_dungeonCellGrid;

    public bool m_debug;
    public Gradient m_xColor, m_yColor;


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
        m_floorData = m_floorLayout.GenerateArray(m_dungeonTheme.m_roomTypesInDungeon, m_cellAmount, m_cellSize);
        m_dungeonTheme.PaintDungeon(m_floorData.m_floorLayout, m_wallTilemap, m_floorTilemap);
    }


    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        for (int x = 0; x < m_cellAmount.x; x++)
        {
            for (int y = 0; y < m_cellAmount.y; y++)
            {
                Gizmos.color = Color.Lerp(m_xColor.Evaluate((float)(x+1) / (float)m_cellAmount.x), m_yColor.Evaluate((float)(x+1) / (float)m_cellAmount.x), (float)(y+1)/ (float)m_cellAmount.y);
                Gizmos.DrawCube(new Vector3(m_cellSize.x * x + (int)(m_cellSize.x / 2) + (m_cellSize.x % 2f) * 0.5f,- (m_cellSize.y * y + (int)(m_cellSize.y / 2) + (m_cellSize.y % 2f) *0.5f)), new Vector3(m_cellSize.x, m_cellSize.y, 1));
            }
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
}

[System.Serializable]
public class DungeonCellGridRow
{
    public List<CellGridData> m_gridColumn;
}

