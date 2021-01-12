﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Entity_MovementController : MonoBehaviour
{



    #region Components

    private TurnBasedManager m_turnManager;
    private TurnBasedAgent m_actionAgent;
    public LayerMask m_terrainLayer;
    public LayerMask m_diagonalTerrainLayer;
    public bool m_performLayerCheck;
    #endregion





    public LayerMask m_objectsOnFloorLayer;

    private void Start()
    {
        m_actionAgent = GetComponent<TurnBasedAgent>();
        m_turnManager = TurnBasedManager.Instance;
    }

    public void MoveCharacter(Vector2 p_movement)
    {
        if (m_performLayerCheck)
        {
            if (!Physics2D.Raycast(transform.position, p_movement.normalized, p_movement.magnitude, m_terrainLayer))
            {
                if (!Physics2D.Raycast(transform.position, p_movement.normalized, p_movement.magnitude, m_diagonalTerrainLayer))
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
        }
        else
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
