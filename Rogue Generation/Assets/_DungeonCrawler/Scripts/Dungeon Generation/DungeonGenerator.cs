using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    public UnityEngine.Tilemaps.Tilemap m_wallTiles, m_floorTiles, m_miniMapTiles;
    public UnityEngine.Tilemaps.Tile m_miniMapTile;
    public DungeonTheme m_dungeonTheme;
    public List<DungeonType_Base.DungeonGridCell> m_allRooms;
    DungeonNavigation m_dungeonNav;
    DungeonNavigation_Agent m_gridTestAgent;

    public List<GameObject> m_itemsOnFloor;



    [HideInInspector]
    public int m_dungeonGenTypeIndex;
    bool mapGenerated = false;

    ObjectPooler objectPool;


    public List<ItemStruct> m_fixedRatios;

    public GameObject m_playerObject;
    Input_Base m_playerInput;
    public GameObject m_staircase;

    List<Vector2> m_occupiedSpaces;

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
        objectPool = ObjectPooler.instance;
        StartCoroutine(CheckDungeonConnection());
    }
    public void NewFloor()
    {
        //Coroutine CreateDungeon = StartCoroutine(CheckDungeonConnection());
        StartCoroutine(CheckDungeonConnection());
    }


    IEnumerator CheckDungeonConnection()
    {
        bool mapSuccess = false;

        while (!mapSuccess)
        {
            foreach (GameObject item in m_itemsOnFloor)
            {
                objectPool.ReturnToPool(item);
            }
            m_itemsOnFloor.Clear();

            m_allRooms.Clear();
            m_wallTiles.ClearAllTiles();
            m_floorTiles.ClearAllTiles();
            m_miniMapTiles.ClearAllTiles();
            m_allRooms = m_dungeonTheme.CreateNewFloor(this, m_dungeonNav);

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

    Vector2 RandomSpawnPosition(List<DungeonType_Base.DungeonGridCell> m_possibleRooms)
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

}
