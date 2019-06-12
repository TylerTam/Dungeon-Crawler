using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Dungeon Theme", menuName = "DungeonGen/New Dungeon Theme", order = 0)]
public class DungeonTheme : ScriptableObject
{
    public Tile m_floorTile, m_wallTile;

    public List<DungeonType_Base> m_generationTypes;
    
    [Header ("Item Properties")]
    public int m_maxItemsOnFloor;
    public int m_minItemsPerRoom, m_maxItemsPerRoom;

    public List<ItemStruct> m_itemsInDungeon;
    
    






    public List<DungeonType_Base.DungeonGridCell> CreateNewFloor(DungeonGenerator p_gen, DungeonNavigation p_dungeonNav)
    {

       p_gen.m_dungeonGenTypeIndex = Random.Range(0, m_generationTypes.Count);
        return m_generationTypes[p_gen.m_dungeonGenTypeIndex].CreateDungeon(p_gen, this, p_dungeonNav);

    }

    #region Item Properties

    /// <summary>
    /// Returns a list of items to spawn, with the rarity rate adding up to 1, no more, no less
    /// if the existing struct does not equal 1, it adjusts the rates so that it does add up to 1
    /// </summary>

    public List<ItemStruct> ItemsInDungeon(DungeonGenerator p_gen)
    {
        float currentItemRate = 0;
        foreach (ItemStruct currentItem in m_itemsInDungeon)
        {
            currentItemRate += currentItem.m_itemRarity;
        }
        if (currentItemRate == 1)
        {
            return m_itemsInDungeon;
        }
        else
        {
            float changePercent = 1 / currentItemRate;
            List<ItemStruct> fixedItemRate = new List<ItemStruct>();
            for (int i = 0; i < m_itemsInDungeon.Count; i++)
            {

                fixedItemRate.Add(new ItemStruct(m_itemsInDungeon[i].m_itemType, m_itemsInDungeon[i].m_itemRarity * changePercent));
            }
            p_gen.m_fixedRatios = fixedItemRate;
            
            return fixedItemRate;

        }
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
