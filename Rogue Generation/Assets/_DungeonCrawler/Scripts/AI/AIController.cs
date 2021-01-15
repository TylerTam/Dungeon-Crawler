using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public enum AiBehaviour { Idle, Attack };
    public AiBehaviour m_currentBehaviour;
    public EntityData m_entityType;

    private DungeonManager m_dungeonManager;
    private EntityContainer m_entityContainer;


    #region Idle Behaviour
    /// <summary>
    /// The target position for the ai to move towards
    /// </summary>
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


    #region Detection

    public Vector2Int m_detectionRadius;
    public LayerMask m_detectionMask;
    public GameObject m_currentTarget;
    private Transform m_currentTargetPrediction;
    public Transform m_predictedPlace;
    #endregion
    private void Awake()
    {
        m_entityContainer = GetComponent<EntityContainer>();
        m_navAgent = GetComponent<DungeonNavigation_Agent>();
    }
    private bool m_restart;
    /// <summary>
    /// Called from the Dungeon manager, in the SpawnAI Function
    /// This function will set up the ai, with the provided sprite, animator, stats, and moveset
    /// </summary>

    public void InitializeAi(EntityData p_entityType)
    {
        m_entityType = p_entityType;
        m_entityContainer.m_entityVisualManager.AssignEntityData(p_entityType);
        m_path = null;
        m_currentNode = null;
        m_entityContainer.Reinitialize(p_entityType);
        m_currentTarget = null;
        m_currentBehaviour = AiBehaviour.Idle;
        m_path = null;
        m_currentNode = null;
        m_currentTargetPrediction = null;
        if (m_dungeonManager != null)
        {
            m_restart = true;
            NewPath();
        }
    }
    private void Start()
    {
        m_dungeonManager = DungeonManager.Instance;

    }

    public void UpdateAI()
    {
        if (m_restart)
        {
            Debug.Log("Reused AI");
        }
        CheckCurrentBehaviour();
        switch (m_currentBehaviour)
        {
            case AiBehaviour.Idle:
                MoveAi();
                //CheckForPlayer();
                break;
            case AiBehaviour.Attack:
                if (CanAttack())
                {
                    ChooseAttack();
                }
                else
                {
                    MoveAi();

                }

                break;
        }

        m_entityContainer.m_turnBasedAgent.PerformTurn();
    }

    /// <summary>
    /// Creates a new path for the ai to walk towards, while in idle mode
    /// called when their current target has been reached
    /// TODO: Implement the ability for them to recalculate when they become stuck, and cannot continue on their current path
    /// </summary>
    public void NewPath(bool p_continueCurrent = false)
    {


        if (!p_continueCurrent)
        {
            switch (m_currentBehaviour)
            {
                case AiBehaviour.Attack:
                    m_currentTargetPos = (Vector2)m_currentTargetPrediction.transform.position;
                    break;
                case AiBehaviour.Idle:
                    m_currentTargetPos = m_dungeonManager.GetRandomCell(transform.position);
                    break;
            }
        }

        m_path = m_navAgent.CreatePath(transform.position, m_currentTargetPos);
        if (m_path != null)
        {
            m_currentNode = m_path[0];
        }
        else
        {
            Debug.LogError("PAth Cannot Calculate from: " + gameObject.name + " | to: " + m_currentTargetPos, this);
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
        if (m_currentNode == null || m_path == null)
        {
            NewPath();

        }
        if (m_currentBehaviour == AiBehaviour.Attack)
        {

            if (m_currentTargetPrediction.position != m_currentTargetPos)
            {
                NewPath();
            }

        }
        if (Vector2.Distance(transform.position, (Vector2)m_currentNode.worldPosition) < m_nodeStoppingDistance)
        {
            m_path.Remove(m_currentNode);
            if (m_path.Count > 0)
            {
                m_currentNode = m_path[0];
            }
            else
            {
                m_currentNode = null;
            }
        }


        ///The current stuck code
        if (m_currentNode != null)
        {
            RaycastHit2D stuckCast = Physics2D.Raycast(m_currentNode.worldPosition, transform.forward, 100f, m_blockingMask);
            if (stuckCast && stuckCast.transform.gameObject != this.gameObject)
            {
                if (m_currentSkipTurn >= m_skipTurnAmount)
                {
                    NewPath();
                }
                m_currentSkipTurn++;
                m_entityContainer.m_turnBasedAgent.Action_SkipTurn();


            }
            else
            {
                m_currentSkipTurn = 0;
                m_entityContainer.m_turnBasedAgent.Action_Move(m_currentNode.worldPosition);
            }
        }
        else
        {
            m_currentSkipTurn++;
            m_entityContainer.m_turnBasedAgent.Action_SkipTurn();
        }



    }


    public void CheckCurrentBehaviour()
    {
        bool findNewPath = false;
        if (m_currentTarget != null)
        {
            if (!CanSeeTarget())
            {
                findNewPath = true;
                m_currentTarget = SearchForNewTarget();
            }
        }
        else
        {
            m_currentTarget = SearchForNewTarget();
            if (m_currentTarget != null)
            {
                m_currentBehaviour = AiBehaviour.Attack;
                NewPath();
            }
        }


        if (m_currentTarget == null)
        {
            m_currentBehaviour = AiBehaviour.Idle;
            if (findNewPath)
            {
                m_currentTargetPos = m_path[m_path.Count - 1].worldPosition;
                NewPath(m_path.Count > 1);
            }
        }
        else
        {
            m_currentBehaviour = AiBehaviour.Attack;
        }

    }

    #region Detection
    public bool CanSeeTarget()
    {
        if (m_entityContainer.m_dungeonState)
        {
            ///If in a room, check the entire room
            if (m_entityContainer.m_dungeonState.m_inRoom)
            {

                if (m_entityContainer.m_dungeonState.m_currentCell.m_entitiesInRoom.Contains(m_currentTarget))
                {
                    return true;
                }
            }

            #region Radius Check
            ///Else, only check 2 sqaures around
            if (Vector3.Distance(m_predictedPlace.position, m_currentTargetPrediction.position) < 1.45 * 2)
            {
                return true;
            }
            #endregion

        }
        return false;
    }

    public GameObject SearchForNewTarget()
    {
        if (m_entityContainer.m_dungeonState)
        {

            ///If in a room, check the entire room
            if (m_entityContainer.m_dungeonState.m_inRoom)
            {
                #region Room Check
                foreach (GameObject ent in m_entityContainer.m_dungeonState.m_currentCell.m_entitiesInRoom)
                {
                    EntityContainer newTeam = ent.GetComponent<EntityContainer>();
                    if (newTeam.m_entityTeam.m_currentTeam != m_entityContainer.m_entityTeam.m_currentTeam && newTeam.m_entityTeam.m_currentTeam != EntityTeam.Team.Neutral)
                    {
                        m_currentTargetPrediction = newTeam.m_turnBasedAgent.m_predictedPlace.transform;
                        return ent.transform.gameObject;

                    }
                }
                #endregion
            }
            else
            {
                #region Radius Check
                ///Else, only check 2 sqaures around
                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        //Debug.DrawLine(m_predictedPlace.position, m_predictedPlace.position + new Vector3(x, y), Color.green, 1f);
                        RaycastHit2D hit2D = Physics2D.Raycast(m_predictedPlace.position + new Vector3(x, y, 0) - Vector3.forward * 1, Vector3.forward, 5, m_detectionMask);
                        if (hit2D)
                        {
                            EntityContainer newTeam = hit2D.transform.GetComponent<EntityContainer>();
                            if (newTeam.m_entityTeam.m_currentTeam != m_entityContainer.m_entityTeam.m_currentTeam && newTeam.m_entityTeam.m_currentTeam != EntityTeam.Team.Neutral)
                            {
                                Debug.Log("Player Located: Radius");
                                m_currentTargetPrediction = newTeam.m_turnBasedAgent.m_predictedPlace.transform;
                                return hit2D.transform.gameObject;
                            }
                        }
                    }
                }
                #endregion
            }


        }
        return null;
    }
    #endregion

    #region Attacking Behaviour


    public bool CanAttack()
    {
        //Check if any attacks can currently hit
        return false;
    }
    public int ChooseAttack()
    {
        //Check which attack can hit
        return 0;
    }
    #endregion

}
