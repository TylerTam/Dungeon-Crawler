using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject_Staircase : MonoBehaviour, IFloorObject
{
    public LayerMask m_playerLayer;
    

    DungeonManager m_dungeonGenerator;

    private void Start()
    {
        m_dungeonGenerator = DungeonManager.Instance;
    }
    public void Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward, 100f, m_playerLayer);
        if (hit)
        {
            if (hit.transform.GetComponent<Input_Base>() != null)
            {
                Input_Base playerInput = hit.transform.GetComponent<Input_Base>();
                playerInput.m_canPerform = false;
                m_dungeonGenerator.NewFloor();
            }
            

        }
    }
}
