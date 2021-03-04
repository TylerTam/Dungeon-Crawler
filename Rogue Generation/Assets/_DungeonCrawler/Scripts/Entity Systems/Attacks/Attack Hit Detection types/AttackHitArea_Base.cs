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
}
