using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Entity_MovementController : MonoBehaviour
{



    #region Components

    private TurnBasedManager m_turnManager;
    private EntityContainer m_entityContainer;

    public bool m_performLayerCheck;
    #endregion

    public Vector2 m_facingDir = new Vector2(0, -1);

    private GameObject m_currentStoodOnObject;

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
        bool rotate = false;
        if (m_performLayerCheck)
        {
            if (DungeonNavigation.Instance.NodeIsNeighbor(transform.position, (Vector2)transform.position + p_movement.normalized))
            {
                GameObject objectInNextCell = DungeonGenerationManager.Instance.GetEntityCheck((Vector2)transform.position + p_movement.normalized);
                if (objectInNextCell == null)
                {
                    UpdateFacingDir(p_movement);

                    if (p_movement.magnitude != 0)
                    {
                        m_entityContainer.m_turnBasedAgent.Action_Move(p_movement + (Vector2)transform.position);
                    }
                }
                else
                {
                    rotate = true;
                }

                ///TODO: Check if the gameobject is friendly, if it is, and this is the player, switch places with the entity

            }
            else
            {
                rotate = true;
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

        if (rotate && p_movement.magnitude > 0)
        {
            RotateCharacterIdle(p_movement);
        }
    }


    public void SkipMovement()
    {
        m_entityContainer.m_turnBasedAgent.Action_SkipTurn();
    }

    public void UpdateFacingDir(Vector2 p_newDir)
    {
        m_facingDir = new Vector2(Mathf.Abs(p_newDir.x) < 0.5 ? 0 : (int)Mathf.Sign(p_newDir.x), Mathf.Abs(p_newDir.y) < 0.5 ? 0 : (int)Mathf.Sign(p_newDir.y));
    }

    public void RotateCharacterIdle(Vector2 p_newDir)
    {
        UpdateFacingDir(p_newDir);
        m_entityContainer.m_entityVisualManager.UpdateFacingDir(p_newDir);
        m_entityContainer.m_entityVisualManager.SwitchToIdleAnimation();
    }

    public void MovementComplete()
    {
        CheckGround();

    }

    private void CheckGround()
    {
        m_currentStoodOnObject = DungeonGenerationManager.Instance.m_interactableGridOccupancy[(int)transform.position.x, Mathf.Abs((int)transform.position.y)];
        if (m_currentStoodOnObject)
        {
            m_currentStoodOnObject.GetComponent<IFloorObject>().Interact(m_entityContainer);
        }
    }
}
