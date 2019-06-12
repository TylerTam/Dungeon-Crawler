using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MapObjectBase : MonoBehaviour
{
    public Item_Type_Base m_itemType;
    SpriteRenderer m_sRend;
    ObjectPooler m_pooler;


    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        m_sRend = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Call this function whenever a new item is spawned on the map
    /// </summary>
    public void AssignObjectType(Item_Type_Base p_itemType)
    {
        m_itemType = p_itemType;
        if(m_sRend == null)
        {
            m_sRend = GetComponent<SpriteRenderer>();
        }
        m_sRend.sprite = p_itemType.m_objectSprite;
    }


    public void AddObject(Inventory p_ownerInventory, bool m_heldItem)
    {
        if (m_heldItem)
        {
            p_ownerInventory.m_heldItem = m_itemType;
        }
        else
        {
            p_ownerInventory.m_itemInventory.Add(m_itemType);
        }
        m_pooler.ReturnToPool(this.gameObject);
    }



}
