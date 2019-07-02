using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Entity_MovementController : MonoBehaviour
{



    #region Components

    private TurnBasedManager m_turnManager;
    private TurnBasedAgent m_actionAgent;
    public LayerMask m_terrainLayer;
    #endregion





    public LayerMask m_objectsOnFloorLayer;

    private void Start()
    {
        m_actionAgent = GetComponent<TurnBasedAgent>();
        m_turnManager = TurnBasedManager.Instance;
    }

    public void MoveCharacter(Vector2 p_movement)
    {

            Debug.DrawLine(transform.position, (p_movement.normalized * p_movement.magnitude) + (Vector2)transform.position);
            if (!Physics2D.CircleCast(transform.position, .25f, p_movement.normalized, p_movement.magnitude, m_terrainLayer))
            {

                if (p_movement.magnitude != 0)
                {
                    m_actionAgent.Action_Move(p_movement + (Vector2)transform.position);
                }
                else
                {
                    print("Rotate me");
                }
            }
        

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
