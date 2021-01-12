using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Dungeon Test", menuName = "DungeonGen/DungeonTest", order = 0)]
public class DungeonType_Test : DungeonType_Base
{

    [Range(0, 1)]
    public float m_minEmptyPercent, m_maxEmptyPercent;


    public override List<DungeonGridCell> CreateDungeon(DungeonManager p_gen, DungeonTheme p_dungeonTheme, DungeonNavigation p_dungeonNav)
    {
        p_gen.m_allCells = new DungeonGridCell[m_cellsInDungeon.x, m_cellsInDungeon.y];

        ///Generates a random amount of room cells
        #region Room Cell Generation
        int numOfRooms = Random.Range(m_minRooms, m_maxRooms);

        List<int> roomCells = new List<int>();
        List<int> emptyCells = new List<int>();


        List<DungeonGridCell> returningRoomCells = new List<DungeonGridCell>();
        int cellCount = m_cellsInDungeon.x * m_cellsInDungeon.y;
        for (int z = 0; z < cellCount; z++)
        {
            roomCells.Add(z);
        }


        //Create the number of rooms
        for (int i = 0; i < cellCount - numOfRooms; i++)
        {
            int currentCellIndex = Random.Range(0, roomCells.Count);
            emptyCells.Add(roomCells[currentCellIndex]);
            roomCells.RemoveAt(currentCellIndex);

        }
        #endregion


        #region Hallway Cell Generation
        //The hallways
        int currentCorridors = Random.Range((int)((float)emptyCells.Count * m_minEmptyPercent), (int)((float)emptyCells.Count * m_maxEmptyPercent));

        for (int c = 0; c < cellCount - numOfRooms; c++)
        {
            if (c >= currentCorridors)
            {
                int currentCellIndex = Random.Range(0, emptyCells.Count);
                emptyCells.RemoveAt(currentCellIndex);
            }

        }

        #endregion


        #region Creates the connections between cells
        int currentIndex = 0;
        for (int x = 0; x < m_cellsInDungeon.x; x++)
        {
            for (int y = 0; y < m_cellsInDungeon.y; y++)
            {
                p_gen.m_allCells[x, y] = new DungeonGridCell();
                p_gen.m_allCells[x, y].m_connectionPoints = new List<ConnectionPoint>();
                p_gen.m_allCells[x, y].m_connectedTo = new List<Vector2Int>();


                //Create the rooms
                if (roomCells.Contains(currentIndex))
                {
                    p_gen.m_allCells[x, y].ChangeCellType(DungeonGridCell.CellType.Room);

                }
                else if (emptyCells.Contains(currentIndex))
                {
                    p_gen.m_allCells[x, y].ChangeCellType(DungeonGridCell.CellType.None);
                }

                //The connection points
                else
                {
                    p_gen.m_allCells[x, y].ChangeCellType(DungeonGridCell.CellType.Hallway);
                    Vector2Int bounds = new Vector2Int(m_cellSize.x - m_cellBoarder, m_cellSize.y - m_cellBoarder);
                    Vector3Int newPos = new Vector3Int(Random.Range(2, bounds.x) + (x * m_cellSize.x), Random.Range(2, bounds.y) + (y * m_cellSize.y), 0);

                    p_gen.m_allCells[x, y].AddConnectionPoint(newPos, ConnectionPoint.ConnectionType.Node);
                }


                currentIndex++;
            }
        }

        #endregion

        int[,] dungeonGrid = new int[m_cellsInDungeon.x * m_cellSize.x, m_cellsInDungeon.y * m_cellSize.y];
        Debug.Log("Grid Size | x: " + dungeonGrid.GetLength(0) + " | y: " + dungeonGrid.GetLength(1));
        //Draw the boarder
        /*for (int x = -m_dungeonMapBounds.x; x < m_cellsInDungeon.x * m_cellSize.x + m_dungeonMapBounds.x; x++)
        {
            for (int y = -m_dungeonMapBounds.y; y < m_cellsInDungeon.y * m_cellSize.y + m_dungeonMapBounds.y; y++)
            {
                if(x > dungeonGrid.GetLength(0))
                {
                    Debug.LogError("X Too big: " + x);
                }
                else if (y > dungeonGrid.GetLength(1))
                {
                    Debug.LogError("Y Too big: " + y);
                }
                dungeonGrid[x, y] = 0;

            }
        }*/


        //Get the items structs
        p_dungeonTheme.FixRates();
        List<ItemStruct> itemsInDungeon = p_dungeonTheme.ItemsInDungeon();

        //Actually draw the tiles
        for (int x = 0; x < m_cellsInDungeon.x; x++)
        {
            for (int y = 0; y < m_cellsInDungeon.y; y++)
            {
                p_gen.m_allCells[x, y].SetGridPosition(new Vector2Int(x, y));
                switch (p_gen.m_allCells[x, y].m_currentCellType)
                {
                    case DungeonGridCell.CellType.Room:

                        DungeonGridCell roomCell = CreateRoom(p_gen, p_dungeonTheme, new Vector3Int(x * m_cellSize.x, y * m_cellSize.y, 0), p_gen.m_allCells[x, y], p_gen.m_allCells, ref dungeonGrid);
                        PopulateRoomWithItems(p_gen, p_dungeonTheme, roomCell.m_floorTiles, itemsInDungeon);
                        returningRoomCells.Add(roomCell);
                        break;

                    case DungeonGridCell.CellType.HallwayOneWay:
                        break;

                    case DungeonGridCell.CellType.Hallway:
                        CreateCorridor(p_gen, p_dungeonTheme, Vector3Int.zero, p_gen.m_allCells[x, y].m_connectionPoints[0].m_connectionPos, p_gen.m_allCells[x, y], ref dungeonGrid);
                        break;

                    case DungeonGridCell.CellType.None:
                        break;


                }
            }
        }


        //Create the room connections
        for (int x = 0; x < m_cellsInDungeon.x; x++)
        {
            for (int y = 0; y < m_cellsInDungeon.y; y++)
            {

                p_gen.m_allRooms.Add(p_gen.m_allCells[x, y]);
                foreach (ConnectionPoint connectedPoint in p_gen.m_allCells[x, y].m_connectionPoints)
                {
                    RoomConnections(p_gen, p_dungeonTheme, p_gen.m_allCells, p_gen.m_allCells[x, y], connectedPoint, ref dungeonGrid);
                }
            }
        }



        for (int x = 0; x < dungeonGrid.GetLength(0); x++)
        {
            for (int y = 0; y < dungeonGrid.GetLength(1); y++)
            {
                ///Wall
                if (dungeonGrid[x, y] == 0)
                {
                    p_gen.m_wallTiles.SetTile(new Vector3Int(x, y, 0), p_dungeonTheme.m_wallTile);
                }

                ///Floor
                else if (dungeonGrid[x, y] == 1)
                {
                    p_gen.m_floorTiles.SetTile(new Vector3Int(x, y, 0), p_dungeonTheme.m_floorTile);
                    p_gen.m_miniMapTiles.SetTile(new Vector3Int(x, y, 0), p_gen.m_miniMapTile);
                }
            }
        }

        p_dungeonNav.m_gridWorldSize = new Vector2(m_cellSize.x * m_cellsInDungeon.x, m_cellSize.y * m_cellsInDungeon.y);
        p_dungeonNav.m_gridOrigin = p_dungeonNav.m_gridWorldSize / 2;
        p_dungeonNav.CreateGrid();
        return returningRoomCells;

    }

