using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Generation Layout", menuName = "NewDungeonGen/New Dungeon Generation Layout", order = 0)]
public class DungeonGeneration_GenerationLayout : ScriptableObject
{
    public int m_roomAmount;
    public int m_connectionAmounts;

    public FloorData GenerateArray(List<DungeonGeneration_RoomLayout> p_rooms, Vector2Int p_cellAmount, Vector2Int p_cellSize)
    {
        int[,] floorLayout = new int[p_cellAmount.x * p_cellSize.y, p_cellAmount.y * p_cellSize.y];
        FloorData currentFloorData = new FloorData();

        List<DungeonCellGridRow> cellGridRow = new List<DungeonCellGridRow>();
        for (int x = 0; x < p_cellAmount.x; x++)
        {
            DungeonCellGridRow newRow = new DungeonCellGridRow();
            newRow.m_gridColumn = new List<CellGridData>();
            for (int y = 0; y < p_cellAmount.y; y++)
            {
                CellGridData newCellData = new CellGridData();
                newCellData.m_gridIndex = new Vector2Int(x, y);
                newRow.m_gridColumn.Add(newCellData);
            }
            cellGridRow.Add(newRow);
        }

        ///RNG the room placements
        List<CellGridData> cellsWithRooms = new List<CellGridData>();
        List<CellGridData> randomGridOrder = RandomCellList(cellGridRow);
        int p_currentRoomAmount = 0;


        foreach (CellGridData cell in randomGridOrder)
        {
            
            if (p_currentRoomAmount >= m_roomAmount)
            {
                break;
            }
            cellsWithRooms.Add(cell);
            p_currentRoomAmount++;


        }


        randomGridOrder = RandomCellList(cellGridRow);

        ///RNG the connection points
        p_currentRoomAmount = 0;
        foreach(CellGridData cell in randomGridOrder)
        {
            if(p_currentRoomAmount >= m_connectionAmounts)
            {
                break;
            }
            if (cell.m_isRoom) continue;
            cell.m_isConnectionPoint = true;
            Vector2Int worldPos = new Vector2Int(cell.m_gridIndex.x * p_cellSize.x + (p_cellSize.x / 2), cell.m_gridIndex.y * p_cellSize.y + (p_cellSize.y / 2));
            floorLayout[worldPos.x, worldPos.y] = 1;
            p_currentRoomAmount++;
        }
        ///Generate the room types
        ///

        List<DungeonGeneration_RoomLayout> randomizedRoomList;
        randomizedRoomList = RandomizeRoom(p_rooms);
        //TODO: Randomize the room placements, and then go over the rooms placed. If they dont meet more than [2] criteria, re-roll the room type
        //If a room has less than 2 connections, search around it for the closest, non-attached room.

        foreach (CellGridData cell in cellsWithRooms)
        {
            Vector2Int worldPos = new Vector2Int(cell.m_gridIndex.x * p_cellSize.x + (p_cellSize.x / 2), cell.m_gridIndex.y * p_cellSize.y + (p_cellSize.y / 2));

            for (int i = 0; i < cellsWithRooms.Count; i++)
            {
                for (int o = 0; o < randomizedRoomList.Count; o++)
                {
                    if (randomizedRoomList[o].CanPlaceRoom(floorLayout, worldPos, new Vector2Int(cell.m_gridIndex.x, cell.m_gridIndex.y), cellGridRow))
                    {
                        p_currentRoomAmount += 1;
                        randomizedRoomList[o].UpdateDungeonIndex(ref floorLayout, worldPos);
                        cell.m_isRoom = true;
                        break;
                    }
                }
            }
            randomizedRoomList = RandomizeRoom(p_rooms);
        }

        ///Generate the paths
        ///

        currentFloorData.m_floorLayout = floorLayout;
        return currentFloorData;
    }

    private List<DungeonGeneration_RoomLayout> RandomizeRoom(List<DungeonGeneration_RoomLayout> p_rooms)
    {
        List<DungeonGeneration_RoomLayout> tempList = new List<DungeonGeneration_RoomLayout>(p_rooms);
        DungeonGeneration_RoomLayout tempRoom;

        for (int i = 0; i < tempList.Count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            tempRoom = tempList[i];
            tempList[i] = tempList[randomIndex];
            tempList[randomIndex] = tempRoom;
        }
        return tempList;
    }

    private List<CellGridData> RandomCellList(List<DungeonCellGridRow> p_randomCells)
    {
        List<CellGridData> tempList = new List<CellGridData>();
        foreach (DungeonCellGridRow row in p_randomCells)
        {
            foreach (CellGridData cell in row.m_gridColumn)
            {
                tempList.Add(cell);
            }
        }
        CellGridData tempRoom = tempList[0];

        for (int i = 0; i < tempList.Count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            tempRoom = tempList[i];
            tempList[i] = tempList[randomIndex];
            tempList[randomIndex] = tempRoom;
        }
        return tempList;
    }


}
