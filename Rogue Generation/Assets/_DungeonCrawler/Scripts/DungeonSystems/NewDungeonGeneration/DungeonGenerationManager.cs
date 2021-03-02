using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonGenerationManager : MonoBehaviour
{
    public static DungeonGenerationManager Instance;

    [Header("Unchanged Variables")]
    public float m_cellWorldSize = 1;
    public UnityEngine.Tilemaps.Tilemap m_floorTilemap, m_wallTilemap;


    [Header("Dungeon Type")]
    public DungeonGeneration_Theme m_dungeonTheme;
    public DungeonNavigation m_navGrid;
    public DungeonNavigation_Agent m_checkAgent;

    [Header("Generated Data")]
    public FloorData m_floorData;
    public List<DungeonCellGridRow> m_dungeonCellGrid;



    [Header("Runtime Variables")]
    public int m_currentFloor;


    [Header("Debugging")]
    public bool m_debug;
    public Gradient m_xColor, m_yColor;
    public bool m_debugGeneralFloorSize;
    public Vector2Int m_generalFloorSize;
    public Color m_generalFloorDebugColor;

    public bool m_isDebuggingFloorGeneration;
    public bool m_generate = false;

    //Used to keep track of entities
    //Used to replace raycasts check, and to check this 2d array instead for detection
    public int[,] m_runtimeGridOccupancy;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GenerateFloor();
    }
    private void Update()
    {
        if (!m_isDebuggingFloorGeneration) return;
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
        StartCoroutine(GenerationCoroutine());

    }

    private IEnumerator GenerationCoroutine()
    {
        bool canPass = false;
        while (!canPass)
        {
            m_floorData = m_dungeonTheme.GenerateFloor(m_currentFloor);
            m_navGrid.GenerateGrid(m_floorData.m_floorLayout);



            for (int i = 1; i < m_floorData.m_allRooms.Count; i++)
            {
                if (m_checkAgent.CreatePath((Vector2)m_floorData.m_allRooms[0].m_roomCenterWorldPos, (Vector2)m_floorData.m_allRooms[i].m_roomCenterWorldPos) == null)
                {
                    //yield return new WaitForSeconds(0.5f);
                    canPass = false;
                    break;
                }
                else
                {
                    canPass = true;

                }
            }
            yield return null;
        }
        m_dungeonTheme.PaintDungeon(m_floorData.m_floorLayout, m_wallTilemap, m_floorTilemap);


        int currentRoomIndex = Random.Range(0, m_floorData.m_allRooms.Count);
        Vector3 newPos = Vector3.zero;
        for (int i = 0; i < PlayerDungeonManager.Instance.m_playerTeam.Count; i++)
        {
            newPos = (Vector2)(m_floorData.m_allRooms[currentRoomIndex].m_roomCenterWorldPos + m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations[Random.Range(0, m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations.Count)]);
            newPos = new Vector3(newPos.x + 0.5f, -newPos.y - 0.5f, 0);


            PlayerDungeonManager.Instance.m_playerTeam[i].transform.position = newPos;
        }
        currentRoomIndex = Random.Range(0, m_floorData.m_allRooms.Count);

        newPos = (Vector2)(m_floorData.m_allRooms[currentRoomIndex].m_roomCenterWorldPos + m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations[Random.Range(0, m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations.Count)]);
        newPos = new Vector3(newPos.x + 0.5f, -newPos.y - 0.5f, 0);
        FloorObject_Staircase.Instance.transform.position = newPos;
    }

    public void NewFloor()
    {
        ///Disable turn based movement
        ///
        ///Fade to black;
        ///
        GenerateFloor();
        ///
        ///Fade out black
        ///
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
[System.Serializable]
public class RoomData
{
    public int m_roomIndex;
    public Vector2Int m_roomCellIndex;
    public List<Vector2Int> m_enemySpawnLocations;
    public List<Vector2Int> m_itemSpawnLocations;
    public List<Vector2Int> m_trapSpawnLocations;
    public Vector2Int m_roomCenterWorldPos;
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

