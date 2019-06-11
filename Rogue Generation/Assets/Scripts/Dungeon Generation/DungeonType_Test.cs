using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Dungeon Test", menuName = "DungeonGen/DungeonTest", order = 0)]
public class DungeonType_Test : DungeonType_Base
{

    [Header("Testing Variables")]
    public Tile roomTile;
    public Tile roomExitTile, hallwayTile, connectionTile, wallTile;
    [Range(0, 1)]
    public float m_minEmptyPercent, m_maxEmptyPercent;


    public override List<DungeonGridCell> CreateDungeon(Tilemap p_wallTiles, Tilemap p_floorTiles, DungeonGenerator p_gen, DungeonNavigation p_dungeonNav)
    {

        ///Create th cell grid
        Vector2Int dungeonSize = new Vector2Int(m_cellsInDungeon.x * m_cellSize.x, m_cellsInDungeon.y * m_cellSize.y);

        DungeonGridCell[,] allCells = new DungeonGridCell[m_cellsInDungeon.x, m_cellsInDungeon.y];



        int numOfRooms = Random.Range(m_minRooms, m_maxRooms);

        List<int> roomIndexs = new List<int>();
        List<int> emptyIndexs = new List<int>();

        List<DungeonGridCell> roomCells = new List<DungeonGridCell>();

        int cellCount = m_cellsInDungeon.x * m_cellsInDungeon.y;
        for (int z = 0; z < cellCount; z++)
        {
            roomIndexs.Add(z);
        }


        //Create the number of rooms
        for (int i = 0; i < cellCount - numOfRooms; i++)
        {
            int currentCellIndex = Random.Range(0, roomIndexs.Count);
            emptyIndexs.Add(roomIndexs[currentCellIndex]);
            roomIndexs.RemoveAt(currentCellIndex);

        }

        //The hallways
        int currentCorridors = Random.Range((int)((float)emptyIndexs.Count * m_minEmptyPercent), (int)((float)emptyIndexs.Count * m_maxEmptyPercent));

        for (int c = 0; c < cellCount - numOfRooms; c++)
        {
            if (c >= currentCorridors)
            {
                int currentCellIndex = Random.Range(0, emptyIndexs.Count);
                emptyIndexs.RemoveAt(currentCellIndex);
            }

        }



        int currentIndex = 0;
        for (int x = 0; x < m_cellsInDungeon.x; x++)
        {
            for (int y = 0; y < m_cellsInDungeon.y; y++)
            {
                allCells[x, y] = new DungeonGridCell();
                allCells[x, y].m_connectionPoints = new List<ConnectionPoint>();
                allCells[x, y].m_connectedTo = new List<Vector2Int>();


                //Create the rooms
                if (roomIndexs.Contains(currentIndex))
                {
                    allCells[x, y].ChangeCellType(DungeonGridCell.CellType.Room);
                    
                }
                else if (emptyIndexs.Contains(currentIndex))
                {
                    allCells[x, y].ChangeCellType(DungeonGridCell.CellType.None);
                }

                //The connection points
                else
                {
                    allCells[x, y].ChangeCellType(DungeonGridCell.CellType.Hallway);
                    Vector2Int bounds = new Vector2Int(m_cellSize.x - m_cellBoarder, m_cellSize.y - m_cellBoarder);
                    Vector3Int newPos = new Vector3Int(Random.Range(2, bounds.x) + (x * m_cellSize.x), Random.Range(2, bounds.y) + (y * m_cellSize.y), 0);
                    allCells[x, y].AddConnectionPoint(newPos, ConnectionPoint.ConnectionType.Node);

                }


                currentIndex++;
            }
        }




        //Draw the boarder
        for (int x = 0; x < dungeonSize.x; x++)
        {
            for (int y = 0; y < dungeonSize.y; y++)
            {
                p_wallTiles.SetTile(new Vector3Int(x, y, 0), wallTile);
            }
        }

        //Actually draw the tiles
        for (int x = 0; x < m_cellsInDungeon.x; x++)
        {
            for (int y = 0; y < m_cellsInDungeon.y; y++)
            {
                allCells[x, y].SetGridPosition(new Vector2Int(x, y));
                switch (allCells[x, y].m_currentCellType)
                {
                    case DungeonGridCell.CellType.Room:
                        
                        roomCells.Add(CreateRoom(p_wallTiles, p_floorTiles, new Vector3Int(x * m_cellSize.x, y * m_cellSize.y, 0), allCells[x, y], allCells));
                        break;

                    case DungeonGridCell.CellType.HallwayOneWay:

                        break;


                    case DungeonGridCell.CellType.Hallway:
                        CreateCorridor(p_wallTiles, p_floorTiles, Vector3Int.zero, allCells[x, y].m_connectionPoints[0].m_connectionPos, allCells[x, y]);
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

                p_gen.m_allRooms.Add(allCells[x, y]);
                foreach (ConnectionPoint connectedPoint in allCells[x, y].m_connectionPoints)
                    RoomConnections(allCells, allCells[x, y], p_wallTiles, p_floorTiles, connectedPoint);
            }
        }

        
        



        p_dungeonNav.m_gridWorldSize = new Vector2(m_cellSize.x * m_cellsInDungeon.x, m_cellSize.y * m_cellsInDungeon.y);
        p_dungeonNav.m_gridOrigin = p_dungeonNav.m_gridWorldSize / 2;
        p_dungeonNav.CreateGrid();
        return roomCells;

    }

    public override void CreateCorridor(Tilemap p_wallTiles, Tilemap p_floorTiles, Vector3Int p_startPos, Vector3Int p_endPos, DungeonGridCell p_currentCell)
    {

        p_floorTiles.SetTile(p_endPos, connectionTile);
        p_wallTiles.SetTile(p_endPos, null);
    }


    void CreateHallway(Tilemap p_wallTiles,Tilemap p_floorTiles, Vector3Int p_startPos, Vector3Int p_endPos)
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

            p_floorTiles.SetTile(currentPos, hallwayTile);
            p_wallTiles.SetTile(currentPos, null);



        }


    }



    public override DungeonGridCell CreateRoom(Tilemap p_wallTiles, Tilemap p_floorTiles, Vector3Int p_roomPosition, DungeonGridCell p_currentCell, DungeonGridCell[,] p_allCells)
    {

        Vector2Int bounds = new Vector2Int(m_cellSize.x - m_cellBoarder, m_cellSize.y - m_cellBoarder);

        Vector2Int roomSize = new Vector2Int(Random.Range(m_minRoomSize.x, m_maxRoomSize.x), Random.Range(m_minRoomSize.y, m_maxRoomSize.y));

        Vector2Int randomStart = new Vector2Int(Random.Range(m_cellBoarder, bounds.x - roomSize.x), Random.Range(m_cellBoarder, bounds.y - roomSize.y));


        p_currentCell.m_worldPos = new Vector3(randomStart.x + p_roomPosition.x +.5f, randomStart.y + p_roomPosition.y + .5f);
        
        


        for (int x = randomStart.x; x < roomSize.x + randomStart.x; x++)
        {
            for (int y = randomStart.y; y < roomSize.y + randomStart.y; y++)
            {
                p_floorTiles.SetTile(new Vector3Int(x + p_roomPosition.x, y + p_roomPosition.y, 0), roomTile);
                p_wallTiles.SetTile(new Vector3Int(x + p_roomPosition.x, y + p_roomPosition.y, 0), null);

            }
        }

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

                    p_floorTiles.SetTile(connectionPos, roomExitTile);
                    p_wallTiles.SetTile(connectionPos, null);
                    p_currentCell.AddConnectionPoint(connectionPos, connectType);
                }
            }
        }

        return p_currentCell;
    }
    void RoomConnections(DungeonGridCell[,] p_allCells, DungeonGridCell p_currentCell, Tilemap p_wallTiles, Tilemap p_floorTiles,  ConnectionPoint p_currentConnectionPoint)
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
                        CreateHallway(p_wallTiles, p_floorTiles, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos);
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
                        CreateHallway(p_wallTiles, p_floorTiles, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos);
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
                        CreateHallway(p_wallTiles, p_floorTiles, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos);
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
                        CreateHallway(p_wallTiles, p_floorTiles, p_currentConnectionPoint.m_connectionPos, neighbour.m_connectionPos);
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
                            CreateHallway(p_wallTiles, p_floorTiles, p_currentConnectionPoint.m_connectionPos, neighbourCell.m_connectionPoints[0].m_connectionPos);
                        }
                    }

                }
            }

        }





    }



}
