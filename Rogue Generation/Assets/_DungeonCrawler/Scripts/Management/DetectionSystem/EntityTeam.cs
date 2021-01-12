using UnityEngine;

public class EntityTeamEvent: UnityEngine.Events.UnityEvent { }
public class EntityTeam : MonoBehaviour
{
    public enum Team { Player, Enemy, Neutral}
    public Team m_currentTeam;
    public EntityTeamEvent m_teamChangedEvent;

    public void ChangeTeam(Team p_newTeam)
    {
        m_currentTeam = p_newTeam;
        m_teamChangedEvent.Invoke();
    }
}
