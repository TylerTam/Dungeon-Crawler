using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class DungeonType_Base : ScriptableObject
{
    public int m_minRooms, m_maxRooms;
    public Vector2Int m_minRoomSize, m_maxRoomSize;
    public int m_cellBoarder = 2;
    public Vector2Int m_cellsInDungeon;
    public Vector2Int m_cellSize;
    public int m_hallwayTurnAount = 3;



    

    public abstract List<DungeonGridCell> CreateDungeon(DungeonGenerator p_gen, DungeonTheme p_dungeonTheme, DungeonNavigation p_dungeonNav);

    public abstract DungeonGridCell CreateRoom(DungeonGenerator p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_roomPosition, DungeonGridCell p_currentCell, DungeonGridCell[,] p_allCells);
    public abstract void CreateCorridor(DungeonGenerator p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_startPos, Vector3Int p_endPos, DungeonGridCell p_currentCell);

    [System.Serializable]
    public struct DungeonGridCell
    {
        public enum CellType { Room, Hallway, HallwayOneWay, None }
        public Vector2Int m_gridPosition;
        public Vector2 m_worldPos;
        public CellType m_currentCellType;
        public List<ConnectionPoint> m_connectionPoints;
        public List<Vector2Int> m_connectedTo;

        public List<Vector2> m_floorTiles;

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
    }
    [System.Serializable]
    public struct ConnectionPoint {
        public enum ConnectionType{Up, Down, Left, Right, Node}
        public Vector3Int m_connectionPos;
        public ConnectionType currentConnectionType;
        public ConnectionPoint(Vector3Int p_pos, ConnectionType p_connectType){
            m_connectionPos = p_pos;
            currentConnectionType = p_connectType;
        }
    }


   
}
