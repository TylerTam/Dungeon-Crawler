using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    public UnityEngine.Tilemaps.Tilemap m_wallTiles, m_floorTiles, m_miniMapTiles;
    public UnityEngine.Tilemaps.Tile m_miniMapTile;
    public DungeonTheme m_dungeonTheme;
    public DungeonType_Base m_currentDungeonType;
    public List<DungeonGridCell> m_allRooms;
    public List<DungeonGridCell> m_hallwayPoints;
    private DungeonNavigation m_dungeonNav;
    private DungeonNavigation_Agent m_gridTestAgent;

    public List<GameObject> m_itemsOnFloor;

    /// <summary>
    /// Used to create the dungeon's layout.
    /// </summary>
    public DungeonGridCell[,] m_allCells;

    [HideInInspector]
    public int m_dungeonGenTypeIndex;
    //private bool mapGenerated = false;

    private ObjectPooler m_pooler;
    private TurnBasedManager m_turnSystem;


    public List<ItemStruct> m_fixedRatios;

    public GameObject m_playerObject;
    Input_Base m_playerInput;
    public GameObject m_staircase;

    /// <summary>
    /// Used to ensure the staircase, ai, and friendly characters dont spawn on each other
    /// </summary>
    List<Vector2> m_occupiedSpaces;



    [Header("AI Properties")]
    public GameObject m_aiShell;
    public LayerMask m_spawnMask;
    private List<AiStruct> m_currentEnemyTypesInDungeon;


    #region Initialization
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Awake()
    {
        m_dungeonNav = GetComponent<DungeonNavigation>();
        m_gridTestAgent = GetComponent<DungeonNavigation_Agent>();
        m_occupiedSpaces = new List<Vector2>();
        m_playerInput = m_playerObject.GetComponent<Input_Base>();
    }
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        m_turnSystem = TurnBasedManager.Instance;
        StartCoroutine(CheckDungeonConnection());
    }

    #endregion

    #region Room Generation
    public void NewFloor()
    {
        //Coroutine CreateDungeon = StartCoroutine(CheckDungeonConnection());
        AIManager.Instance.ClearFloor();
        m_turnSystem.ClearAgents();

        StartCoroutine(CheckDungeonConnection());
    }



    IEnumerator CheckDungeonConnection()
    {
        bool mapSuccess = false;

        while (!mapSuccess)
        {
            foreach (GameObject item in m_itemsOnFloor)
            {
                m_pooler.ReturnToPool(item);
            }
            m_itemsOnFloor.Clear();

            m_allRooms.Clear();
            m_hallwayPoints.Clear();
            m_wallTiles.ClearAllTiles();
            m_floorTiles.ClearAllTiles();
            m_miniMapTiles.ClearAllTiles();



            m_allRooms = m_dungeonTheme.CreateNewFloor(this, m_dungeonNav);
            m_currentEnemyTypesInDungeon = m_dungeonTheme.AiInDungeon();

            yield return new WaitForEndOfFrame();

            m_dungeonNav.m_gridWorldSize = new Vector2(m_dungeonTheme.m_generationTypes[m_dungeonGenTypeIndex].m_cellSize.x * m_dungeonTheme.m_generationTypes[m_dungeonGenTypeIndex].m_cellsInDungeon.x, m_dungeonTheme.m_generationTypes[m_dungeonGenTypeIndex].m_cellSize.y * m_dungeonTheme.m_generationTypes[m_dungeonGenTypeIndex].m_cellsInDungeon.y);
            m_dungeonNav.m_gridOrigin = m_dungeonNav.m_gridWorldSize / 2;
            m_dungeonNav.CreateGrid();
            Vector3 startPoint = m_allRooms[0].m_worldPos;
            bool roomFailed = false;
            for (int i = 1; i < m_allRooms.Count; i++)
            {
                List<Node> path = m_gridTestAgent.CreatePath(startPoint, m_allRooms[i].m_worldPos);
                if (path == null)
                {
                    roomFailed = true;
                }
            }
            mapSuccess = !roomFailed;

            /*TODO
            /// Currently, the path is drawn first, to create all the physical colliders, generate the navigation grid, and uses a* to determine if the path is possible.<br/>
            /// But maybe its more efficient to utilize cell types isntead, so there are no physics and art parts to that.
             */


            /*if (roomFailed)
            {
                m_floorTiles.color = Color.red;
                yield return new WaitForSeconds(.25f);
            }
            yield return new WaitForSeconds(.1f);
            m_floorTiles.color = Color.white;*/
            //mapSuccess = false;

        }
        List<int> removeIndex = new List<int>();
        for (int i = 0; i < m_hallwayPoints.Count; i++)
        {
            if (m_hallwayPoints[i].m_connectedTo.Count == 0)
            {
                Debug.LogWarning("Hallway point: " + m_hallwayPoints[i].m_worldPos + " is not connected to anything. Removing");
                removeIndex.Add(i);
            }
        }
        removeIndex.Reverse();
        foreach (int i in removeIndex)
        {
            m_hallwayPoints.RemoveAt(i);
        }
        m_occupiedSpaces.Clear();

        Vector2 randomPos = RandomSpawnPosition(m_allRooms);
        m_playerObject.transform.position = randomPos;
        m_occupiedSpaces.Add(randomPos);

        randomPos = RandomSpawnPosition(m_allRooms);
        m_staircase.transform.position = randomPos;
        m_occupiedSpaces.Add(randomPos);

        m_playerInput.m_canPerform = true;

    }

    Vector2 RandomSpawnPosition(List<DungeonGridCell> m_possibleRooms)
    {
        int randomRoomSpawn = Random.Range(0, m_allRooms.Count);
        List<Vector2> possibleSpawnPos = new List<Vector2>();
        foreach (Vector2 pos in m_possibleRooms[randomRoomSpawn].m_floorTiles)
        {
            Vector2 centeredPos = pos + new Vector2(.5f, .5f);
            if (!Physics2D.Raycast(centeredPos, Vector3.forward, Mathf.Infinity) && !m_occupiedSpaces.Contains(centeredPos))
            {
                possibleSpawnPos.Add(centeredPos);
            }

        }

        randomRoomSpawn = Random.Range(0, possibleSpawnPos.Count);

        return possibleSpawnPos[randomRoomSpawn];
    }

    #endregion

    #region AI Methods

    /// <summary>
    /// This method is called by the AIController script, when it desires a new path / target
    /// This returns the world position of a random room, or hallway, that is not the currently occupied one
    /// </summary>

    public Vector3 GetRandomCell(Vector3 m_currentPos)
    {

        DungeonGridCell currentCell = CurrentCell(m_currentPos);

        List<DungeonGridCell> randomCell = new List<DungeonGridCell>();

        bool targetRoom = (Random.Range(0f, 1f) > .5) ? true : false;

        if (targetRoom)
        {
            foreach (DungeonGridCell room in m_allRooms)
            {
                if (room.m_gridPosition == currentCell.m_gridPosition) continue;
                randomCell.Add(room);
            }

        }
        else
        {
            foreach (DungeonGridCell hall in m_hallwayPoints)
            {
                if (hall.m_gridPosition == currentCell.m_gridPosition) continue;
                randomCell.Add(hall);
            }

        }
        return randomCell[Random.Range(0, randomCell.Count)].m_worldPos;
    }

    /// <summary>
    /// Calculates which cell the character is currently in
    /// </summary>

    public DungeonGridCell CurrentCell(Vector3 m_currentPos)
    {
        DungeonGridCell returnCell = new DungeonGridCell();
        int xPos = 0, yPos = 0;
        for (int x = 0; x < m_currentDungeonType.m_cellsInDungeon.x; x++)
        {
            if ((x + 1) * m_currentDungeonType.m_cellSize.x > m_currentPos.x)
            {
                xPos = x;
                break;
            }
        }
        for (int y = 0; y < m_currentDungeonType.m_cellsInDungeon.y; y++)
        {
            if ((y + 1) * m_currentDungeonType.m_cellSize.y > m_currentPos.y)
            {
                yPos = y;
                break;
            }
        }
        bool inRoom = false;
        foreach (DungeonGridCell currentRoom in m_allRooms)
        {
            if (currentRoom.m_gridPosition.x == xPos)
            {
                if (currentRoom.m_gridPosition.y == yPos)
                {
                    returnCell = currentRoom;
                    inRoom = true;
                }
            }
        }
        if (!inRoom)
        {
            foreach (DungeonGridCell currentHallway in m_hallwayPoints)
            {
                if (currentHallway.m_gridPosition.x == xPos)
                {
                    if (currentHallway.m_gridPosition.y == yPos)
                    {
                        returnCell = currentHallway;

                    }
                }
            }
        }

        return returnCell;
    }

    /// <summary>
    /// Called through an event in the TurnBased manager, when the cycle of agents is complete, and the player is given control again
    /// This determines whether an enemy should spawn, using a random chance to determine whether it should actually spawn
    /// If an enemy should spawn, it then uses another random to determine which Ai should spawn from the list of enemies in the dungeon theme scriptable object
    /// </summary>
    public void SpawnNewEnemy()
    {
        if (AIManager.Instance.m_currentAiOnScene < AIManager.Instance.m_maxAiOnScene)
        {

            float randomAi = Random.Range(0f, 1f);
            if (randomAi < m_dungeonTheme.m_chanceOfEnemySpawn)
            {
                AIManager.Instance.m_currentAiOnScene++;

                DungeonGridCell spawnRoom = m_allRooms[Random.Range(0, m_allRooms.Count)];
                List<Vector2> possiblePos = new List<Vector2>();
                foreach (Vector2 possibleSpot in spawnRoom.m_floorTiles)
                {
                    if (!Physics2D.Raycast(possibleSpot, Vector3.forward, 100, m_spawnMask))
                    {
                        possiblePos.Add(possibleSpot);
                    }
                }
                Vector2 spawnPos = possiblePos[Random.Range(0, possiblePos.Count)];
                spawnPos += new Vector2(.5f, .5f);

                randomAi = Random.Range(0f, 1f);

                foreach (AiStruct ai in m_currentEnemyTypesInDungeon)
                {
                    if (randomAi < ai.m_aiRarity)
                    {
                        AIController newAi = m_pooler.NewObject(m_aiShell, spawnPos, Quaternion.identity).GetComponent<AIController>();
                        newAi.InitializeAi(ai.m_entityType);
                        m_turnSystem.NewAgent(newAi.GetComponent<TurnBasedAgent>());
                        AIManager.Instance.AddAiEntity(newAi.gameObject);
                        return;
                    }
                }
            }
        }
    }

    #endregion

    public void AssignEntityToRoom(int p_roomInt, GameObject p_entity)
    {

        m_allRooms[p_roomInt].AddEntityToRoom(p_entity);

    }
    public void RemoveEntityFromRoom(int p_roomInt, GameObject p_entity)
    {

        m_allRooms[p_roomInt].m_entitiesInRoom.Remove(p_entity);

    }
}

