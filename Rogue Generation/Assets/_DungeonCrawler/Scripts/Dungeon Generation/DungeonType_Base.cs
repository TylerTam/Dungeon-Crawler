using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class DungeonType_Base : ScriptableObject
{
    public Vector2Int m_dungeonMapBounds;
    public int m_minRooms, m_maxRooms;
    public Vector2Int m_minRoomSize, m_maxRoomSize;
    public int m_cellBoarder = 2;
    public Vector2Int m_cellsInDungeon;
    public Vector2Int m_cellSize;
    public int m_hallwayTurnAount = 3;





    public abstract List<DungeonGridCell> CreateDungeon(DungeonManager p_gen, DungeonTheme p_dungeonTheme, DungeonNavigation p_dungeonNav);

    public abstract DungeonGridCell CreateRoom(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_roomPosition, DungeonGridCell p_currentCell, DungeonGridCell[,] p_allCells, ref int[,] p_dungeonGrid);
    public abstract void CreateCorridor(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_startPos, Vector3Int p_endPos, DungeonGridCell p_currentCell, ref int[,] p_dungeonGrid);

    public abstract void RoomConnections(DungeonManager p_gen, DungeonTheme p_dungeonTheme, DungeonGridCell[,] p_allCells, DungeonGridCell p_currentCell, ConnectionPoint p_currentConnectionPoint, ref int[,] p_dungeonGrid);

    public abstract void CreateHallway(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_startPos, Vector3Int p_endPos, ref int[,] p_dungeonGrid);
}
