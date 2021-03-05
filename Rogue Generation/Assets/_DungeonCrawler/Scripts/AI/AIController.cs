using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public enum AiBehaviour { Idle, Attack };
    public AiBehaviour m_currentBehaviour;
    public EntityData m_entityType;

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

    private int m_skipTurnAmount = 1;
    private int m_currentSkipTurn = 0;

    private bool m_reused;
    #endregion


    #region Detection
    public GameObject m_currentTarget;
    private EntityDungeonState m_targetDungeonState;
    #endregion
    private void Awake()
    {
        m_entityContainer = GetComponent<EntityContainer>();
        m_navAgent = GetComponent<DungeonNavigation_Agent>();
    }
    /// <summary>
    /// Called from the Dungeon manager, in the SpawnAI Function
    /// This function will set up the ai, with the provided sprite, animator, stats, and moveset
    /// </summary>

    public void InitializeAi(EntityData p_entityType, int p_level)
    {
        m_entityType = p_entityType;
        m_entityContainer.m_entityVisualManager.AssignEntityData(p_entityType);
        m_entityContainer.Reinitialize(p_entityType, p_level);
        m_currentTarget = null;
        m_targetDungeonState = null;
        m_path = null;
        m_currentNode = null;
        m_currentBehaviour = AiBehaviour.Idle;
    }

    public void UpdateAI()
    {
        CheckCurrentBehaviour();
        switch (m_currentBehaviour)
        {
            case AiBehaviour.Idle:
                MoveAi();
                break;
            case AiBehaviour.Attack:
                int currentAttack = 0;
                if (CanAttack(out currentAttack))
                {
                    Vector2 facingDir = ((Vector2)m_targetDungeonState.m_currentGridPosition - (Vector2)transform.position).normalized;
                    m_entityContainer.m_movementController.UpdateFacingDir(facingDir);
                    m_entityContainer.m_entityVisualManager.UpdateFacingDir(facingDir);

                    m_entityContainer.m_turnBasedAgent.Action_Attack(currentAttack);
                }
                else
                {
                    CheckAttackTargetPosition();
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
    public void NewPath(bool p_lostPlayer = false)
    {



        switch (m_currentBehaviour)
        {
            case AiBehaviour.Attack:
                m_currentTargetPos = (Vector2)m_targetDungeonState.m_currentGridPosition;
                break;
            case AiBehaviour.Idle:
                if (m_currentSkipTurn >= m_skipTurnAmount)
                {
                    m_currentTargetPos = DungeonGenerationManager.Instance.GetRandomTargetPosition(transform.position.x, transform.position.y, -m_entityContainer.m_movementController.m_facingDir, false);
                }
                else
                {
                    m_currentTargetPos = DungeonGenerationManager.Instance.GetRandomTargetPosition(transform.position.x, transform.position.y, m_entityContainer.m_movementController.m_facingDir, p_lostPlayer);
                }
                ///TODO: Make this an actual idle target
                break;
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
        if (Vector2.Distance(new Vector3((int)transform.position.x, (int)Mathf.Abs(transform.position.y)), (Vector2)m_currentTargetPos) < m_nodeStoppingDistance || m_path == null)
        {

            NewPath();
        }
        if (m_currentNode == null || m_path == null)
        {
            NewPath();

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
            if (DungeonGenerationManager.Instance.GetEntityCheck(m_currentNode.worldPosition.x, m_currentNode.worldPosition.y))
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
                if (m_currentTarget != null)
                {
                    m_prevTargetPos = (Vector2)m_targetDungeonState.m_currentGridPosition;
                    m_targetDungeonState = m_currentTarget.GetComponent<EntityDungeonState>();
                }
            }
        }
        else
        {
            m_currentTarget = SearchForNewTarget();
            if (m_currentTarget != null)
            {
                m_currentBehaviour = AiBehaviour.Attack;
                m_targetDungeonState = m_currentTarget.GetComponent<EntityDungeonState>();
                m_prevTargetPos = (Vector2)m_targetDungeonState.m_currentGridPosition;
                NewPath();
            }
        }


        if (m_currentTarget == null)
        {
            m_currentBehaviour = AiBehaviour.Idle;
            if (findNewPath)
            {
                Debug.Log("Lost Player");
                m_currentSkipTurn = 0;
                NewPath(true);
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
        if (!m_entityContainer.gameObject.activeSelf || m_entityContainer.m_entityHealth.m_defeated) return false;
        ///If in a room, check the entire room
        if (m_entityContainer.m_dungeonState.m_inRoom)
        {
            if (m_targetDungeonState.m_inRoom && m_targetDungeonState.m_roomIndex == m_entityContainer.m_dungeonState.m_roomIndex) return true;
        }

        if (Mathf.Abs(m_targetDungeonState.m_currentGridPosition.x - transform.position.x) <= 3 && Mathf.Abs(m_targetDungeonState.m_currentGridPosition.y - transform.position.y) < 3)
        {
            return true;
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
                if (PlayerDungeonManager.Instance.m_playerEntityContainer.m_dungeonState.m_inRoom &&
                    PlayerDungeonManager.Instance.m_playerEntityContainer.m_dungeonState.m_roomIndex == m_entityContainer.m_dungeonState.m_roomIndex)
                {
                    //Debug.Log("Player in same room");
                    return PlayerDungeonManager.Instance.m_playerEntityContainer.gameObject;
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

                        GameObject currentTarg = DungeonGenerationManager.Instance.GetEntityCheck((Vector2)transform.position + new Vector2(x, -y));
                        if (currentTarg != null)
                        {
                            EntityContainer newTeam = currentTarg.transform.GetComponent<EntityContainer>();
                            if (newTeam.m_entityTeam.m_currentTeam != m_entityContainer.m_entityTeam.m_currentTeam && newTeam.m_entityTeam.m_currentTeam != EntityTeam.Team.Neutral)
                            {
                                //Debug.Log("Player Located: Radius");
                                return currentTarg;
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

    private Vector2 m_prevTargetPos;
    private void CheckAttackTargetPosition()
    {
        if ((Vector2)m_targetDungeonState.m_currentGridPosition != m_prevTargetPos)
        {
            m_prevTargetPos = (Vector2)m_targetDungeonState.m_currentGridPosition;
            NewPath();
        }
    }
    public bool CanAttack(out int p_attackIndex)
    {
        List<int> possibleAttacks;
        if (m_entityContainer.m_attackController.CanPerformAttack(m_targetDungeonState.m_currentGridPosition, out possibleAttacks))
        {
            p_attackIndex = possibleAttacks[Random.Range(0, possibleAttacks.Count)];
            return true;
        }
        p_attackIndex = 0;
        return false;
    }
    #endregion

}
