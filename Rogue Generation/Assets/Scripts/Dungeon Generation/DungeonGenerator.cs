﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DungeonGenerator : MonoBehaviour
{


    public UnityEngine.Tilemaps.Tilemap m_wallTiles, m_floorTiles;
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

    private void Awake()
    {
        m_dungeonNav = GetComponent<DungeonNavigation>();
        m_gridTestAgent = GetComponent<DungeonNavigation_Agent>();
    }
    private void Start()
    {
        objectPool = ObjectPooler.instance;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            mapGenerated = false;
            

        }
        if (!mapGenerated)
        {

            Coroutine CreateDungeon = StartCoroutine(CheckDungeonConnection());
            mapGenerated = true;
        }
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


    }



}