using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Room Layout", menuName = "NewDungeonGen/New Dungeon Room Layout", order = 0)]
public class DungeonGeneration_RoomLayout : ScriptableObject
{
    public Vector2Int m_roomSize;
    public int m_rooWeight;

    public List<RoomGridRow> m_roomGridData;

    [Header("Room Exits")]
    public bool m_northExit;
    public Vector2Int m_northExitPos;

    public bool m_southExit;
    public Vector2Int m_southExitPos;

    public bool m_eastExit;
    public Vector2Int m_eastExitPos;

    public bool m_westExit;
    public Vector2Int m_westExitPos;



    [System.Serializable]
    public class RoomGridRow
    {
        public List<RoomGridData> m_roomRowData;
    }
    [System.Serializable]
    public class RoomGridData
    {
        public Vector2Int m_gridPosition;
        public int m_cellType;
        public bool m_canSpawnItem;
        public bool m_canSpawnEnemy;
        public string m_data;
    }


    public bool CanPlaceRoom(int[,] p_dungeonGrid, Vector2Int p_position, Vector2Int p_gridIndex, List<DungeonCellGridRow> p_cellGridData)
    {
        RoomGridData data;

        if (p_gridIndex == new Vector2Int(0, 0))
        {
            if (!m_southExit && !m_eastExit) return false;
        }
        else if (p_gridIndex == new Vector2Int(0, p_cellGridData[0].m_gridColumn.Count-1))
        {
            if (!m_northExit && !m_eastExit) return false;
        }
        else if (p_gridIndex == new Vector2Int(p_cellGridData.Count-1, 0))
        {
            if (!m_southExit && !m_westExit) return false;
        }
        else if (p_gridIndex == new Vector2Int(p_cellGridData.Count-1, p_cellGridData[0].m_gridColumn.Count-1))
        {
            if (!m_northExit && !m_westExit) return false;
        }

        for (int x = 0; x < m_roomGridData.Count; x++)
        {
            for (int y = 0; y < m_roomGridData[0].m_roomRowData.Count; y++)
            {
                data = m_roomGridData[x].m_roomRowData[y];
                if (p_position.x + data.m_gridPosition.x >= p_dungeonGrid.GetLength(0))
                {
                    Debug.Log("x larger: " + (p_position.x + data.m_gridPosition.x) + " | " + p_dungeonGrid.GetLength(0));
                    return false;
                }
                if (p_position.y + data.m_gridPosition.y >= p_dungeonGrid.GetLength(1))
                {
                    Debug.Log("y larger: " + (p_position.y + data.m_gridPosition.y) + " | " + p_dungeonGrid.GetLength(0));
                    return false;
                }
                if (p_dungeonGrid[p_position.x + data.m_gridPosition.x, p_position.y + data.m_gridPosition.y] == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public RoomData UpdateDungeonIndex(ref int[,] p_dungeonGrid, Vector2Int p_position)
    {
        RoomData newData = new RoomData();
        RoomGridData data;
        for (int x = 0; x < m_roomGridData.Count; x++)
        {
            for (int y = 0; y < m_roomGridData[x].m_roomRowData.Count; y++)
            {
                data = m_roomGridData[x].m_roomRowData[y];
                p_dungeonGrid[p_position.x + data.m_gridPosition.x, p_position.y + data.m_gridPosition.y] = data.m_cellType;
                if (data.m_canSpawnEnemy)
                {
                    newData.m_enemySpawnLocations.Add(new Vector2Int(x, y));
                }
            }
        }
        return newData;
    }
}