    public override void CreateCorridor(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_startPos, Vector3Int p_endPos, DungeonGridCell p_currentCell, ref int[,] p_dungeonGrid)
    {
        p_currentCell.m_worldPos = (Vector3)p_endPos + new Vector3(.5f, .5f, 0f);
        p_gen.m_hallwayPoints.Add(p_currentCell);
        p_dungeonGrid[p_endPos.x, p_endPos.y] = 1;
    }


    public override void CreateHallway(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_startPos, Vector3Int p_endPos, ref int[,] p_dungeonGrid)
    {


        bool travelX = (p_endPos.x - p_startPos.x > p_endPos.y - p_startPos.y) ? true : false;

        int amountOfTiles = Mathf.Abs(p_endPos.x - p_startPos.x) + Mathf.Abs(p_endPos.y - p_startPos.y);


        int xDir, yDir;
        if (p_endPos.x > p_startPos.x)
        {
            xDir = 1;
        }
        else if (p_endPos.x < p_startPos.x)
        {
            xDir = -1;
        }
        else
        {
            xDir = 0;
        }

        if (p_endPos.y > p_startPos.y)
        {
            yDir = 1;
        }
        else if (p_endPos.y < p_startPos.y)
        {
            yDir = -1;
        }
        else
        {
            yDir = 0;

        }

        Vector3Int currentPos = p_startPos;
        int currentTurnAmount = m_hallwayTurnAount;

        for (int t = 0; t < amountOfTiles; t++)
        {

            if (travelX)
            {

                currentPos = new Vector3Int(currentPos.x + 1 * xDir, currentPos.y, 0);

                if (currentPos.x == p_endPos.x)
                {
                    travelX = false;
                }
                else
                {
                    if (currentTurnAmount > 1)
                    {
                        float turnPercent = (float)t / (float)amountOfTiles;

                        if (Random.Range(0f, 1f) > 1 - turnPercent && currentPos.y != p_endPos.y)
                        {
                            currentTurnAmount -= 1;

                            travelX = false;
                        }
                    }
                }
            }
            else
            {
                currentPos = new Vector3Int(currentPos.x, currentPos.y + 1 * yDir, 0);
                if (currentPos.y == p_endPos.y)
                {
                    travelX = true;
                }
                else
                {
                    if (currentTurnAmount > 1)
                    {
                        float turnPercent = (float)t / (float)amountOfTiles;
                        if (Random.Range(0f, 1f) > 1 - turnPercent && currentPos.x != p_endPos.x)
                        {
                            currentTurnAmount -= 1;

                            travelX = true;
                        }
                    }
                }
            }

            p_dungeonGrid[currentPos.x, currentPos.y] = 1;
        }
    }



