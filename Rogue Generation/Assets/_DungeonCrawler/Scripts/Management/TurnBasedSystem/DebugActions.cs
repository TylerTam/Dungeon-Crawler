using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActions : TurnBasedAgent
{
    public override void AgentUpdate()
    {
        return;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            m_actionComplete = true;
            EndTurn();
        }
    }
}
