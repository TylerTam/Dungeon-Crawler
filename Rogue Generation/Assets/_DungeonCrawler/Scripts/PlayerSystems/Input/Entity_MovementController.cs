using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Entity_MovementController : MonoBehaviour
{



    #region Components

    private TurnBasedManager m_turnManager;
    private EntityContainer m_entityContainer;

    public LayerMask m_terrainLayer;
    public LayerMask m_diagonalTerrainLayer;
    public bool m_performLayerCheck;
    #endregion

    public Vector2 m_facingDir = new Vector2(0, -1);




    public LayerMask m_objectsOnFloorLayer;

    private void Start()
    {

        m_entityContainer = GetComponent<EntityContainer>();
        m_turnManager = TurnBasedManager.Instance;
    }
    private void OnEnable()
    {
        m_facingDir = new Vector2(0, -1);
    }

    /// <summary>
    /// Only the player utilizes this function.<br/>
    /// The AI uses it's own movement function
    /// </summary>
    public void MoveCharacter(Vector2 p_movement)
    {
        if (m_performLayerCheck)
        {
            if (!Physics2D.Raycast(transform.position, p_movement.normalized, p_movement.magnitude, m_terrainLayer))
            {
                if (!Physics2D.Raycast(transform.position, p_movement.normalized, p_movement.magnitude, m_diagonalTerrainLayer))
                {
                    UpdateFacingDir(p_movement);
                    if (p_movement.magnitude != 0)
                    {
                        m_entityContainer.m_turnBasedAgent.Action_Move(p_movement + (Vector2)transform.position);
                    }
                }
            }
        }
        else
        {
            if (p_movement.magnitude != 0)
            {
                UpdateFacingDir(p_movement);
                m_entityContainer.m_turnBasedAgent.Action_Move(p_movement + (Vector2)transform.position);
            }
        }
    }

    public void SkipMovement()
    {
        m_entityContainer.m_turnBasedAgent.Action_SkipTurn();
    }

    public void UpdateFacingDir(Vector2 p_newDir)
    {
        m_facingDir = p_newDir;
    }
    public void MovementComplete()
    {
        CheckGround();

    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward, 100f, m_objectsOnFloorLayer);
        if (hit)
        {

            hit.transform.GetComponent<IFloorObject>().Interact();

        }

    }
}
