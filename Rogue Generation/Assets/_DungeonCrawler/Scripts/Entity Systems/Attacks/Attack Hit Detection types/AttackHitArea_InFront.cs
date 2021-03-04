using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDetection_InFront", menuName = "Attack Detection / InFront", order = 0)]
public class AttackHitArea_InFront : AttackHitArea_Base
{
    public override List<AttackController> CommencedActions(AttackController p_attackController, Vector2 p_facingDir)
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



}
