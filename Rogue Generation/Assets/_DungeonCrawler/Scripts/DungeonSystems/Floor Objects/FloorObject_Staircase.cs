using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObject_Staircase : MonoBehaviour, IFloorObject
{
    public static FloorObject_Staircase Instance;
    public LayerMask m_playerLayer;
    private void Awake()
    {
        Instance = this;
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
                DungeonGenerationManager.Instance.NewFloor();
            }
            

        }
    }
}
