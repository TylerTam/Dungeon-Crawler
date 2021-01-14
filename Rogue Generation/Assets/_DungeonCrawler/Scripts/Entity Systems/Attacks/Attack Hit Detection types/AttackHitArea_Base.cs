using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The script that determines what type of attack this is
/// ie. ranged, full room, directly infront, or radius
/// </summary>
public abstract class AttackHitArea_Base : ScriptableObject
{
    public abstract List<AttackController> CommencedActions(AttackController p_attackController, Vector2 p_facingDir);
}
