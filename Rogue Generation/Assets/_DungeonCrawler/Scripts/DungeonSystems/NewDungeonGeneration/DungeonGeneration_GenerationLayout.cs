using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Generation Layout", menuName = "NewDungeonGen/New Dungeon Generation Layout", order = 0)]
public class DungeonGeneration_GenerationLayout : ScriptableObject
{

    public FloorData GenerateArray(List<DungeonGeneration_RoomLayout> p_rooms, List<DungeonGeneration_RoomLayout> p_connections, Vector2Int p_cellAmount, Vector2Int p_cellSize, int p_roomAmount, int p_connectionAmount)
    {
        FloorData currentFloorData = new FloorData();
        currentFloorData.m_allRooms = new List<RoomData>();

        int[,] floorLayout = new int[p_cellAmount.x * p_cellSize.y, p_cellAmount.y * p_cellSize.y];

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
        List<DungeonCellGridRow> allCells = new List<DungeonCellGridRow>(cellGridRow);


        ///RNG the room placements
        List<CellGridData> cellsWithRooms = new List<CellGridData>();
        List<CellGridData> randomGridOrder = RandomCellList(cellGridRow);
        int p_currentRoomAmount = 0;


        foreach (CellGridData cell in randomGridOrder)
        {

            if (p_currentRoomAmount >= p_roomAmount)
            {
                break;
            }
            cellsWithRooms.Add(cell);
            allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_isRoom = true;
            p_currentRoomAmount++;


        }


        randomGridOrder = RandomCellList(cellGridRow);

        ///RNG the connection points
        p_currentRoomAmount = 0;
        foreach (CellGridData cell in randomGridOrder)
        {
            if (p_currentRoomAmount >= p_connectionAmount)
            {
                break;
            }
            if (allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_isRoom) continue;
            //CreateConnectionPoint(ref floorLayout, ref allCells, cell, p_cellSize);
            allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_isConnectionPoint = true;
            p_currentRoomAmount++;
        }
        ///Generate the room types
        ///



        List<CellGridData> allRooms = new List<CellGridData>();

        List<RoomData> newRoomData = new List<RoomData>();

        for (int i = 0; i < cellsWithRooms.Count; i++)
        {
            CellGridData cell = cellsWithRooms[i];
            DungeonGeneration_RoomLayout tempRoom = GetCellBasedRoom(p_rooms, allCells, cell);
            allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_roomLayout = tempRoom;
            UpdateSurroundingCells(ref allCells, tempRoom, cell, p_rooms);



            allRooms.Add(cell);

        }


        for (int i = 0; i < allRooms.Count; i++)
        {
            CellGridData cell = allRooms[i];
            Vector2Int bounds = new Vector2Int(p_cellSize.x - cell.m_roomLayout.m_roomGridData.Count, p_cellSize.y - cell.m_roomLayout.m_roomGridData[0].m_roomRowData.Count);
            Vector2Int randomOffset = new Vector2Int(Random.Range(0, bounds.x), Random.Range(0, bounds.y));

            Vector2Int worldPos = new Vector2Int(cell.m_gridIndex.x * p_cellSize.x + randomOffset.x + cell.m_roomLayout.m_roomGridData.Count / 2,
                cell.m_gridIndex.y * p_cellSize.y + randomOffset.y + cell.m_roomLayout.m_roomGridData[0].m_roomRowData.Count / 2);

            RoomData newRoom = new RoomData();
            if (cell.m_roomLayout != null)
            {
                newRoom = cell.m_roomLayout.UpdateFloorLayoutWithRoom(ref floorLayout, worldPos, ref cell, currentFloorData.m_allRooms.Count);
            }
            currentFloorData.m_allRooms.Add(newRoom);
        }



        ///Determine the hallwayConnection Point Layouts

        bool north, south, east, west;

        for (int x = 0; x < allCells.Count; x++)
        {
            for (int y = 0; y < allCells[x].m_gridColumn.Count; y++)
            {
                if (!allCells[x].m_gridColumn[y].m_isConnectionPoint) continue;
                north = south = east = west = false;

                for (int xx = -1; xx < 2; xx++)
                {
                    for (int yy = -1; yy < 2; yy++)
                    {
                        if (x + xx < 0 || x + xx >= allCells.Count) continue;
                        if (y + yy < 0 || y + yy >= allCells[x].m_gridColumn.Count) continue;
                        if (xx == -1 && yy != 0 || xx == 1 && yy != 0) continue;
                        if (xx != 0 && yy == -1 || xx != 0 && yy == 1) continue;

                        if (allCells[x + xx].m_gridColumn[y + yy].m_isConnectionPoint || allCells[x + xx].m_gridColumn[y + yy].m_isRoom)
                        {

                            if (xx == 1)
                            {
                                east = true;
                            }
                            else if (xx == -1)
                            {
                                west = true;
                            }
                            else if (yy == -1)
                            {
                                north = true;
                            }
                            else if (yy == 1)
                            {
                                south = true;
                            }

                        }
                    }
                }


                List<DungeonGeneration_RoomLayout> possibleRooms = new List<DungeonGeneration_RoomLayout>();

                foreach (DungeonGeneration_RoomLayout connection in p_connections)
                {
                    if (connection.m_northExit == north && connection.m_southExit == south && connection.m_eastExit == east && connection.m_westExit == west)
                    {
                        possibleRooms.Add(connection);
                    }
                }
                allCells[x].m_gridColumn[y].m_connectionLayout = possibleRooms[Random.Range(0, possibleRooms.Count)];
                CellGridData currentCell = allCells[x].m_gridColumn[y];
                if (allCells[x].m_gridColumn[y].m_connectionLayout != null)
                {
                    Vector2Int bounds = new Vector2Int(p_cellSize.x - currentCell.m_connectionLayout.m_roomGridData.Count, p_cellSize.y - currentCell.m_connectionLayout.m_roomGridData[0].m_roomRowData.Count);
                    Vector2Int randomOffset = new Vector2Int(Random.Range(0, bounds.x), Random.Range(0, bounds.y));

                    Vector2Int worldPos = new Vector2Int(currentCell.m_gridIndex.x * p_cellSize.x + randomOffset.x + currentCell.m_connectionLayout.m_roomGridData.Count / 2,
                        currentCell.m_gridIndex.y * p_cellSize.y + randomOffset.y + currentCell.m_connectionLayout.m_roomGridData[0].m_roomRowData.Count / 2);
                    allCells[x].m_gridColumn[y].m_connectionLayout.UpdateFloorLayoutWithRoom(ref floorLayout, worldPos, ref currentCell);
                }
            }
        }




        ///Generate the paths
        ///

        #region Space Filling Code

        for (int i = 0; i < allCells.Count; i++)
        {
            for (int o = 0; o < allCells[0].m_gridColumn.Count; o++)
            {
                if (!allCells[i].m_gridColumn[o].m_isConnectionPoint && !allCells[i].m_gridColumn[o].m_isRoom) continue;
                Vector2Int cellIndex = allCells[i].m_gridColumn[o].m_gridIndex;
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {

                        if (cellIndex.x + x < 0 || cellIndex.x + x > allCells.Count - 1) continue;
                        if (cellIndex.y + y < 0 || cellIndex.y + y > allCells[0].m_gridColumn.Count - 1) continue;

                        if (x == 0 && y == 0 ||
                            x == -1 && y == -1 ||
                            x == 1 && y == -1 ||
                            x == -1 && y == 1 ||
                            x == 1 && y == 1) continue;

                        if (allCells[i].m_gridColumn[o].m_connectedCells.Contains(new Vector2Int(i + x, o + y)) ||
                            allCells[i + x].m_gridColumn[o + y].m_connectedCells.Contains(new Vector2Int(i, o))) continue;



                        if (allCells[i + x].m_gridColumn[o + y].m_isRoom || allCells[i + x].m_gridColumn[o + y].m_isConnectionPoint)
                        {
                            allCells[i].m_gridColumn[o].m_connectedCells.Add(new Vector2Int(i + x, o + y));
                            allCells[i + x].m_gridColumn[o + y].m_connectedCells.Add(new Vector2Int(i, o));
                        }
                    }
                }
            }
        }


