using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    public static TurnBasedManager Instance { get; private set; }

    public float m_lerpSpeed;
    public float m_moveDistance = 1;
    public List<TurnBasedAgent> m_allAgents;
    int m_currentAgentIndex;
    List<TurnBasedAgent> m_defeatedAgents = new List<TurnBasedAgent>();
    int m_indexAlter;


    Coroutine m_turnCoroutine;

    //public List<ActionStoppingObjects>

    private void Awake()
    {
        Instance = this;
        StartCoroutine(PerformTurns());
    }
    public bool CurrentAgent(TurnBasedAgent p_agentRequest)
    {
        return m_allAgents[m_currentAgentIndex] == p_agentRequest;
    }

    public TurnBasedAgent PreviousAgent()
    {
        if (m_currentAgentIndex - 1 < 0)
        {
            
            return m_allAgents[m_allAgents.Count-1];
        }
        else
        {
            
            return m_allAgents[m_currentAgentIndex - 1];
        }
    }

    public void TurnComplete()
    {

        m_currentAgentIndex += 1 - m_indexAlter;
        m_indexAlter = 0;
        foreach (TurnBasedAgent defeated in m_defeatedAgents)
        {
            m_allAgents.Remove(defeated);
        }

        if (m_currentAgentIndex >= m_allAgents.Count)
        {
            m_currentAgentIndex = 0;
        }
        print("turn complete");
    }

    public void AgentDefeated(TurnBasedAgent p_defeatedAgent)
    {
        int indexOfDeatedAgent = m_allAgents.IndexOf(p_defeatedAgent);
        if (indexOfDeatedAgent < m_currentAgentIndex)
        {
            m_indexAlter++;
        }
        m_defeatedAgents.Add(p_defeatedAgent);
    }


    IEnumerator PerformTurns()
    {
        while (true)
        {
            for (int i = 0; i < m_allAgents.Count; i++)
            {
                if (m_currentAgentIndex != i)
                {
                    continue;
                }
                else if (m_currentAgentIndex == i)
                {
                    m_allAgents[i].AgentUpdate();
                    break;
                }
            }
            yield return null;
        }
    }
}
