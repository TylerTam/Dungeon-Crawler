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
    public bool m_debugCellSizes;
    public Gradient m_xColor, m_yColor;
    public bool m_debugGeneralFloorSize;
    public Vector2Int m_generalFloorSize;
    public Color m_generalFloorDebugColor;

    public bool m_isDebuggingFloorGeneration;
    public bool m_generate = false;

    public bool m_debugEntityCheck;
    public Color m_debugEntityColor;

    //Used to keep track of entities
    //Used to replace raycasts check, and to check this 2d array instead for detection
    public GameObject[,] m_runtimeGridOccupancy;
    public GameObject[,] m_interactableGridOccupancy;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Input_Base.Instance.m_canPerform = false;
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

        m_runtimeGridOccupancy = new GameObject[m_floorData.m_floorLayout.GetLength(0), m_floorData.m_floorLayout.GetLength(1)];
        m_interactableGridOccupancy = new GameObject[m_floorData.m_floorLayout.GetLength(0), m_floorData.m_floorLayout.GetLength(1)];


        #region Placing player team

        int currentRoomIndex = Random.Range(0, m_floorData.m_allRooms.Count);
        Vector3 newPos = Vector3.zero;
        for (int i = 0; i < PlayerDungeonManager.Instance.m_playerTeam.Count; i++)
        {
            newPos = (Vector2)(m_floorData.m_allRooms[currentRoomIndex].m_roomCenterWorldPos + m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations[Random.Range(0, m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations.Count)]);
            newPos = new Vector3(newPos.x + 0.5f, -newPos.y - 0.5f, 0);


            PlayerDungeonManager.Instance.m_playerTeam[i].transform.position = newPos;
            PlayerDungeonManager.Instance.m_playerTeam[i].GetComponent<EntityContainer>().m_turnBasedAgent.SetupCellAttendence();
        }
        #endregion

        #region Placing Staircase

        currentRoomIndex = Random.Range(0, m_floorData.m_allRooms.Count);

        newPos = (Vector2)(m_floorData.m_allRooms[currentRoomIndex].m_roomCenterWorldPos + m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations[Random.Range(0, m_floorData.m_allRooms[currentRoomIndex].m_enemySpawnLocations.Count)]);
        newPos = new Vector3(newPos.x + 0.5f, -newPos.y - 0.5f, 0);
        FloorObject_Staircase.Instance.transform.position = newPos;
        m_interactableGridOccupancy[(int)newPos.x, Mathf.Abs((int)newPos.y)] = FloorObject_Staircase.Instance.gameObject;

        #endregion

        AIManager.Instance.m_currentAIFloorData = m_dungeonTheme.m_floorData[m_currentFloor].m_aiOnFloor;

        yield return null;
        Input_Base.Instance.m_canPerform = true;
    }

    public void NewFloor()
    {
        ///Disable turn based movement
        ///
        ///Fade to black;
        ///

        AIManager.Instance.ClearFloor();
        GenerateFloor();
        ///
        ///Fade out black
        ///
    }


    #region Getter Functions

    public bool IsInGrid(float x, float y)
    {
        if ((int)x >= m_floorData.m_floorLayout.GetLength(0) || (int)x < 0) return false;
        if ((int)-y >= m_floorData.m_floorLayout.GetLength(0) || (int)-y < 0) return false;
        return true;
    }

    public int GetWallCheck(float x, float y)
    {
        return m_floorData.m_floorLayout[(int)x, Mathf.Abs((int)y)];
    }
    public GameObject GetEntityCheck(float x, float y)
    {
        return m_runtimeGridOccupancy[(int)x, Mathf.Abs((int)y)];
    }

    public GameObject GetEntityCheck(Vector3 p_pos)
    {
        return GetEntityCheck(p_pos.x, p_pos.y);
    }


    public GameObject GetEntityCheck(Vector2 p_pos)
    {
        return m_runtimeGridOccupancy[(int)p_pos.x, Mathf.Abs((int)p_pos.y)];
    }

    public void AdjustEntityCheckGrid(float x, float y, GameObject p_occupiedObject)
    {
        m_runtimeGridOccupancy[(int)x, Mathf.Abs((int)y)] = p_occupiedObject;
    }

    public bool IsCellClear(float x, float y)
    {
        if (m_runtimeGridOccupancy[(int)x, Mathf.Abs((int)y)] == null)
        {
            if (m_interactableGridOccupancy[(int)x, Mathf.Abs((int)y)] == null)
            {
                return true;
            }
        }
        return false;
    }



    public Vector3 GetRandomTargetPosition(float x, float y, Vector2 p_facingDir, bool p_lostPlayer)
    {
        int currentTileType = m_floorData.m_floorLayout[(int)x, (int)Mathf.Abs(y)];

        #region If they are in a room, get a random exit if they can
        ///If they are in a room, get a different exit point. If there is only one exit point, get the center of the room
        if (currentTileType > GlobalVariables.m_startingWalkable || currentTileType < GlobalVariables.m_hazardCell)
        {
            RoomData currentRoom = m_floorData.m_allRooms[(currentTileType > 0) ? (currentTileType - GlobalVariables.m_startingWalkable - 1) : (currentTileType + GlobalVariables.m_hazardCell + 1)];

            ///If the direction they are facing is going into the room
            if ((int)x + p_facingDir.x <= currentRoom.m_roomCenterWorldPos.x + currentRoom.m_roomSize.x && (int)x + p_facingDir.x >= currentRoom.m_roomCenterWorldPos.x - currentRoom.m_roomSize.x &&
                Mathf.Abs((int)y + p_facingDir.y) <= currentRoom.m_roomCenterWorldPos.y + currentRoom.m_roomSize.y && Mathf.Abs((int)y + p_facingDir.y) >= currentRoom.m_roomCenterWorldPos.y - currentRoom.m_roomSize.y)
            {

                #region Logic for being in room
                List<Vector2Int> possiblePoints = new List<Vector2Int>();

                if (p_lostPlayer)
                {
                    float currentDis = 1000;
                    Vector2Int currentExit = Vector2Int.zero;
                    Vector2 playerPos = PlayerDungeonManager.Instance.m_playerEntityContainer.transform.position;
                    playerPos = new Vector2(playerPos.x, Mathf.Abs(playerPos.y));
                    foreach (Vector2Int exitPoints in currentRoom.m_exitPoints)
                    {
                        float curDis = Vector2.Distance(currentRoom.m_roomCenterWorldPos + exitPoints, playerPos);
                        if (curDis < currentDis)
                        {
                            currentDis = curDis;
                            currentExit = currentRoom.m_roomCenterWorldPos + exitPoints;
                        }
                    }
                    return (Vector3)(Vector2)currentExit;
                }
                else
                {
                    foreach (Vector2Int exitPoints in currentRoom.m_exitPoints)
                    {
                        if ((int)x == exitPoints.x + currentRoom.m_roomCenterWorldPos.x && Mathf.Abs((int)(y)) == exitPoints.y + currentRoom.m_roomCenterWorldPos.y) continue;
                        possiblePoints.Add(exitPoints);
                    }
                }

                if (possiblePoints.Count == 0)
                {
                    return (Vector2)currentRoom.m_roomCenterWorldPos;
                }

                List<Vector2Int> pointsInFacingDir = new List<Vector2Int>();

                foreach (Vector2Int point in possiblePoints)
                {
                    if (Mathf.Sign(x - point.x) == Mathf.Sign(p_facingDir.x) && p_facingDir.x != 0 && Mathf.Sign(y - point.y) == Mathf.Sign(p_facingDir.y) && p_facingDir.y != 0)
                    {
                        pointsInFacingDir.Add(point);
                    }
                }
                if (pointsInFacingDir.Count > 0)
                {
                    return (Vector2)currentRoom.m_roomCenterWorldPos + (Vector2)pointsInFacingDir[Random.Range(0, pointsInFacingDir.Count)];
                }

                int randomIndexxxx = Random.Range(0, possiblePoints.Count);
                return (Vector2)currentRoom.m_roomCenterWorldPos + (Vector2)possiblePoints[randomIndexxxx];
                #endregion
            }

            ///If they are facing out of the room, go to the appropriate cell
            else
            {
                #region Logic for leaving Room
                Vector2Int roomCheckCell = new Vector2Int((int)(x / m_dungeonTheme.m_floorData[m_currentFloor].m_cellSize.x),
                                        (int)((Mathf.Abs(y)) / m_dungeonTheme.m_floorData[m_currentFloor].m_cellSize.y));

                if (p_facingDir.x < 0)
                {
                    foreach (Vector2Int connect in m_floorData.m_cellGrid[roomCheckCell.x].m_gridColumn[roomCheckCell.y].m_connectedCells)
                    {
                        if (connect.x - roomCheckCell.x == -1)
                        {
                            return (Vector2)m_floorData.m_cellGrid[connect.x].m_gridColumn[connect.y].m_eastConnectionPoint;
                        }
                    }
                }
                else if (p_facingDir.x > 0)
                {
                    foreach (Vector2Int connect in m_floorData.m_cellGrid[roomCheckCell.x].m_gridColumn[roomCheckCell.y].m_connectedCells)
                    {
                        if (connect.x - roomCheckCell.x == 1)
                        {

                            return (Vector2)m_floorData.m_cellGrid[connect.x].m_gridColumn[connect.y].m_westConnectionPoint;
                        }
                    }
                }
                else if (p_facingDir.y < 0)
                {
                    foreach (Vector2Int connect in m_floorData.m_cellGrid[roomCheckCell.x].m_gridColumn[roomCheckCell.y].m_connectedCells)
                    {
                        if (connect.y - roomCheckCell.y == 1)
                        {

                            return (Vector2)m_floorData.m_cellGrid[connect.x].m_gridColumn[connect.y].m_northConnectionPoint;
                        }
                    }
                }
                else if (p_facingDir.y > 0)
                {
                    foreach (Vector2Int connect in m_floorData.m_cellGrid[roomCheckCell.x].m_gridColumn[roomCheckCell.y].m_connectedCells)
                    {
                        if (connect.y - roomCheckCell.y == -1)
                        {

                            return (Vector2)m_floorData.m_cellGrid[connect.x].m_gridColumn[connect.y].m_southConnectionPoint;
                        }
                    }
                }
                #endregion
            }
        }

        #endregion

        #region If they are in a hallway, get a random connection point

        ///If they are in a hallway, or entering a hallway
        Vector2Int cellIndex = new Vector2Int((int)(x / m_dungeonTheme.m_floorData[m_currentFloor].m_cellSize.x),
                                                (int)((Mathf.Abs(y)) / m_dungeonTheme.m_floorData[m_currentFloor].m_cellSize.y));

        CellGridData currentCell = m_floorData.m_cellGrid[cellIndex.x].m_gridColumn[cellIndex.y];

        List<Vector2Int> hallwayPossiblePoints = new List<Vector2Int>();

        /*if (!p_lostPlayer)
        {
            DungeonGenerationManager
        }
        else
        {*/
            foreach (Vector2Int connection in currentCell.m_connectedCells)
            {
                //if its different horizontally
                if (connection.x != currentCell.m_gridIndex.x && connection.y == currentCell.m_gridIndex.y)
                {
                    if (Mathf.Sign(p_facingDir.x) == Mathf.Sign(connection.x - currentCell.m_gridIndex.x) || p_facingDir.x == 0)
                    {
                        if (Mathf.Sign(connection.x - currentCell.m_gridIndex.x) == 1)
                        {
                            hallwayPossiblePoints.Add(m_floorData.m_cellGrid[connection.x].m_gridColumn[connection.y].m_westConnectionPoint);
                        }
                        else
                        {
                            hallwayPossiblePoints.Add(m_floorData.m_cellGrid[connection.x].m_gridColumn[connection.y].m_eastConnectionPoint);
                        }
                    }
                }
                else if (connection.x == currentCell.m_gridIndex.x && connection.y != currentCell.m_gridIndex.y)
                {
                    if (Mathf.Sign(p_facingDir.y) != Mathf.Sign(connection.y - currentCell.m_gridIndex.y) || p_facingDir.y == 0)
                    {
                        if (Mathf.Sign(connection.y - currentCell.m_gridIndex.y) == 1)
                        {
                            hallwayPossiblePoints.Add(m_floorData.m_cellGrid[connection.x].m_gridColumn[connection.y].m_northConnectionPoint);
                        }
                        else
                        {
                            hallwayPossiblePoints.Add(m_floorData.m_cellGrid[connection.x].m_gridColumn[connection.y].m_southConnectionPoint);
                        }
                    }
                }
            }
        //}

        if (hallwayPossiblePoints.Count > 0)
        {
            return (Vector2)hallwayPossiblePoints[Random.Range(0, hallwayPossiblePoints.Count)];
        }

        if ((int)x == currentCell.m_centerWorldPosition.x && Mathf.Abs((int)y) == currentCell.m_centerWorldPosition.y)
        {
            Vector2Int currentLastCell = currentCell.m_connectedCells[Random.Range(0, currentCell.m_connectedCells.Count)];
            if (currentCell.m_gridIndex.x > currentLastCell.x)
            {
                return (Vector2)m_floorData.m_cellGrid[currentLastCell.x].m_gridColumn[currentLastCell.y].m_eastConnectionPoint;
            }
            else if (currentCell.m_gridIndex.x < currentLastCell.x)
            {
                return (Vector2)m_floorData.m_cellGrid[currentLastCell.x].m_gridColumn[currentLastCell.y].m_westConnectionPoint;
            }
            else if (currentCell.m_gridIndex.y > currentLastCell.y)
            {
                return (Vector2)m_floorData.m_cellGrid[currentLastCell.x].m_gridColumn[currentLastCell.y].m_southConnectionPoint;
            }
            else if (currentCell.m_gridIndex.x < currentLastCell.x)
            {
                return (Vector2)m_floorData.m_cellGrid[currentLastCell.x].m_gridColumn[currentLastCell.y].m_northConnectionPoint;
            }
            else
            {
                return (Vector2)m_floorData.m_cellGrid[currentLastCell.x].m_gridColumn[currentLastCell.y].m_centerWorldPosition;
            }
        }

        return (Vector2)currentCell.m_centerWorldPosition;

        #endregion
    }
    #endregion


    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        if (m_debugCellSizes)
        {
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
        }

        if (m_debugGeneralFloorSize)
        {
            Gizmos.color = m_generalFloorDebugColor;
            Gizmos.DrawCube(new Vector3(m_generalFloorSize.x / 2, -m_generalFloorSize.y / 2, 10), new Vector3(m_generalFloorSize.x, m_generalFloorSize.y, .5f));
        }

        if (m_debugEntityCheck)
        {
            Gizmos.color = m_debugEntityColor;
            if (m_runtimeGridOccupancy != null)
            {
                for (int x = 0; x < m_runtimeGridOccupancy.GetLength(0); x++)
                {
                    for (int y = 0; y < m_runtimeGridOccupancy.GetLength(1); y++)
                    {
                        if (m_runtimeGridOccupancy[x, y] != null)
                        {
                            Gizmos.DrawCube(new Vector3(x + 0.5f, -(y + 0.5f), 0), new Vector3(1, 1, .25f));
                        }
                    }
                }
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
[System.Serializable]
public class RoomData
{
    public int m_roomIndex;
    public Vector2Int m_roomCellIndex;
    public List<Vector2Int> m_enemySpawnLocations;
    public List<Vector2Int> m_itemSpawnLocations;
    public List<Vector2Int> m_trapSpawnLocations;
    public Vector2Int m_roomCenterWorldPos;
    public Vector2Int m_roomSize;
    public List<Vector2Int> m_exitPoints;
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

    public Vector2Int m_centerWorldPosition;

    public DungeonGeneration_RoomLayout m_roomLayout;
    public DungeonGeneration_RoomLayout m_connectionLayout;
}

[System.Serializable]
public class DungeonCellGridRow
{
    public List<CellGridData> m_gridColumn;
}

