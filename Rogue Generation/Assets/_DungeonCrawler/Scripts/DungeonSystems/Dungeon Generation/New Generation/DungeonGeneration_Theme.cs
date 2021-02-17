using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Dungeon Generation Theme", menuName = "NewDungeonGen/New Dungeon Generation Theme", order = 0)]
public class DungeonGeneration_Theme : ScriptableObject
{
    public Tile m_wallTile, m_floorTile;
    public List<DungeonGeneration_RoomLayout> m_roomTypesInDungeon;


    public void PaintDungeon(int[,] p_floorLayout, Tilemap p_wallTilemap, Tilemap p_floorTilemap)
    {
        for (int x = 0; x < p_floorLayout.GetLength(0); x++)
        {
            for (int y = 0; y < p_floorLayout.GetLength(1); y++)
            {
                if(p_floorLayout[x,y] == 0)
                {
                    p_wallTilemap.SetTile(new Vector3Int(x, -y, 0), m_wallTile);
                }
                else
                {
                    p_floorTilemap.SetTile(new Vector3Int(x, -y, 0), m_floorTile);
                }
            }
        }
    }
}
