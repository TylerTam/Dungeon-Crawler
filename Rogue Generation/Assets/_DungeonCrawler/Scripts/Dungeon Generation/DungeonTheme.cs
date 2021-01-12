using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Dungeon Theme", menuName = "DungeonGen/New Dungeon Theme", order = 0)]
public class DungeonTheme : ScriptableObject
{
    public Tile m_floorTile, m_wallTile;

    public List<DungeonType_Base> m_generationTypes;

    [Header("Item Properties")]
    public int m_maxItemsOnFloor;
    public int m_minItemsPerRoom, m_maxItemsPerRoom;

    public List<ItemStruct> m_itemsInDungeon;
    private List<ItemStruct> m_fixedItemRate;

    [Header("Enemy Properties")]
    [Range(0, 1)]
    public float m_chanceOfEnemySpawn;  //How often a new enemy will spawn, after a cycle is complete. 1 = always, if there is room, 0 = never.
    public List<AiStruct> m_aiInDungeon;
    private List<AiStruct> m_fixedAiRate;

    /// <summary>
    /// This method fixes the spawn rates of the items and the ai.
    /// It clamps them all to add up to 1.
    /// IE, if all the item spawn rates added up to 3, this fixes that so it maintains the rate among the other objects, but is less than 1.
    /// This allows easier random chance calculations
    /// </summary>
    public void FixRates()
    {
        m_fixedItemRate = new List<ItemStruct>();
        float currentRate = 0, changePercent = 0;
        #region Item Fix
        foreach (ItemStruct item in m_itemsInDungeon)
        {
            currentRate += item.m_itemRarity;
        }
        if (currentRate == 1)
        {
            m_fixedItemRate = m_itemsInDungeon;
        }
        else
        {
            changePercent = 1 / currentRate;
            foreach (ItemStruct item in m_itemsInDungeon)
            {
                m_fixedItemRate.Add(new ItemStruct(item.m_itemType, item.m_itemRarity * changePercent));
            }
        }
        #endregion

        #region Ai Fix
        m_fixedAiRate = new List<AiStruct>();
        currentRate = 0;

        foreach (AiStruct ai in m_aiInDungeon)
        {
            currentRate += ai.m_aiRarity;
        }
        if (currentRate == 1)
        {
            m_fixedAiRate = m_aiInDungeon;
        }
        else
        {
            changePercent = 1 / currentRate;
            foreach (AiStruct ai in m_aiInDungeon)
            {
                m_fixedAiRate.Add(new AiStruct(ai.m_aiType, ai.m_aiRarity * changePercent));
            }
        }
        #endregion

    }



    public List<DungeonGridCell> CreateNewFloor(DungeonManager p_gen, DungeonNavigation p_dungeonNav)
    {

        p_gen.m_dungeonGenTypeIndex = Random.Range(0, m_generationTypes.Count);
        p_gen.m_currentDungeonType = m_generationTypes[p_gen.m_dungeonGenTypeIndex];
        return m_generationTypes[p_gen.m_dungeonGenTypeIndex].CreateDungeon(p_gen, this, p_dungeonNav);

    }

    #region Item Properties

    /// <summary>
    /// Returns a list of items to spawn, with the rarity rate adding up to 1, no more, no less
    /// if the existing struct does not equal 1, it adjusts the rates so that it does add up to 1
    /// </summary>

    public List<ItemStruct> ItemsInDungeon()
    {
        return m_fixedItemRate;
    }

    public List<AiStruct> AiInDungeon()
    {
        return m_fixedAiRate;
    }


    #endregion
}

[System.Serializable]
public class ItemStruct
{
    public Item_Type_Base m_itemType;
    [Range(0, 1)]
    public float m_itemRarity;

    public ItemStruct(Item_Type_Base p_itemType, float p_rarity)
    {
        m_itemRarity = p_rarity;
        m_itemType = p_itemType;
    }
}

[System.Serializable]
public class AiStruct
{
    public AIType_Base m_aiType;
    [Range(0, 1)]
    public float m_aiRarity;

    public AiStruct(AIType_Base p_aiType, float p_rarity)
    {
        m_aiRarity = p_rarity;
        m_aiType = p_aiType;
    }
}
