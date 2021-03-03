using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRunTurnSystem : MonoBehaviour
{
    public TurnBasedAgent m_playerAgent;

    public bool m_autoRun;

    private void Update()
    {
        if (m_autoRun)
        {

            m_playerAgent.m_currentAgentAction = TurnBasedAgent.AgentAction.SkipTurn;
            m_playerAgent.m_performAction = true;

        }
    }
}
