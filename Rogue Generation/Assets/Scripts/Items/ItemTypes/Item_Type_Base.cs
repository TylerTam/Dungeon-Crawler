using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item_Type_Base : ScriptableObject
{
    public string m_objectName;
    public Item_MapObjectBase m_itemGameWorldPrefab;
    public Sprite m_objectSprite;

    public abstract void UseItem ();
    public virtual void ThrowItem(Vector3 p_startPos, Vector3 p_thrownDir, float m_thrownSpeed)
    {
        ObjectPooler.instance.NewObject(m_itemGameWorldPrefab.gameObject, p_startPos, Quaternion.identity).GetComponent<Item_MapObjectBase>().AssignObjectType(this);
    }
}