        ///Check the rooms to see if they can do any final connections
        foreach (CellGridData data in allRooms)
        {
            GetFinalRoomCheck(ref floorLayout, ref allCells, data);
        }


        ///Draw the hallways
        foreach (DungeonCellGridRow row in allCells)
        {
            foreach (CellGridData cell in row.m_gridColumn)
            {
                foreach (Vector2Int connect in cell.m_connectedCells)
                {
                    CellGridData currentConnected = allCells[connect.x].m_gridColumn[connect.y];

                    if (currentConnected.m_hallwayToCell.Contains(cell.m_gridIndex)) continue;

                    if (cell.m_gridIndex.x < currentConnected.m_gridIndex.x)
                    {
                        if (cell.m_eastConnectionPoint != Vector2.zero && currentConnected.m_westConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_eastConnectionPoint, currentConnected.m_westConnectionPoint, true);
                        }
                    }
                    else if (cell.m_gridIndex.x > currentConnected.m_gridIndex.x)
                    {
                        if (currentConnected.m_eastConnectionPoint != Vector2.zero && cell.m_westConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_westConnectionPoint, currentConnected.m_eastConnectionPoint, true);
                        }
                    }
                    else if (cell.m_gridIndex.y < currentConnected.m_gridIndex.y)
                    {
                        if (currentConnected.m_northConnectionPoint != Vector2.zero && cell.m_southConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_southConnectionPoint, currentConnected.m_northConnectionPoint, false);
                        }
                    }
                    else if (cell.m_gridIndex.y > currentConnected.m_gridIndex.y)
                    {
                        if (cell.m_northConnectionPoint != Vector2.zero && currentConnected.m_southConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_northConnectionPoint, currentConnected.m_southConnectionPoint, false);
                        }
                    }
                    allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_hallwayToCell.Add(currentConnected.m_gridIndex);
                    allCells[currentConnected.m_gridIndex.x].m_gridColumn[currentConnected.m_gridIndex.y].m_hallwayToCell.Add(cell.m_gridIndex);

                }
            }
        }

        #endregion

        currentFloorData.m_floorLayout = floorLayout;
        currentFloorData.m_cellGrid = allCells;
        return currentFloorData;
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

    private DungeonGeneration_RoomLayout GetCellBasedRoom(List<DungeonGeneration_RoomLayout> p_allRooms, List<DungeonCellGridRow> p_allCells, CellGridData p_cellData)
    {
        List<DungeonGeneration_RoomLayout> returnList = new List<DungeonGeneration_RoomLayout>();

        bool needNorth, needSouth, needEast, needWest;
        needNorth = needSouth = needEast = needWest = false;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;
                if (x == -1 && y == -1 || x == -1 && y == 1 || x == 1 && y == 1 || x == 1 && y == -1) continue;
                if (p_cellData.m_gridIndex.x + x < 0 || p_cellData.m_gridIndex.x + x > p_allCells.Count - 1) continue;
                if (p_cellData.m_gridIndex.y + y < 0 || p_cellData.m_gridIndex.y + y > p_allCells[0].m_gridColumn.Count - 1) continue;

                Vector2Int currentIndex = new Vector2Int(p_cellData.m_gridIndex.x + x, p_cellData.m_gridIndex.y + y);
                CellGridData currentCell = p_allCells[currentIndex.x].m_gridColumn[currentIndex.y];
                if (currentCell.m_isRoom || currentCell.m_isConnectionPoint)
                {
                    if (x == 1)
                    {
                        needEast = true;
                    }
                    else if (x == -1)
                    {
                        needWest = true;
                    }
                    else if (y == 1)
                    {
                        needSouth = true;
                    }
                    else if (y == -1)
                    {
                        needNorth = true;
                    }

                }
            }
        }
        foreach (DungeonGeneration_RoomLayout room in p_allRooms)
        {
            bool canAddRoom = true;

            if (needNorth && !room.m_northExit) continue;

            if (needSouth && !room.m_southExit) continue;

            if (needEast && !room.m_eastExit) continue;

            if (needWest && !room.m_westExit) continue;

            if (p_cellData.m_gridIndex.x == 0 && room.m_westExit) continue;
            if (p_cellData.m_gridIndex.x == p_allCells.Count - 1 && room.m_eastExit) continue;

            if (p_cellData.m_gridIndex.y == 0 && room.m_northExit) continue;
            if (p_cellData.m_gridIndex.y == p_allCells[0].m_gridColumn.Count - 1 && room.m_southExit) continue;


            if (canAddRoom)
            {
                returnList.Add(room);
            }

        }

        if (returnList.Count > 0)
        {
            DungeonGeneration_RoomLayout returnRoom = RandomizeRoom(returnList)[0];
            return returnRoom;
        }
        else
        {
            Debug.Log("Cell Index: " + p_cellData.m_gridIndex + " | Return Randomized DEfault List");
            return p_allRooms[Random.Range(0, p_allRooms.Count)];
        }
    }

    private void UpdateSurroundingCells(ref List<DungeonCellGridRow> p_allCells, DungeonGeneration_RoomLayout p_roomLayout, CellGridData p_placedCell, List<DungeonGeneration_RoomLayout> p_allRooms)
    {
        CellGridData currentNewCell = null;
        if (p_roomLayout.m_northExit)
        {
            if (p_placedCell.m_gridIndex.y - 1 >= 0)
            {
                currentNewCell = p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1];

                if (!currentNewCell.m_isRoom && !currentNewCell.m_isConnectionPoint)
                {
                    p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1].m_isConnectionPoint = true;

                    AdjustSurroundingRooms(ref p_allCells, currentNewCell, p_allRooms);

                }
            }
        }

        if (p_roomLayout.m_southExit)
        {
            if (p_placedCell.m_gridIndex.y + 1 < p_allCells[0].m_gridColumn.Count)
            {
                currentNewCell = p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y + 1];
                if (!currentNewCell.m_isRoom && !currentNewCell.m_isConnectionPoint)
                {
                    p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y + 1].m_isConnectionPoint = true;
                    AdjustSurroundingRooms(ref p_allCells, currentNewCell, p_allRooms);

                }
            }
        }

        if (p_roomLayout.m_eastExit)
        {
            if (p_placedCell.m_gridIndex.x + 1 < p_allCells.Count)
            {
                currentNewCell = p_allCells[p_placedCell.m_gridIndex.x + 1].m_gridColumn[p_placedCell.m_gridIndex.y];
                if (!currentNewCell.m_isRoom &&
                    !currentNewCell.m_isConnectionPoint)
                {
                    p_allCells[p_placedCell.m_gridIndex.x + 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isConnectionPoint = true;
                    AdjustSurroundingRooms(ref p_allCells, currentNewCell, p_allRooms);
                }
            }
        }

        if (p_roomLayout.m_westExit)
        {
            if (p_placedCell.m_gridIndex.x - 1 >= 0)
            {
                currentNewCell = p_allCells[p_placedCell.m_gridIndex.x - 1].m_gridColumn[p_placedCell.m_gridIndex.y];
                if (!currentNewCell.m_isRoom &&
                !currentNewCell.m_isConnectionPoint)
                {
                    p_allCells[p_placedCell.m_gridIndex.x - 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isConnectionPoint = true;
                    AdjustSurroundingRooms(ref p_allCells, currentNewCell, p_allRooms);
                }
            }
        }
    }

    private void AdjustSurroundingRooms(ref List<DungeonCellGridRow> p_allCells, CellGridData p_currentCell, List<DungeonGeneration_RoomLayout> p_allRooms)
    {

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;
                if (p_currentCell.m_gridIndex.x + x >= p_allCells.Count || p_currentCell.m_gridIndex.x + x < 0) continue;
                if (p_currentCell.m_gridIndex.y + y >= p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn.Count || p_currentCell.m_gridIndex.y + y < 0) continue;

                if (x == 1 && y != 0 || x == -1 && y != 0) continue;
                if (x != 0 && y == 1 || x != 0 && y == -1) continue;

                CellGridData cell = p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn[p_currentCell.m_gridIndex.y + y];
                if (!cell.m_isRoom || cell.m_roomLayout == null) continue;
                if (x == -1)
                {
                    if (!cell.m_roomLayout.m_eastExit)
                    {
                        p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn[p_currentCell.m_gridIndex.y + y].m_roomLayout = GetCellBasedRoom(p_allRooms, p_allCells, cell);
                    }
                }
                else if (x == 1)
                {
                    if (!cell.m_roomLayout.m_westExit)
                    {
                        p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn[p_currentCell.m_gridIndex.y + y].m_roomLayout = GetCellBasedRoom(p_allRooms, p_allCells, cell);
                    }
                }
                else if (y == -1)
                {
                    if (!cell.m_roomLayout.m_southExit)
                    {
                        p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn[p_currentCell.m_gridIndex.y + y].m_roomLayout = GetCellBasedRoom(p_allRooms, p_allCells, cell);
                    }
                }
                else if (y == 1)
                {
                    if (!cell.m_roomLayout.m_northExit)
                    {
                        p_allCells[p_currentCell.m_gridIndex.x + x].m_gridColumn[p_currentCell.m_gridIndex.y + y].m_roomLayout = GetCellBasedRoom(p_allRooms, p_allCells, cell);
                    }
                }
            }
        }
    }

    private void GetFinalRoomCheck(ref int[,] p_floorLayout, ref List<DungeonCellGridRow> p_allCells, CellGridData p_checkData)
    {
        CellGridData currentCell = p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y];
        if (p_checkData.m_roomLayout.m_northExit)
        {
            if (p_checkData.m_gridIndex.y - 1 > 0 && p_checkData.m_gridIndex.y - 1 < p_allCells[0].m_gridColumn.Count)
            {
                CellGridData newCell = p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y - 1];
                bool connect = true;
                if (currentCell.m_isRoom || newCell.m_isConnectionPoint)
                {
                    if (newCell.m_isRoom)
                    {
                        connect = newCell.m_roomLayout.m_southExit;
                    }
                    if (connect)
                    {
                        if (!currentCell.m_connectedCells.Contains(newCell.m_gridIndex))
                        {

                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add(newCell.m_gridIndex);

                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y - 1].m_connectedCells.Add(currentCell.m_gridIndex);
                        }
                    }
                }

            }
        }

        if (p_checkData.m_roomLayout.m_southExit)
        {
            if (p_checkData.m_gridIndex.y + 1 > 0 && p_checkData.m_gridIndex.y + 1 < p_allCells[0].m_gridColumn.Count)
            {
                CellGridData newCell = p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y + 1];
                bool connect = true;
                if (newCell.m_isRoom || newCell.m_isConnectionPoint)
                {
                    if (newCell.m_isRoom)
                    {
                        connect = newCell.m_roomLayout.m_northExit;
                    }

                    if (connect)
                    {
                        if (!currentCell.m_connectedCells.Contains(newCell.m_gridIndex))
                        {
                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add
                                (newCell.m_gridIndex);

                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y + 1].m_connectedCells.Add
                                (currentCell.m_gridIndex);
                        }
                    }


                }

            }
        }

        if (p_checkData.m_roomLayout.m_eastExit)
        {
            if (p_checkData.m_gridIndex.x + 1 > 0 && p_checkData.m_gridIndex.x + 1 < p_allCells.Count)
            {
                CellGridData newCell = p_allCells[p_checkData.m_gridIndex.x + 1].m_gridColumn[p_checkData.m_gridIndex.y];
                bool connect = true;
                if (newCell.m_isRoom || newCell.m_isConnectionPoint)
                {
                    if (newCell.m_isRoom)
                    {
                        connect = newCell.m_roomLayout.m_westExit;
                    }

                    if (connect)
                    {
                        if (!currentCell.m_connectedCells.Contains
                        (newCell.m_gridIndex))
                        {
                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add(newCell.m_gridIndex);
                            p_allCells[p_checkData.m_gridIndex.x + 1].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add(currentCell.m_gridIndex);
                        }
                    }
                }

            }
        }

        if (p_checkData.m_roomLayout.m_westExit)
        {
            if (p_checkData.m_gridIndex.x - 1 > 0 && p_checkData.m_gridIndex.x - 1 < p_allCells.Count)
            {

                CellGridData newCell = p_allCells[p_checkData.m_gridIndex.x - 1].m_gridColumn[p_checkData.m_gridIndex.y];
                bool connect = true;
                if (p_allCells[p_checkData.m_gridIndex.x - 1].m_gridColumn[p_checkData.m_gridIndex.y].m_isRoom ||
                    p_allCells[p_checkData.m_gridIndex.x - 1].m_gridColumn[p_checkData.m_gridIndex.y].m_isConnectionPoint)
                {
                    if (newCell.m_isRoom)
                    {
                        connect = newCell.m_roomLayout.m_eastExit;
                    }

                    if (connect)
                    {
                        if (!newCell.m_connectedCells.Contains
                        (newCell.m_gridIndex))
                        {
                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add(newCell.m_gridIndex);
                            p_allCells[p_checkData.m_gridIndex.x - 1].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add(currentCell.m_gridIndex);
                        }
                    }
                }

            }
        }

    }

    private void CreateHallway(ref int[,] p_floorLayout, Vector2Int p_startPos, Vector2Int p_endPos, bool p_horizontal)
    {
        int disX = (p_endPos.x - p_startPos.x);
        int disY = (p_endPos.y - p_startPos.y);

        bool canTurn = (disX > disY) ? (disX != 0) : (disY != 0);
        int dirX = (disX != 0) ? (int)Mathf.Sign(disX) : 0;
        int dirY = (disY != 0) ? (int)Mathf.Sign(disY) : 0;

        bool isTurning = false;

        int curX = p_startPos.x;
        int curY = p_startPos.y;
        if (p_horizontal)
        {
            for (int x = 0; x < Mathf.Abs(disX) + 1; x++)
            {
                curX = p_startPos.x + x * dirX;
                if (isTurning)
                {
                    for (int y = 0; y < Mathf.Abs(disY) + 1; y++)
                    {
                        curY = p_startPos.y + y * dirY;
                        if (p_floorLayout[curX, curY] < GlobalVariables.m_startingWalkable)
                        {
                            p_floorLayout[curX, curY] = 1;
                        }
                    }
                    isTurning = false;
                    canTurn = false;
                }
                if (p_floorLayout[curX, curY] < GlobalVariables.m_startingWalkable)
                {
                    p_floorLayout[curX, curY] = 1;
                }
                if (canTurn)
                {
                    if (curX > 2)
                    {

                        if (Random.Range(0f, ((float)x / (float)dirX)) > .5f)
                        {

                            isTurning = true;

                        }
                        if (x + 2 == Mathf.Abs(disX))
                        {
                            isTurning = true;
                        }

                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < Mathf.Abs(disY) + 1; y++)
            {
                curY = p_startPos.y + y * dirY;
                if (isTurning)
                {
                    for (int x = 0; x < Mathf.Abs(disX) + 1; x++)
                    {
                        curX = p_startPos.x + x * dirX;
                        if (p_floorLayout[curX, curY] < GlobalVariables.m_startingWalkable)
                        {
                            p_floorLayout[curX, curY] = 1;
                        }
                    }
                    isTurning = false;
                    canTurn = false;
                }

                if (p_floorLayout[curX, curY] < GlobalVariables.m_startingWalkable)
                {
                    p_floorLayout[curX, curY] = 1;
                }
                if (canTurn)
                {
                    if (curY > 2)
                    {

                        if (Random.Range(0f, ((float)y / (float)dirY)) > .5f)
                        {

                            isTurning = true;

                        }

                        if (y + 2 == Mathf.Abs(disY))
                        {
                            isTurning = true;
                        }
                    }
                }
            }
        }

    }
}
