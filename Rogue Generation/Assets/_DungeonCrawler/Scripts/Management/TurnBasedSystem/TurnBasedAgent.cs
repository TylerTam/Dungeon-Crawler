using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnBasedAgent : MonoBehaviour
{

    public enum AgentAction { Move, Attack, UseItem, SkipTurn };
    public AgentAction m_currentAgentAction;
    [HideInInspector]
    public bool m_performAction;           //The boolean that determines if they are doing an action
    [HideInInspector]
    public bool m_performingAction; //Used by the controller, to determine if they can start a new function
    [HideInInspector]
    public bool m_actionComplete = true;   //Used by the following agent, to determine if this agent is done performing its action

    public TurnBasedAgent m_previousAgent;

    private TurnBasedManager m_turnManager;
    private ObjectPooler m_pooler;

    private EntityDungeonState m_dungeonState;
    public bool m_isPlayer;


    #region Movement variables
    [HideInInspector]
    public Vector3 m_targetPos;


    [Header("Pre-set values")]
    public Transform m_predictedPlace;
    ///Predicted place is used to make sure no characters land on top of each other. 
    ///It is a collision box that is moved to the target position, so that any ai are able to know where the player will be once their movement is complete
    ///this is to stop the ai from targeting an occupied spot, when moving, and also to stop targeting an empty spot, when attacking

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
        m_dungeonState = GetComponent<EntityDungeonState>();

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
            m_performAction = true;


        }
    }

    public void Action_Attack(int p_currentAttack)
    {
        if (!m_performingAction)
        {


            m_currentAttackIndex = p_currentAttack;
            m_currentAgentAction = AgentAction.Attack;
            m_performAction = true;

        }
    }

    public void Action_UseItem(/*ItemType_Base p_currentItemAction*/)
    {
        //m_currentItemAction = p_currentItemAction;
        m_currentAgentAction = AgentAction.UseItem;
        m_performAction = true;
        m_actionComplete = false;
    }

    public void Action_SkipTurn()
    {
        if (!m_performingAction)
        {
            m_currentAgentAction = AgentAction.SkipTurn;
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
        m_predictedPlace.transform.position = m_targetPos;
        m_dungeonState.UpdateCellAttendance();
        m_movementController.UpdateFacingDir(new Vector2(Mathf.Sign(m_targetPos.x - transform.position.x) * Mathf.Abs(m_targetPos.x - transform.position.x), 
                                                        Mathf.Sign(m_targetPos.y - transform.position.y) * Mathf.Abs(m_targetPos.y - transform.position.y)));

        while (m_currentMovementTimer / m_turnManager.m_lerpSpeed < 1)
        {

            m_predictedPlace.transform.position = m_targetPos;
            float percent = m_currentMovementTimer / m_turnManager.m_lerpSpeed;
            transform.position = Vector3.Lerp(startPos, m_targetPos, percent);
            m_currentMovementTimer += Time.deltaTime;
            yield return null;
        }
        transform.position = m_targetPos;

        m_predictedPlace.transform.position = m_targetPos;

        if (m_isPlayer)
        {
            yield return new WaitForSeconds(.001f);
        }
        m_actionComplete = true;
        m_movementController.MovementComplete();
        m_performingAction = false;
        m_movementCoroutine = null;
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
