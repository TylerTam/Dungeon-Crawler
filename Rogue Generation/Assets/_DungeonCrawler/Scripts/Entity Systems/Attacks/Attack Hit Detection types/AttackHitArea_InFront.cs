using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDetection_InFront", menuName = "Attacks/Attack Detection / InFront", order = 0)]
public class AttackHitArea_InFront : AttackHitArea_Base
{
    public override List<AttackController> CommencedActions(AttackController p_attackController, Vector2 p_facingDir, int p_range, Vector2Int p_explosionRange)
    {
        List<AttackController> gatheredActions = new List<AttackController>();

        Vector2 pos = new Vector2(p_attackController.transform.position.x + p_facingDir.x, -(p_attackController.transform.position.y + p_facingDir.y));
        GameObject hitObject = DungeonGenerationManager.Instance.GetEntityCheck(pos.x, pos.y);
        if (hitObject)
        {
            EntityTeam.Team hitTeam = hitObject.GetComponent<EntityContainer>().m_entityTeam.m_currentTeam;

            if (CanAddTarget(hitTeam, p_attackController.m_entityContainer.m_entityTeam.m_currentTeam))
            {
                gatheredActions.Add(hitObject.transform.GetComponent<AttackController>());
            }



        }

        return gatheredActions;

    }

    public override bool IsWithinRange(Vector2 p_attackerPos, Vector2 p_targetPos, int p_range, EntityTeam.Team p_team)
    {
        if ((int)Mathf.Abs(p_targetPos.x - p_attackerPos.x) <= p_range && (int)Mathf.Abs(Mathf.Abs((int)p_targetPos.y) - Mathf.Abs((int)p_attackerPos.y)) <= p_range)
        {
            //Debug.DrawLine(p_targetPos, p_attackerPos, Color.red, 2f);
            return true;
        }
        return false;
    }

}
