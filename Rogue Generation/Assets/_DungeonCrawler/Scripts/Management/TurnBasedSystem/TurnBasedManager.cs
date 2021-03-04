using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ActivationEvent : UnityEvent { }


public class TurnBasedManager : MonoBehaviour
{
    public static TurnBasedManager Instance { get; private set; }

    public float m_lerpSpeed;
    public float m_normalLerpSpeed;
    public float m_fastLerpSpeed;
    public float m_moveDistance = 1;
    public List<TurnBasedAgent> m_keepAgents;
    public List<TurnBasedAgent> m_allAgents;
    public int m_currentAgentIndex;
    public TurnBasedAgent m_currentAgent;
    List<TurnBasedAgent> m_defeatedAgents = new List<TurnBasedAgent>();
    int m_indexAlter;


    //public List<ActionStoppingObjects>

    public ActivationEvent m_turnComplete = new ActivationEvent();
    public ActivationEvent m_cycleComplete = new ActivationEvent();



    private void Awake()
    {
        ToggleLerpSpeed(false);
        Instance = this;
        StartCoroutine(PerformTurns());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLerpSpeed(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleLerpSpeed(false);
        }
    }

    public void ToggleLerpSpeed(bool p_toggleFast)
    {
        if (p_toggleFast)
        {
            m_lerpSpeed = m_fastLerpSpeed;
        }
        else
        {
            m_lerpSpeed = m_normalLerpSpeed;
        }
    }

    public bool CurrentAgent(TurnBasedAgent p_agentRequest)
    {
        return m_allAgents[m_currentAgentIndex] == p_agentRequest;
    }


    /// <summary>
    /// Returns the previous agent, if there is one. 
    /// </summary>

    public TurnBasedAgent PreviousAgent()
    {
        if (m_currentAgentIndex - 1 < 0)
        {

            return m_allAgents[m_allAgents.Count - 1];
        }
        else
        {

            return m_allAgents[m_currentAgentIndex - 1];
        }
    }


    /// <summary>
    /// Runs when a turn has completed
    /// Any defeated agents are removed from the queue here, and the queue adjusts accordingly, fixing the turn order
    /// </summary>
    public void TurnComplete()
    {

        m_currentAgentIndex += 1 - m_indexAlter;
        m_indexAlter = 0;
        foreach (TurnBasedAgent defeated in m_defeatedAgents)
        {
            m_allAgents.Remove(defeated);

        }
        m_defeatedAgents.Clear();

        if (m_currentAgentIndex >= m_allAgents.Count)
        {
            m_currentAgentIndex = 0;
            m_cycleComplete.Invoke();
        }
        m_turnComplete.Invoke();
    }


    /// <summary>
    /// Called when an agent is defeated. As instantly removing it may mess up the queue, it stashes them into another list
    /// Once the turn is complete, then these agents are removed from the master agent queue
    /// </summary>

    public void AgentDefeated(TurnBasedAgent p_defeatedAgent)
    {
        int indexOfDeatedAgent = m_allAgents.IndexOf(p_defeatedAgent);
        if (indexOfDeatedAgent < m_currentAgentIndex)
        {
            m_indexAlter++;
        }
        m_defeatedAgents.Add(p_defeatedAgent);
        AIManager.Instance.AIDefeated();
    }

    /// <summary>
    /// Called from the dungeon manager, when a new ai has spawned.
    /// This adds it to the agent queue.
    /// </summary>
    public void NewAgent(GameObject p_newAgent)
    {
        m_allAgents.Add(p_newAgent.GetComponent<TurnBasedAgent>());
    }

    /// <summary>
    /// Clears all agents, save for the player, and their allies, when a new floor is loaded
    /// </summary>
    public void ClearAgents()
    {
        m_allAgents.Clear();
        foreach (TurnBasedAgent keepAgent in m_keepAgents)
        {
            m_allAgents.Add(keepAgent);

        }
        m_currentAgentIndex = 0;
    }

    /// <summary>
    /// This is where the turn system takes place.<br/> 
    /// This coroutine constantly runs, determining which active agent is allowed to perform their requested action.<br/>
    /// It cycles through all the agents, running events when an agent has completed their turn, and also when the queue has been cycled through<br/>
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformTurns()
    {
        while (true)
        {
            if (m_allAgents.Count > 0)
                for (int i = 0; i < m_allAgents.Count; i++)
                {
                    if (m_currentAgentIndex != i)
                    {
                        continue;
                    }
                    else if (m_currentAgentIndex == i)
                    {
                        m_currentAgent = m_allAgents[i];
                        m_allAgents[i].AgentUpdate();
                        break;
                    }
                }
            yield return null;
        }
    }
}
