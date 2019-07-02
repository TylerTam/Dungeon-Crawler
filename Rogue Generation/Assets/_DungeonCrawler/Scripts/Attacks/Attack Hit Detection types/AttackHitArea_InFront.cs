using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDetection_InFront", menuName = "Attack Detection / InFront", order = 0)]
public class AttackHitArea_InFront : AttackHitArea_Base
{
    public override List<TurnBasedAction> CommencedActions(AttackController p_attackController)
    {
        List<TurnBasedAction> gatheredActions = new List<TurnBasedAction>();
        Vector3 attackPos = new Vector3(p_attackController.transform.position.x + p_attackController.m_facingDir.x, p_attackController.transform.position.y + p_attackController.m_facingDir.y,0);
        RaycastHit2D hit = Physics2D.Raycast(attackPos, Vector3.forward, 100f, p_attackController.m_enemyMask);

        if (hit)
        {
            gatheredActions.Add(hit.transform.GetComponent<TurnBasedAction>());
        }

        return gatheredActions;

    }
}
