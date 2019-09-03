using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public enum AiBehaviour { Idle, Attack };
    public AiBehaviour m_currentBehave;

    private DungeonManager m_dungeonManager;
    private SpriteRenderer m_spriteRender;
    private Animator m_visualsAnimator;


    #region Idle Behaviour
    private Vector3 m_currentTargetPos;
    private DungeonNavigation_Agent m_navAgent;
    private List<Node> m_path;
    [Header("Movement Properties")]
    public float m_nodeStoppingDistance = .1f;
    private Node m_currentNode;

    public int m_skipTurnAmount = 3;
    private int m_currentSkipTurn = 0;
    public LayerMask m_blockingMask;

    #endregion

    #region TurnBased System
    private TurnBasedAgent m_turnAgent;
    #endregion

    private void OnEnable()
    {
        m_navAgent = GetComponent<DungeonNavigation_Agent>();
        m_turnAgent = GetComponent<TurnBasedAgent>();
        m_spriteRender = transform.GetChild(1).GetComponent<SpriteRenderer>();
        m_visualsAnimator = transform.GetChild(1).GetComponent<Animator>();
    }


    /// <summary>
    /// Called from the Dungeon manager, in the SpawnAI Function
    /// This function will set up the ai, with the provided sprite, animator, stats, and moveset
    /// </summary>
    
    public void InitializeAi(AIType_Base aiType)
    {
        m_spriteRender.sprite = aiType.m_aiBaseSprite;
        m_visualsAnimator.runtimeAnimatorController = aiType.m_animController;
    }
    private void Start()
    {
        m_dungeonManager = DungeonManager.Instance;

    }

    private void Update()
    {
        switch (m_currentBehave)
        {
            case AiBehaviour.Idle:
                MoveAi();
                //CheckForPlayer();
                break;
            case AiBehaviour.Attack:
                //IsPlayerStillInRange()
                break;
        }
    }

    /// <summary>
    /// Creates a new path for the ai to walk towards, while in idle mode
    /// called when their current target has been reached
    /// TODO: Implement the ability for them to recalculate when they become stuck, and cannot continue on their current path
    /// </summary>
    public void NewPath()
    {
        m_currentTargetPos = m_dungeonManager.GetRandomCell(transform.position);
        m_path = m_navAgent.CreatePath(transform.position, m_currentTargetPos);
        if (m_path != null)
        {
            m_currentNode = m_path[0];
        }
    }

    /// <summary>
    /// Determines how far along the ai is on their path
    /// Calls the TurnAgent to allow the ai to move
    /// TODO: Put the stuck code here, using raycasts
    /// </summary>
    private void MoveAi()
    {
        if (Vector2.Distance(transform.position, (Vector2)m_currentTargetPos) < m_nodeStoppingDistance || m_path == null)
        {
            NewPath();
        }
        if (Vector2.Distance(transform.position, (Vector2)m_currentNode.worldPosition) < m_nodeStoppingDistance)
        {
            m_path.Remove(m_currentNode);
            m_currentNode = m_path[0];
        }


        ///The current stuck code
        RaycastHit2D oi = Physics2D.Raycast(m_currentNode.worldPosition, transform.forward, 100f, m_blockingMask);
        if (oi && oi.transform.gameObject !=this.gameObject)
        {
            if (m_currentSkipTurn >= m_skipTurnAmount)
            {
                NewPath();
            }
            m_currentSkipTurn++;
            m_turnAgent.Action_SkipTurn();
            

        }
        else
        {
            m_currentSkipTurn = 0;
            m_turnAgent.Action_Move(m_currentNode.worldPosition);
        }
        

    }
}
