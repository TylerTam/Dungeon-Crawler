using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType_Base : ScriptableObject
{
    public abstract List<TurnBasedAction> CommencedActions();
}
