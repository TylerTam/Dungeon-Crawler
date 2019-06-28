using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Items/Heal Item", order = 0)]

public class Item_Type_Heal : Item_Type_Base
{
    public float healAmount;
    public override void UseItem()
    {
        Debug.Log("Heal " + healAmount + " HP Points");
    }
}