[System.Serializable]
public class DungeonGridCell
{
    public enum CellType { Wall, Room, Hallway, HallwayOneWay, Hazard, None }
    public CellType m_currentCellType;

    public int m_roomIndex;

    /// <summary>
    /// Maybe use this to check for a path before all the tiles are drawn.<br/>
    /// </summary>
    public int m_cellMultiplier;
    public Vector2Int m_gridPosition;
    public Vector2 m_worldPos;
    public Vector2 m_roomSize;

    public List<ConnectionPoint> m_connectionPoints;
    public List<Vector2Int> m_connectedTo;

    public List<Vector2> m_floorTiles;
    public List<GameObject> m_entitiesInRoom = new List<GameObject>();

    public void ChangeCellType(CellType p_newCellType)
    {
        m_currentCellType = p_newCellType;
    }
    public void AddConnectionPoint(Vector3Int p_position, ConnectionPoint.ConnectionType p_connectType)
    {
        m_connectionPoints.Add(new ConnectionPoint(p_position, p_connectType));
    }
    public void SetGridPosition(Vector2Int p_gridPos)
    {
        m_gridPosition = p_gridPos;
    }

    public bool IsWithinCell(Vector3 p_worldPos)
    {
        if (p_worldPos.x > m_worldPos.x - m_roomSize.x / 2 && p_worldPos.x < m_worldPos.x + m_roomSize.x / 2
            && p_worldPos.y > m_worldPos.y - m_roomSize.y / 2 && p_worldPos.y < m_worldPos.y + m_roomSize.y / 2)
        {
            //Debug.DrawLine(new Vector3(m_worldPos.x - m_roomSize.x / 2, m_worldPos.y - m_roomSize.y / 2, 0), new Vector3(m_worldPos.x + m_roomSize.x / 2, m_worldPos.y + m_roomSize.y / 2, 0), Color.blue, 5f);
            return true;
        }
        return false;
    }

    public void AddEntityToRoom(GameObject p_newEntity)
    {
        if (!m_entitiesInRoom.Contains(p_newEntity))
        {
            m_entitiesInRoom.Add(p_newEntity);
        }
    }
    public void RemoveEntityFromRoom(GameObject p_removeEntity)
    {
        if (m_entitiesInRoom.Contains(p_removeEntity))
        {
            m_entitiesInRoom.Remove(p_removeEntity);
        }
    }
}

[System.Serializable]
public struct ConnectionPoint
{
    public enum ConnectionType { Up, Down, Left, Right, Node }
    public Vector3Int m_connectionPos;
    public ConnectionType currentConnectionType;
    public ConnectionPoint(Vector3Int p_pos, ConnectionType p_connectType)
    {
        m_connectionPos = p_pos;
        currentConnectionType = p_connectType;
    }
}

[System.Serializable]
public struct DungeonRoom
{
    public Vector2 m_roomOrigin;
    public Vector2 m_roomDimensions;
}