using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedAgent_AI : TurnBasedAgent
{
    public AIController m_aiCont;

    public override void AgentUpdate()
    {
        m_aiCont.UpdateAI();
    }

}