    public override DungeonGridCell CreateRoom(DungeonManager p_gen, DungeonTheme p_dungeonTheme, Vector3Int p_roomPosition, DungeonGridCell p_currentCell, DungeonGridCell[,] p_allCells, ref int[,] p_dungeonGrid)
    {

        Vector2Int bounds = new Vector2Int(m_cellSize.x - m_cellBoarder, m_cellSize.y - m_cellBoarder);

        Vector2Int roomSize = new Vector2Int(Random.Range(m_minRoomSize.x, m_maxRoomSize.x), Random.Range(m_minRoomSize.y, m_maxRoomSize.y));

        Vector2Int randomStart = new Vector2Int(Random.Range(m_cellBoarder, bounds.x - roomSize.x), Random.Range(m_cellBoarder, bounds.y - roomSize.y));


        p_currentCell.m_worldPos = new Vector3(randomStart.x + p_roomPosition.x + .5f, randomStart.y + p_roomPosition.y + .5f);


        List<Vector2> floorTiles = new List<Vector2>();

        for (int x = randomStart.x; x < roomSize.x + randomStart.x; x++)
        {
            for (int y = randomStart.y; y < roomSize.y + randomStart.y; y++)
            {


                p_dungeonGrid[x + p_roomPosition.x, y + p_roomPosition.y] = 1;
                floorTiles.Add(new Vector2(x + p_roomPosition.x, y + p_roomPosition.y));

            }
        }

        p_currentCell.m_floorTiles = floorTiles;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0 || x == -1 && y != 0 || x == 1 && y != 0 || x != 0 && y == -1 || x != 0 && y == 1) continue;
                if (x + p_currentCell.m_gridPosition.x < 0 || x + p_currentCell.m_gridPosition.x > m_cellsInDungeon.x - 1) continue;
                if (y + p_currentCell.m_gridPosition.y < 0 || y + p_currentCell.m_gridPosition.y > m_cellsInDungeon.y - 1) continue;
                if (p_allCells[x + p_currentCell.m_gridPosition.x, y + p_currentCell.m_gridPosition.y].m_connectedTo.Contains(p_currentCell.m_gridPosition)) continue;
                if (p_allCells[x + p_currentCell.m_gridPosition.x, y + p_currentCell.m_gridPosition.y].m_currentCellType == DungeonGridCell.CellType.Hallway || p_allCells[x + p_currentCell.m_gridPosition.x, y + p_currentCell.m_gridPosition.y].m_currentCellType == DungeonGridCell.CellType.Room)
                {
                    Vector3Int connectionPos = new Vector3Int();
                    ConnectionPoint.ConnectionType connectType = ConnectionPoint.ConnectionType.Node;
                    if (y > 0)
                    {
                        connectionPos = new Vector3Int(Random.Range(0, roomSize.x) + p_roomPosition.x + randomStart.x, roomSize.y + p_roomPosition.y + randomStart.y, 0);
                        connectType = ConnectionPoint.ConnectionType.Up;
                    }

                    else if (y < 0)
                    {
                        connectionPos = new Vector3Int(Random.Range(0, roomSize.x) + p_roomPosition.x + randomStart.x, p_roomPosition.y + randomStart.y - 1, 0);
                        connectType = ConnectionPoint.ConnectionType.Down;
                    }

                    else if (x > 0)
                    {
                        connectionPos = new Vector3Int(roomSize.x + p_roomPosition.x + randomStart.x, Random.Range(0, roomSize.y) + p_roomPosition.y + randomStart.y, 0);
                        connectType = ConnectionPoint.ConnectionType.Right;
                    }
                    else if (x < 0)
                    {
                        connectionPos = new Vector3Int(p_roomPosition.x + randomStart.x - 1, Random.Range(0, roomSize.y) + p_roomPosition.y + randomStart.y, 0);
                        connectType = ConnectionPoint.ConnectionType.Left;
                    }

                    p_dungeonGrid[connectionPos.x, connectionPos.y] = 1;
                    p_currentCell.AddConnectionPoint(connectionPos, connectType);
                }
            }
        }

        return p_currentCell;
    }

    public override void RoomConnections(DungeonManager p_gen, DungeonTheme p_dungeonTheme, DungeonGridCell[,] p_allCells, DungeonGridCell p_currentCell, ConnectionPoint p_currentConnectionPoint, ref int[,] p_dungeonGrid)
    {

        Vector2Int neighbourIndex = new Vector2Int();

        if (p_currentConnectionPoint.currentConnectionType != ConnectionPoint.ConnectionType.Node)
        {
            if (p_currentConnectionPoint.currentConnectionType == ConnectionPoint.ConnectionType.Left)
            {
                neighbourIndex = new Vector2Int(p_currentCell.m_gridPosition.x - 1, p_currentCell.m_gridPosition.y);
                if (p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectedTo.Contains(p_currentCell.m_gridPosition)) return;

                foreach (ConnectionPoint neighbour in p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectionPoints)
                {
                    if (neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Right || neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Node)
                    {
                        CreateHallway(p_gen, p_dungeonTheme, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos, ref p_dungeonGrid);
                    }
                }




            }

            else if (p_currentConnectionPoint.currentConnectionType == ConnectionPoint.ConnectionType.Right)
            {
                neighbourIndex = new Vector2Int(p_currentCell.m_gridPosition.x + 1, p_currentCell.m_gridPosition.y);
                if (p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectedTo.Contains(p_currentCell.m_gridPosition)) return;

                foreach (ConnectionPoint neighbour in p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectionPoints)
                {
                    if (neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Left || neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Node)
                    {
                        CreateHallway(p_gen, p_dungeonTheme, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos, ref p_dungeonGrid);
                    }
                }


            }

            else if (p_currentConnectionPoint.currentConnectionType == ConnectionPoint.ConnectionType.Up)
            {
                neighbourIndex = new Vector2Int(p_currentCell.m_gridPosition.x, p_currentCell.m_gridPosition.y + 1);
                if (p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectedTo.Contains(p_currentCell.m_gridPosition)) return;

                foreach (ConnectionPoint neighbour in p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectionPoints)
                {
                    if (neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Down || neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Node)
                    {
                        CreateHallway(p_gen, p_dungeonTheme, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos, ref p_dungeonGrid);
                    }
                }


            }

            else if (p_currentConnectionPoint.currentConnectionType == ConnectionPoint.ConnectionType.Down)
            {
                neighbourIndex = new Vector2Int(p_currentCell.m_gridPosition.x, p_currentCell.m_gridPosition.y - 1);
                if (p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectedTo.Contains(p_currentCell.m_gridPosition)) return;

                foreach (ConnectionPoint neighbour in p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectionPoints)
                {
                    if (neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Up || neighbour.currentConnectionType == ConnectionPoint.ConnectionType.Node)
                    {
                        CreateHallway(p_gen, p_dungeonTheme, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos, ref p_dungeonGrid);
                    }
                }


            }


            p_allCells[neighbourIndex.x, neighbourIndex.y].m_connectedTo.Add(p_currentCell.m_gridPosition);
            p_currentCell.m_connectedTo.Add(p_allCells[neighbourIndex.x, neighbourIndex.y].m_gridPosition);
        }
        else
        {
            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    if (x == 0 && y == 0 || x == -1 && y != 0 || x == 1 && y != 0 || x != 0 && y == -1 || x != 0 && y == 1) continue;
                    if (x + p_currentCell.m_gridPosition.x < 0 || x + p_currentCell.m_gridPosition.x > m_cellsInDungeon.x - 1) continue;
                    if (y + p_currentCell.m_gridPosition.y < 0 || y + p_currentCell.m_gridPosition.y > m_cellsInDungeon.y - 1) continue;



                    DungeonGridCell neighbourCell = p_allCells[p_currentCell.m_gridPosition.x + x, p_currentCell.m_gridPosition.y + y];
                    if (neighbourCell.m_currentCellType == DungeonGridCell.CellType.Hallway)
                    {
                        if (!neighbourCell.m_connectedTo.Contains(p_currentCell.m_gridPosition))
                        {
                            neighbourCell.m_connectedTo.Add(p_currentCell.m_gridPosition);
                            p_currentCell.m_connectedTo.Add(neighbourCell.m_gridPosition);
                            CreateHallway(p_gen, p_dungeonTheme, p_currentConnectionPoint.m_connectionPos, neighbourCell.m_connectionPoints[0].m_connectionPos, ref p_dungeonGrid);
                        }
                    }

                }
            }

        }





    }

    void PopulateRoomWithItems(DungeonManager p_gen, DungeonTheme p_dungeonTheme, List<Vector2> p_floorTilesLocations, List<ItemStruct> p_itemList)
    {
        int numberOfItems = Random.Range(p_dungeonTheme.m_minItemsPerRoom, p_dungeonTheme.m_maxItemsPerRoom);
        List<Vector2> itemPlacements = new List<Vector2>();
        for (int i = 0; i < numberOfItems; i++)
        {
            int randomPlace = Random.Range(0, p_floorTilesLocations.Count);
            itemPlacements.Add(p_floorTilesLocations[randomPlace]);
            p_floorTilesLocations.RemoveAt(randomPlace);
        }

        foreach (Vector2 itemPlace in itemPlacements)
        {
            float randomItem = Random.Range(0f, 1f);
            float currentRate = 0;
            int itemSpawnIndex = 0;
            foreach (ItemStruct item in p_itemList)
            {

                if (randomItem < item.m_itemRarity + currentRate)
                {

                    itemSpawnIndex = p_itemList.IndexOf(item);
                    break;
                }
                else
                {
                    currentRate += item.m_itemRarity;
                }
            }

            Vector3 itemPos = new Vector3(itemPlace.x + .5f, itemPlace.y + .5f, 0);
            Item_MapObjectBase newItem = ObjectPooler.instance.NewObject(p_itemList[itemSpawnIndex].m_itemType.m_itemGameWorldPrefab.gameObject, itemPos, Quaternion.identity).GetComponent<Item_MapObjectBase>();
            p_gen.m_itemsOnFloor.Add(newItem.gameObject);
            newItem.AssignObjectType(p_itemList[itemSpawnIndex].m_itemType);
        }

    }

}
