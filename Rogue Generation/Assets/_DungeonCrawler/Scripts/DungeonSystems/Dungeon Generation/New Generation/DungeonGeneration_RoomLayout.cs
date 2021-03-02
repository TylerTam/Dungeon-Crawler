using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Room Layout", menuName = "NewDungeonGen/New Dungeon Room Layout", order = 0)]
public class DungeonGeneration_RoomLayout : ScriptableObject
{
    public string m_roomLayoutName;
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
        public string m_data;
    }


    public bool CanPlaceRoom(int[,] p_dungeonGrid, Vector2Int p_position, Vector2Int p_gridIndex, List<DungeonCellGridRow> p_cellGridData)
    {
        RoomGridData data;

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

    /// <summary>
    /// Used for the connection point generation
    /// </summary>
    public RoomData UpdateFloorLayoutWithRoom(ref int[,] p_dungeonGrid, Vector2Int p_position, ref CellGridData p_refCellData)
    {
        RoomData newData = new RoomData();
        RoomGridData data;
        for (int x = 0; x < m_roomGridData.Count; x++)
        {
            for (int y = 0; y < m_roomGridData[x].m_roomRowData.Count; y++)
            {
                data = m_roomGridData[x].m_roomRowData[y];
                Vector2Int pos = new Vector2Int(p_position.x + data.m_gridPosition.x, p_position.y + data.m_gridPosition.y);
                if (pos.x < 0 || pos.x >= p_dungeonGrid.GetLength(0))
                {
                    Debug.Log("X Too Big");
                }
                if (pos.y < 0 || pos.y >= p_dungeonGrid.GetLength(1))
                {
                    Debug.Log("y Too Big");
                }
                p_dungeonGrid[pos.x, pos.y] = data.m_cellType;
            }
        }

        if (m_northExit)
        {
            p_refCellData.m_northConnectionPoint = p_position + m_northExitPos;
        }
        if (m_southExit)
        {
            p_refCellData.m_southConnectionPoint = p_position + m_southExitPos;
        }
        if (m_eastExit)
        {
            p_refCellData.m_eastConnectionPoint = p_position + m_eastExitPos;
        }
        if (m_westExit)
        {
            p_refCellData.m_westConnectionPoint = p_position + m_westExitPos;
        }

        return newData;
    }


    public RoomData UpdateFloorLayoutWithRoom(ref int[,] p_dungeonGrid, Vector2Int p_position, ref CellGridData p_refCellData, int p_roomIndex)
    {
        RoomData newData = new RoomData();
        newData.m_roomCenterWorldPos = p_position;
        newData.m_roomCellIndex = p_refCellData.m_gridIndex;
        RoomGridData data;

        newData.m_enemySpawnLocations = new List<Vector2Int>();
        newData.m_itemSpawnLocations = new List<Vector2Int>();
        newData.m_trapSpawnLocations = new List<Vector2Int>();
        for (int x = 0; x < m_roomGridData.Count; x++)
        {
            for (int y = 0; y < m_roomGridData[x].m_roomRowData.Count; y++)
            {
                data = m_roomGridData[x].m_roomRowData[y];
                Vector2Int pos = new Vector2Int(p_position.x + data.m_gridPosition.x, p_position.y + data.m_gridPosition.y);
                if (pos.x < 0 || pos.x >= p_dungeonGrid.GetLength(0))
                {
                    Debug.Log("X Too Big");
                }
                if (pos.y < 0 || pos.y >= p_dungeonGrid.GetLength(1))
                {
                    Debug.Log("y Too Big");
                }



                if (data.m_cellType == GlobalVariables.m_hazardCell)
                {
                    p_dungeonGrid[pos.x, pos.y] = GlobalVariables.m_hazardCell - p_roomIndex;
                }
                else if (data.m_cellType == GlobalVariables.m_trapSpawnCell)
                {
                    newData.m_trapSpawnLocations.Add(new Vector2Int(x, y));
                    p_dungeonGrid[pos.x, pos.y] = GlobalVariables.m_startingWalkable + p_roomIndex;
                }
                else if (data.m_cellType == GlobalVariables.m_itemSpawnCell)
                {
                    newData.m_itemSpawnLocations.Add(new Vector2Int(x, y));
                    p_dungeonGrid[pos.x, pos.y] = GlobalVariables.m_startingWalkable + p_roomIndex;
                }

                else if (data.m_cellType == GlobalVariables.m_unwalkableCell)
                {
                    p_dungeonGrid[pos.x, pos.y] = 0;
                }
                else if (data.m_cellType == GlobalVariables.m_startingWalkable)
                {
                    newData.m_enemySpawnLocations.Add(new Vector2Int(x, y));
                    p_dungeonGrid[pos.x, pos.y] = GlobalVariables.m_startingWalkable + p_roomIndex;
                }
            }
        }

        if (m_northExit)
        {
            p_refCellData.m_northConnectionPoint = p_position + m_northExitPos;
        }
        if (m_southExit)
        {
            p_refCellData.m_southConnectionPoint = p_position + m_southExitPos;
        }
        if (m_eastExit)
        {
            p_refCellData.m_eastConnectionPoint = p_position + m_eastExitPos;
        }
        if (m_westExit)
        {
            p_refCellData.m_westConnectionPoint = p_position + m_westExitPos;
        }

        return newData;
    }

}
