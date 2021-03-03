using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Dungeon Generation Theme", menuName = "NewDungeonGen/New Dungeon Generation Theme", order = 0)]
public class DungeonGeneration_Theme : ScriptableObject
{
    public Tile m_wallTile, m_floorTile;
    public List<Tile> m_debugTiles;

    public List<FloorGenerationData> m_floorData;
    public List<DungeonGeneration_RoomLayout> m_connectionTypesInDungeon;
    [System.Serializable]
    public class FloorGenerationData
    {
        public int m_roomCount;
        public int m_connectPointCount;
        public Vector2Int m_cellAmount;
        public Vector2Int m_cellSize;
        public DungeonGeneration_GenerationLayout m_layoutType;
        public List<DungeonGeneration_RoomLayout> m_roomTypesInDungeon;


        [Tooltip("Ensure that the AI rarity add up to 1")]
        [Header("The AI that can appear on this floor")]
        public AIFloorData m_aiOnFloor;
    }


    public FloorData GenerateFloor(int p_currentFloor)
    {
        FloorGenerationData floorType = m_floorData[p_currentFloor];
        return floorType.m_layoutType.GenerateArray(floorType.m_roomTypesInDungeon, m_connectionTypesInDungeon, floorType.m_cellAmount, floorType.m_cellSize, floorType.m_roomCount, floorType.m_connectPointCount);
    }

    public void PaintDungeon(int[,] p_floorLayout, Tilemap p_wallTilemap, Tilemap p_floorTilemap)
    {
        for (int x = 0; x < p_floorLayout.GetLength(0); x++)
        {
            for (int y = 0; y < p_floorLayout.GetLength(1); y++)
            {
                if (p_floorLayout[x, y] == 0)
                {
                    p_wallTilemap.SetTile(new Vector3Int(x, -y - 1, 0), m_wallTile);
                }
                else if (p_floorLayout[x, y] >= GlobalVariables.m_startingWalkable)
                {
                    if (p_floorLayout[x, y] == GlobalVariables.m_startingWalkable)
                    {
                        p_floorTilemap.SetTile(new Vector3Int(x, -y - 1, 0), m_floorTile);
                    }
                    else
                    {
                        p_floorTilemap.SetTile(new Vector3Int(x, -y - 1, 0), m_debugTiles[p_floorLayout[x, y] - GlobalVariables.m_startingWalkable-1]);
                    }
                }
            }
        }
    }
}

[System.Serializable]
public struct AIFloorData
{
    public float m_chanceOfAiSpawn;
    public List<AISpawnData> m_aiFloorData;
    [System.Serializable]
    public struct AISpawnData
    {
        public EntityData m_entityType;
        public int m_minLevel, m_maxLevel;
        [Range(0, 1)]
        public float m_rarity;
    }

}


