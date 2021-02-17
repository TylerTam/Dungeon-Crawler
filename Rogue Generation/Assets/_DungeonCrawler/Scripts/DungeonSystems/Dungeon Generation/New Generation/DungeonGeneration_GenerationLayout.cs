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
        List<DungeonCellGridRow> allCells = new List<DungeonCellGridRow>(cellGridRow);


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
            allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_isRoom = true;
            p_currentRoomAmount++;


        }


        randomGridOrder = RandomCellList(cellGridRow);

        ///RNG the connection points
        p_currentRoomAmount = 0;
        foreach (CellGridData cell in randomGridOrder)
        {
            if (p_currentRoomAmount >= m_connectionAmounts)
            {
                break;
            }
            if (allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_isRoom) continue;
            CreateConnectionPoint(ref floorLayout, ref allCells, cell, p_cellSize);
            p_currentRoomAmount++;
        }
        ///Generate the room types
        ///



        List<DungeonGeneration_RoomLayout> randomizedRoomList;
        List<CellGridData> allRooms = new List<CellGridData>();
        randomizedRoomList = RandomizeRoom(p_rooms);

        for (int i = 0; i < cellsWithRooms.Count; i++)
        {
            CellGridData cell = cellsWithRooms[i];
            DungeonGeneration_RoomLayout tempRoom = GetCellBasedRoom(p_rooms, allCells, cell);
            allCells[cell.m_gridIndex.x].m_gridColumn[cell.m_gridIndex.y].m_roomLayout = tempRoom;
            UpdateSurroundingCells(ref floorLayout, ref allCells, tempRoom, cell, p_cellSize);

            Vector2Int bounds = new Vector2Int(p_cellSize.x - tempRoom.m_roomGridData.Count, p_cellSize.y - tempRoom.m_roomGridData[0].m_roomRowData.Count);
            Vector2Int randomOffset = new Vector2Int(Random.Range(0, bounds.x), Random.Range(0, bounds.y));

            Vector2Int worldPos = new Vector2Int(cell.m_gridIndex.x * p_cellSize.x + randomOffset.x + tempRoom.m_roomGridData.Count/2,
                cell.m_gridIndex.y * p_cellSize.y +  randomOffset.y + tempRoom.m_roomGridData[0].m_roomRowData.Count/2); 

            allRooms.Add(cell);
            if (tempRoom != null)
            {
                tempRoom.UpdateFloorLayoutWithRoom(ref floorLayout, worldPos, ref cell);
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
                            CreateHallway(ref floorLayout, cell.m_eastConnectionPoint, currentConnected.m_westConnectionPoint);
                        }
                    }
                    else if (cell.m_gridIndex.x > currentConnected.m_gridIndex.x)
                    {
                        if (currentConnected.m_eastConnectionPoint != Vector2.zero && cell.m_westConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_westConnectionPoint, currentConnected.m_eastConnectionPoint);
                        }
                    }
                    else if (cell.m_gridIndex.y < currentConnected.m_gridIndex.y)
                    {
                        if (currentConnected.m_northConnectionPoint != Vector2.zero && cell.m_southConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_southConnectionPoint, currentConnected.m_northConnectionPoint);
                        }
                    }
                    else if (cell.m_gridIndex.y > currentConnected.m_gridIndex.y)
                    {
                        if (cell.m_northConnectionPoint != Vector2.zero && currentConnected.m_southConnectionPoint != Vector2.zero)
                        {
                            CreateHallway(ref floorLayout, cell.m_northConnectionPoint, currentConnected.m_southConnectionPoint);
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

        foreach (DungeonGeneration_RoomLayout room in p_allRooms)
        {
            if (p_cellData.m_gridIndex == new Vector2Int(0, 0))
            {
                if (!room.m_southExit && !room.m_eastExit) continue;
            }
            else if (p_cellData.m_gridIndex == new Vector2Int(0, p_allCells[0].m_gridColumn.Count - 1))
            {
                if (!room.m_northExit && !room.m_westExit) continue;
            }
            else if (p_cellData.m_gridIndex == new Vector2Int(p_allCells.Count - 1, 0))
            {
                if (!room.m_southExit && !room.m_eastExit) continue;
            }
            else if (p_cellData.m_gridIndex == new Vector2Int(p_allCells.Count - 1, p_allCells[0].m_gridColumn.Count - 1))
            {
                if (!room.m_northExit && !room.m_eastExit) continue;
            }

            bool canAddRoom = true;
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

                        if (x == -1 && y == 0)
                        {

                            if (!room.m_westExit)
                            {
                                canAddRoom = false;
                                break;
                            }

                        }
                        if (x == 1 && y == 0)
                        {
                            if (!room.m_eastExit)
                            {
                                canAddRoom = false;
                                break;
                            }
                        }
                        if (x == 0 && y == -1)
                        {
                            if (!room.m_northExit)
                            {
                                canAddRoom = false;
                                break;
                            }
                        }
                        if (x == 0 && y == 1)
                        {
                            if (!room.m_southExit)
                            {
                                canAddRoom = false;
                                break;
                            }
                        }

                        //Debug.Log("Index: " + new Vector2(x, y) + " | Room: " + p_cellData.m_gridIndex + " | Room Layout Name:" + room.m_roomLayoutName + " |  North: " + room.m_northExit + " |  South: " + room.m_southExit + " |  East: " + room.m_eastExit + " |  West: " + room.m_westExit);
                    }

                }
                if (!canAddRoom)
                {
                    break;
                }
            }
            if (canAddRoom)
            {
                returnList.Add(room);
            }

        }

        if (returnList.Count > 0)
        {
            return RandomizeRoom(returnList)[0];
        }
        else return p_allRooms[Random.Range(0, p_allRooms.Count)];
    }

    private void UpdateSurroundingCells(ref int[,] p_floorLayout, ref List<DungeonCellGridRow> p_allCells, DungeonGeneration_RoomLayout p_roomLayout, CellGridData p_placedCell, Vector2Int p_cellSize)
    {
        if (p_roomLayout.m_northExit)
        {
            if (p_placedCell.m_gridIndex.y - 1 >= 0)
            {
                if (!p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1].m_isRoom &&
                !p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1].m_isConnectionPoint)
                {
                    p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1].m_isConnectionPoint = true;

                    CreateConnectionPoint(ref p_floorLayout, ref p_allCells, p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y - 1], p_cellSize);
                }
            }
        }

        if (p_roomLayout.m_southExit)
        {
            if (p_placedCell.m_gridIndex.y + 1 < p_allCells[0].m_gridColumn.Count)
            {
                if (!p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y + 1].m_isRoom &&
                !p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y + 1].m_isConnectionPoint)
                {
                    CreateConnectionPoint(ref p_floorLayout, ref p_allCells, p_allCells[p_placedCell.m_gridIndex.x].m_gridColumn[p_placedCell.m_gridIndex.y + 1], p_cellSize);
                    //Debug.Log("New Connection Point: " + new Vector2(worldPos.x, -worldPos.y));

                }
            }
        }

        if (p_roomLayout.m_eastExit)
        {
            if (p_placedCell.m_gridIndex.x + 1 < p_allCells.Count)
            {
                if (!p_allCells[p_placedCell.m_gridIndex.x + 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isRoom &&
                    !p_allCells[p_placedCell.m_gridIndex.x + 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isConnectionPoint)
                {
                    CreateConnectionPoint(ref p_floorLayout, ref p_allCells, p_allCells[p_placedCell.m_gridIndex.x + 1].m_gridColumn[p_placedCell.m_gridIndex.y], p_cellSize);
                    //Debug.Log("New Connection Point: " + new Vector2(worldPos.x, -worldPos.y));
                }
            }
        }

        if (p_roomLayout.m_westExit)
        {
            if (p_placedCell.m_gridIndex.x - 1 >= 0)
            {
                if (!p_allCells[p_placedCell.m_gridIndex.x - 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isRoom &&
                !p_allCells[p_placedCell.m_gridIndex.x - 1].m_gridColumn[p_placedCell.m_gridIndex.y].m_isConnectionPoint)
                {
                    CreateConnectionPoint(ref p_floorLayout, ref p_allCells, p_allCells[p_placedCell.m_gridIndex.x - 1].m_gridColumn[p_placedCell.m_gridIndex.y], p_cellSize);
                    //Debug.Log("New Connection Point: " + new Vector2(worldPos.x, -worldPos.y));
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

                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add (newCell.m_gridIndex);

                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y - 1].m_connectedCells.Add (currentCell.m_gridIndex);
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
                        if (!currentCell.m_connectedCells.Contains (newCell.m_gridIndex))
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
                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add (newCell.m_gridIndex);
                            p_allCells[p_checkData.m_gridIndex.x + 1].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add (currentCell.m_gridIndex);
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
                            p_allCells[p_checkData.m_gridIndex.x].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add (newCell.m_gridIndex);
                            p_allCells[p_checkData.m_gridIndex.x - 1].m_gridColumn[p_checkData.m_gridIndex.y].m_connectedCells.Add (currentCell.m_gridIndex);
                        }
                    }
                }

            }
        }

    }

    private void CreateConnectionPoint(ref int[,] p_floorLayout, ref List<DungeonCellGridRow> p_allCells, CellGridData p_cellData, Vector2Int p_cellSize)
    {
        ///Getting some padding
        Vector2Int bounds = new Vector2Int(p_cellSize.x - 3, p_cellSize.x - 3);
        Vector2Int worldPos = new Vector2Int(p_cellData.m_gridIndex.x * p_cellSize.x + (p_cellSize.x / 2) /*+ Random.Range(-bounds.x, bounds.x)*/, 
            -((p_cellData.m_gridIndex.y) * p_cellSize.y + (p_cellSize.y / 2) /*+ Random.Range(-bounds.y, bounds.y)*/));
        p_floorLayout[worldPos.x, -worldPos.y] = 1;

        p_allCells[p_cellData.m_gridIndex.x].m_gridColumn[p_cellData.m_gridIndex.y].m_northConnectionPoint =
        p_allCells[p_cellData.m_gridIndex.x].m_gridColumn[p_cellData.m_gridIndex.y].m_southConnectionPoint =
        p_allCells[p_cellData.m_gridIndex.x].m_gridColumn[p_cellData.m_gridIndex.y].m_eastConnectionPoint =
        p_allCells[p_cellData.m_gridIndex.x].m_gridColumn[p_cellData.m_gridIndex.y].m_westConnectionPoint = new Vector2Int(worldPos.x, Mathf.Abs(worldPos.y));


        p_allCells[p_cellData.m_gridIndex.x].m_gridColumn[p_cellData.m_gridIndex.y].m_isConnectionPoint = true;
    }

    private void CreateHallway(ref int[,] p_floorLayout, Vector2Int p_startPos, Vector2Int p_endPos)
    {
        int disX = (p_endPos.x - p_startPos.x);
        int disY = (p_endPos.y - p_startPos.y);

        bool canTurn = (disX > disY) ? (disX != 0) : (disY != 0);
        int dirX = (disX != 0) ? (int)Mathf.Sign(disX) : 0;
        int dirY = (disY != 0) ? (int)Mathf.Sign(disY) : 0;

        bool isTurning = false;

        int curX = p_startPos.x;
        int curY = p_startPos.y;
        if (disX >= disY)
        {
            for (int x = 0; x < Mathf.Abs(disX) + 1; x++)
            {
                curX = p_startPos.x + x * dirX;
                if (isTurning)
                {
                    for (int y = 0; y < Mathf.Abs(disY) + 1; y++)
                    {
                        curY = p_startPos.y + y * dirY;
                        p_floorLayout[curX, curY] = 1;
                    }
                    isTurning = false;
                    canTurn = false;
                }
                p_floorLayout[curX, curY] = 1;
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
                        p_floorLayout[curX, curY] = 1;
                    }
                    isTurning = false;
                    canTurn = false;
                }

                p_floorLayout[curX, curY] = 1;
                if (canTurn)
                {
                    if (curY > 2)
                    {
                        Debug.Log("CurY: " + curY + " | DisY: " + disY + " | disX: " + disX );
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
