using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DungeonGenerator : MonoBehaviour
{


    public UnityEngine.Tilemaps.Tilemap m_wallTiles, m_floorTiles;
    public DungeonType_Base dungeonType;
    public List<DungeonType_Base.DungeonGridCell> m_allRooms;
    DungeonNavigation m_dungeonNav;
    DungeonNavigation_Agent m_gridTestAgent;


    bool mapGenerated = false;


    private void Awake()
    {
        m_dungeonNav = GetComponent<DungeonNavigation>();
        m_gridTestAgent = GetComponent<DungeonNavigation_Agent>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            mapGenerated = false;

        }
        if (!mapGenerated)
        {

            Coroutine eyy = StartCoroutine(CheckDungeonConnection());
            mapGenerated = true;
        }
    }


    IEnumerator CheckDungeonConnection()
    {
        bool mapSuccess = false;
        int mapAttempts = 0;

        while (true)
        {
            m_allRooms.Clear();
            m_wallTiles.ClearAllTiles();
            m_floorTiles.ClearAllTiles();
            m_allRooms = dungeonType.CreateDungeon(m_wallTiles, m_floorTiles, this, m_dungeonNav);

            yield return new WaitForEndOfFrame();

            m_dungeonNav.m_gridWorldSize = new Vector2(dungeonType.m_cellSize.x * dungeonType.m_cellsInDungeon.x, dungeonType.m_cellSize.y * dungeonType.m_cellsInDungeon.y);
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
            if (mapSuccess == false)
            {
                mapAttempts++;
                yield return new WaitForSeconds(.5f);
                

            }
        }


    }



}
