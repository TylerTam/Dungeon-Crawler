using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnBasedAgent : MonoBehaviour
{

    public enum AgentAction { Move, Attack, UseItem };
    public AgentAction m_currentAgentAction;
    private bool m_performAction;           //The boolean that determines if they are doing an action
    //S[HideInInspector]
    public bool m_performingAction; //Used by the controller, to determine if they can start a new function
    [HideInInspector]
    public bool m_actionComplete = true;   //Used by the following agent, to determine if this agent is done performing its action

    public TurnBasedAgent m_previousAgent;
    
    private TurnBasedManager m_turnManager;
    private ObjectPooler m_pooler;

    #region Movement variables
    [HideInInspector]
    public Vector3 m_targetPos;


    [Header("Pre-set values")]
    public Transform m_predictedPlace;

    private float m_currentMovementTimer;
    private Coroutine m_movementCoroutine;
    private Entity_MovementController m_movementController;
    #endregion

    #region Attack Variables
    private AttackController m_attackController;
    private int m_currentAttackIndex;
    public Vector3 m_attackSpawnPos;
    private Coroutine m_attackCoroutine;
    #endregion

    private void Start()
    {
        m_turnManager = TurnBasedManager.Instance;
        m_pooler = ObjectPooler.instance;
        m_attackController = GetComponent<AttackController>();
        m_movementController = GetComponent<Entity_MovementController>();
    }
    public void AgentUpdate()
    {
        if (m_performAction)
        {
            PerformTurn();
        }
    }


    //The state machine for this agent
    void PerformTurn()
    {
        switch (m_currentAgentAction)
        {

            case (AgentAction.Attack):
                if (m_attackCoroutine == null)
                {
                    m_attackCoroutine = StartCoroutine(Attack());
                }
                
                break;
            case (AgentAction.Move):
                if (m_movementCoroutine == null)
                {
                    m_movementCoroutine = StartCoroutine(Movement());
                }
                

                //ToDo: Add in stepping on traps functionallity, ie, check for traps before ending the turn. 
                //IF they do step on a trap, wait for this agent's movement action to end, then activate the trap, 
                //and then wait for all the action objects to complete, then finish the turn
                EndTurn();
                break;
            case (AgentAction.UseItem):
                break;
        }

    }


    //Called from the controllers, and starts the turn
    #region Called From controller
    public void Action_Move(Vector3 p_targetPos)
    {
        if (!m_performingAction)
        {


            m_targetPos = p_targetPos;
            m_currentAgentAction = AgentAction.Move;
            m_performAction = true;
            m_actionComplete = false;
        }
    }

    public void Action_Attack(int p_currentAttack)
    {
        if (!m_performingAction)
        {


            m_currentAttackIndex = p_currentAttack;
            print(m_currentAttackIndex);
            m_currentAgentAction = AgentAction.Attack;
            m_performAction = true;
            m_actionComplete = false;
        }
    }

    public void Action_UseItem(/*ItemType_Base p_currentItemAction*/)
    {
        //m_currentItemAction = p_currentItemAction;
        m_currentAgentAction = AgentAction.UseItem;
        m_performAction = true;
        m_actionComplete = false;
    }

    #endregion
    
    #region Turn Manage Methods
    //Completes this agent's turn
    void EndTurn()
    {
        m_performAction = false;
        m_turnManager.TurnComplete();
    }

    #endregion

    #region Movement Methods
    IEnumerator Movement()
    {
        
        m_performingAction = true;
        m_currentMovementTimer = 0;
        Vector3 startPos = transform.position;
        while (m_currentMovementTimer / m_turnManager.m_lerpSpeed < 1)
        {
            m_predictedPlace.transform.position = m_targetPos;
            float percent = m_currentMovementTimer / m_turnManager.m_lerpSpeed;
            transform.position = Vector3.Lerp(startPos, m_targetPos, percent);
            m_currentMovementTimer += Time.deltaTime;
            yield return null;
        }
        transform.position = m_targetPos;
        //print("Target Pos: " + m_targetPos + " | Transform: " + transform.position);
        m_predictedPlace.transform.position = m_targetPos;
        
        m_actionComplete = true;
        m_movementController.MovementComplete();
        m_performingAction = false;
        m_movementCoroutine = null;
    }
    #endregion

    #region Attack Methods
    IEnumerator Attack()
    {
        
        m_performingAction = true;
        m_previousAgent = m_turnManager.PreviousAgent();
        while (!m_previousAgent.m_actionComplete)
        {
            if (m_previousAgent == this) break;
            yield return null;
        }
        

        m_attackController.StartAttack(m_currentAttackIndex);
        
        while (!m_attackController.m_attackComplete)
        {
            yield return null;
        }
        m_actionComplete = true;
        m_performingAction = false;
        m_attackCoroutine = null;
        EndTurn();

    }
    #endregion
}
