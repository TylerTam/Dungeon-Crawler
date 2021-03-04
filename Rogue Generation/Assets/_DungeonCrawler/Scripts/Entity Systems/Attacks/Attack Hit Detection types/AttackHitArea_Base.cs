using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The script that determines what type of attack this is
/// ie. ranged, full room, directly infront, or radius
/// </summary>
public abstract class AttackHitArea_Base : ScriptableObject
{
    public enum TargetTeams { Friendly, Enemy, All }
    public TargetTeams m_targetTeams;
    public int m_attackDistance;

    [Range(0, 1)]
    public float m_chanceOfAttack;

    public abstract List<AttackController> CommencedActions(AttackController p_attackController, Vector2 p_facingDir);

    public bool CanAddTarget(EntityTeam.Team p_target, EntityTeam.Team p_controllerTeam)
    {
        switch (m_targetTeams)
        {
            case TargetTeams.All:
                return true;
                break;
            case TargetTeams.Enemy:
                if (p_controllerTeam == EntityTeam.Team.Enemy)
                {
                    return (p_target == EntityTeam.Team.Player || p_target == EntityTeam.Team.Neutral);
                }
                else
                {
                    return (p_target == EntityTeam.Team.Enemy);
                }
                break;
            case TargetTeams.Friendly:
                if (p_controllerTeam == EntityTeam.Team.Enemy)
                {
                    return (p_target == EntityTeam.Team.Enemy);
                }
                else
                {
                    return (p_target == EntityTeam.Team.Player || p_target == EntityTeam.Team.Neutral);
                }
                break;
        }
        return false;
    }

    public virtual bool IsWithinRange(Vector2 p_attackerPos, Vector2 p_targetPos)
    {
        if (Mathf.Abs(p_targetPos.x - p_attackerPos.x) <= m_attackDistance && Mathf.Abs(p_targetPos.y - p_attackerPos.y) <= m_attackDistance)
        {
            if (Random.Range(0, 1f) > m_chanceOfAttack)
            {
                return true;
            }
        }
        return false;
    }
}
