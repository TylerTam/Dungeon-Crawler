using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackDetection_Range", menuName = "Attacks/Attack Detection / Range", order = 0)]
public class AttackHitArea_Range : AttackHitArea_Base
{
    public override List<AttackController> CommencedActions(AttackController p_attackController, Vector2 p_facingDir, int p_range, Vector2Int p_explosionRange)
    {
        List<AttackController> affectedControllers = new List<AttackController>();

        bool foundTarget = false;
        Vector2 startingPos = p_attackController.transform.position;
        int dirX = (int)((Mathf.Abs(p_facingDir.x) < 0.5f) ? 0 : Mathf.Sign(p_facingDir.x));
        int dirY = (int)((Mathf.Abs(p_facingDir.y) < 0.5f) ? 0 : Mathf.Sign(p_facingDir.y));

        for (int i = 1; i <= p_range; i++)
        {
            if (DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + (i * dirX), startingPos.y + (i * dirY)) != null)
            {
                Debug.Log("Found Target");
                startingPos = startingPos + new Vector2((i * dirX), (i * dirY));
                foundTarget = true;
                AttackController target = DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x, startingPos.y).GetComponent<AttackController>();
                if (CanAddTarget(target.m_entityContainer.m_entityTeam.m_currentTeam, p_attackController.m_entityContainer.m_entityTeam.m_currentTeam))
                {
                    affectedControllers.Add(target);
                }
                else
                {
                    affectedControllers.Add(null);
                }
                break;
            }
        }
        if (!foundTarget)
        {

            for (int i = 1; i < p_range; i++)
            {
                if (DungeonGenerationManager.Instance.GetWallCheck(startingPos.x + (i * dirX), startingPos.y + (i * dirY)) == GlobalVariables.m_wallCell)
                {
                    startingPos = startingPos + new Vector2((i * dirX), (i * dirY));
                    foundTarget = true;
                    break;
                }
            }
            if (!foundTarget)
            {
                startingPos = startingPos + new Vector2(dirX, dirY) * p_range;
            }

            affectedControllers.Add(null);
        }


        if (p_explosionRange != Vector2Int.zero)
        {
            for (int x = 1; x < p_explosionRange.x; x++)
            {
                for (int y = 1; y < p_explosionRange.y; y++)
                {
                    if (DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + x, startingPos.y + dirY) != null)
                    {
                        affectedControllers.Add(DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + x, startingPos.y + dirY).GetComponent<AttackController>());
                    }
                }
            }
        }
        return affectedControllers;

    }

    public override bool IsWithinRange(Vector2 p_attackerPos, Vector2 p_targetPos, int p_range)
    {
        throw new System.NotImplementedException();
    }
}
