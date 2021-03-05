using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnBasedAgent : MonoBehaviour
{

    public enum AgentAction { Move, Attack, UseItem, SkipTurn };
    public AgentAction m_currentAgentAction;
    //[HideInInspector]
    public bool m_performAction;           //The boolean that determines if they are doing an action
    [HideInInspector]
    public bool m_performingAction; //Used by the controller, to determine if they can start a new function
    [HideInInspector]
    public bool m_actionComplete = true;   //Used by the following agent, to determine if this agent is done performing its action


    public bool m_isPlayer;
    private TurnBasedManager m_turnManager;
    private ObjectPooler m_pooler;
    public TurnBasedAgent m_previousAgent;

    public EntityContainer m_entityContainer;


    #region Movement variables
    [HideInInspector]
    public Vector3 m_targetPos;

    private float m_currentMovementTimer;
    private Coroutine m_movementCoroutine;
    #endregion

    #region Attack Variables
    private int m_currentAttackIndex;
    public Vector3 m_attackSpawnPos;
    private Coroutine m_attackCoroutine;
    #endregion

    private void Awake()
    {
        m_entityContainer = GetComponent<EntityContainer>();
    }
    private void Start()
    {
        m_turnManager = TurnBasedManager.Instance;
        m_pooler = ObjectPooler.instance;
    }
    public void Reinitialize()
    {
        StopAllCoroutines();
        m_movementCoroutine = null;
        m_attackCoroutine = null;
        m_actionComplete = true;
        m_performAction = false;
        m_performingAction = false;
        m_currentAgentAction = AgentAction.Move;
    }

    public void SetupCellAttendence()
    {
        m_entityContainer.m_dungeonState.UpdateCellAttendance(transform.position);
        DungeonGenerationManager.Instance.AdjustEntityCheckGrid(transform.position.x, transform.position.y, gameObject);
    }

    public void RemoveCellAttendence()
    {
        DungeonGenerationManager.Instance.AdjustEntityCheckGrid(transform.position.x, transform.position.y, null);
    }

    /// <summary>
    /// This function is called from the turn system manager, in it's coroutine that constantly runs.
    /// </summary>
    public virtual void AgentUpdate()
    {
        if (m_performAction)
        {
            PerformTurn();
        }
    }


    //The state machine for this agent
    public virtual void PerformTurn()
    {
        switch (m_currentAgentAction)
        {

            case (AgentAction.Attack):

                if (m_attackCoroutine == null)
                {
                    m_attackCoroutine = StartCoroutine(Attack());
                    m_actionComplete = false;
                }

                break;
            case (AgentAction.Move):
                if (m_movementCoroutine == null)
                {
                    DungeonGenerationManager.Instance.AdjustEntityCheckGrid(transform.position.x, transform.position.y, null);
                    DungeonGenerationManager.Instance.AdjustEntityCheckGrid(m_targetPos.x, m_targetPos.y, gameObject);
                    m_movementCoroutine = StartCoroutine(Movement());

                    m_actionComplete = false;
                }

                //ToDo: Add in stepping on traps functionallity, ie, check for traps before ending the turn. 
                //IF they do step on a trap, wait for this agent's movement action to end, then activate the trap, 
                //and then wait for all the action objects to complete, then finish the turn
                EndTurn();
                break;
            case (AgentAction.UseItem):
                break;
            case (AgentAction.SkipTurn):
                m_entityContainer.m_entityVisualManager.SwitchToIdleAnimation();
                m_actionComplete = true;
                EndTurn();
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
            GameObject temp = gameObject;
            m_performAction = true;


        }
    }

    public void Action_Attack(int p_currentAttack)
    {
        if (!m_performingAction)
        {


            m_currentAttackIndex = p_currentAttack;
            m_currentAgentAction = AgentAction.Attack;
            GameObject temp = gameObject;
            m_performAction = true;

        }
    }

    public void Action_UseItem(/*ItemType_Base p_currentItemAction*/)
    {
        //m_currentItemAction = p_currentItemAction;
        m_currentAgentAction = AgentAction.UseItem;
        GameObject temp = gameObject;
        m_performAction = true;
        m_actionComplete = false;
    }

    public void Action_SkipTurn()
    {
        if (!m_performingAction)
        {
            m_currentAgentAction = AgentAction.SkipTurn;
            GameObject temp = gameObject;
            m_performAction = true;
        }
    }

    #endregion

    #region Turn Manage Methods
    //Completes this agent's turn
    public void EndTurn()
    {
        m_performAction = false;
        m_turnManager.TurnComplete();
    }

    #endregion

    #region Movement Methods

    /// <summary>
    /// The coroutine that enables the movement of the characters
    /// The coroutine is instantly initiated when the characters choose to move
    /// While this coroutine is running, no other action, from this character, can be performed
    /// 
    /// If the next agent, in the queue for the turn system, chooses to attack they will have to wait for this coroutine to be completed
    /// The opposite is also true
    /// </summary>

    private IEnumerator Movement()
    {


        
        m_performingAction = true;
        m_currentMovementTimer = 0;
        Vector3 startPos = transform.position;
        m_entityContainer.m_dungeonState.UpdateCellAttendance(m_targetPos);
        Vector2 newFacingDir = new Vector2(Mathf.Sign(m_targetPos.x - transform.position.x) * Mathf.Abs(m_targetPos.x - transform.position.x),
                                                        Mathf.Sign(m_targetPos.y - transform.position.y) * Mathf.Abs(m_targetPos.y - transform.position.y));
        m_entityContainer.m_movementController.UpdateFacingDir(newFacingDir);


        m_entityContainer.m_entityVisualManager.UpdateFacingDir(newFacingDir);
        m_entityContainer.m_entityVisualManager.SwitchToMovingAnimation();

        while (m_currentMovementTimer / m_turnManager.m_lerpSpeed < 1)
        {

            float percent = m_currentMovementTimer / m_turnManager.m_lerpSpeed;
            transform.position = Vector3.Lerp(startPos, m_targetPos, percent);
            m_currentMovementTimer += Time.deltaTime;
            yield return null;
        }
        transform.position = m_targetPos;

        /*if (m_isPlayer)
        {
            yield return new WaitForSeconds(.001f);
        }*/
        m_entityContainer.m_movementController.MovementComplete();
        m_entityContainer.m_entityVisualManager.UpdateFacingDir(newFacingDir);

        m_actionComplete = true;
        
        m_performingAction = false;
        m_movementCoroutine = null;
        yield return new WaitForSeconds(.025f);
        if(m_movementCoroutine == null)
        {
            m_entityContainer.m_entityVisualManager.SwitchToIdleAnimation();
        }
    }

    #endregion

    #region Attack Methods

    /// <summary>
    /// This attack coroutine, when initiated, will wait until the previous agent is done performing their action, then it will perform the attack action
    /// This calls the attack controller, to initate the attack and all it's components (the animation, and any follow up actions, such as applying damage and its animation)
    /// Once the attack controller returns the attack as complete, this agent ends its turn
    /// </summary>

    private IEnumerator Attack()
    {

        m_performingAction = true;
        m_previousAgent = m_turnManager.PreviousAgent();
        while (!m_previousAgent.m_actionComplete)
        {

            if (m_previousAgent == this) break;
            yield return null;
        }

        yield return StartCoroutine( m_entityContainer.m_attackController.StartAttack(m_currentAttackIndex));
        m_actionComplete = true;
        m_performingAction = false;
        m_attackCoroutine = null;
        EndTurn();

    }
    #endregion
}
