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
            if (!DungeonGenerationManager.Instance.IsInGrid(startingPos.x + (i * dirX), startingPos.y + (i * dirY))) continue;
            if (DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + (i * dirX), startingPos.y + (i * dirY)) != null)
            {
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
            for (int x = -p_explosionRange.x; x <= p_explosionRange.x; x++)
            {
                for (int y = -p_explosionRange.y; y <= p_explosionRange.y; y++)
                {
                    if (!DungeonGenerationManager.Instance.IsInGrid(startingPos.x + x, startingPos.y + y)) continue;
                    if (DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + x, startingPos.y + y) != null)
                    {
                        AttackController addCont = DungeonGenerationManager.Instance.GetEntityCheck(startingPos.x + x, startingPos.y + y).GetComponent<AttackController>();
                        if (affectedControllers.Contains(addCont)) continue;
                        affectedControllers.Add(addCont);
                    }
                }
            }
        }
        return affectedControllers;

    }

    public override bool IsWithinRange(Vector2 p_attackerPos, Vector2 p_targetPos, int p_range, EntityTeam.Team p_attackerTeam)
    {
        int xDis = (int)p_targetPos.x - (int)p_attackerPos.x;
        int yDis = (int)p_targetPos.y - (int)p_attackerPos.y;

        if (Mathf.Abs(xDis) != Mathf.Abs(yDis) && yDis != 0 && xDis != 0) return false;

        if (Mathf.Abs(xDis) > p_range || Mathf.Abs(yDis) > p_range) return false;

        int dis = 0;
        if(Mathf.Abs(xDis) >= Mathf.Abs(yDis))
        {
            dis = Mathf.Abs(xDis);
        }else if (Mathf.Abs(xDis) < Mathf.Abs(yDis))
        {
            dis = Mathf.Abs(yDis);
        }

        int xDir = xDis == 0 ? 0 : (int)Mathf.Sign((p_targetPos.x - p_attackerPos.x));
        int yDir = yDis == 0 ? 0 : (int)Mathf.Sign((p_targetPos.y - p_attackerPos.y));

        for (int i = 1; i <= dis; i++)
        {
            if(DungeonGenerationManager.Instance.IsInGrid(i*xDir + p_attackerPos.x, i*yDir + p_attackerPos.y))
            {
                if (DungeonGenerationManager.Instance.GetWallCheck(i * xDir + p_attackerPos.x, i * yDir + p_attackerPos.y) == GlobalVariables.m_wallCell) return false;
                GameObject check = DungeonGenerationManager.Instance.GetEntityCheck(i * xDir + p_attackerPos.x, i * yDir + p_attackerPos.y);
                if(check != null)
                {
                    if(CanAddTarget(check.GetComponent<EntityTeam>().m_currentTeam, p_attackerTeam))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        return false;
    }
}
