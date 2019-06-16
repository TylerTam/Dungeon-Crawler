using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    TurnBasedAgent m_agent;
    #region INput Values

    [Header("Input Values")]
    public float m_inputSensitivity;
    public string m_movementAxisX = "Horizontal", m_movementAxisY = "Vertical";
    public float m_inputBufferTime = .05f;
    bool m_moveCoroutineStarted;
    Coroutine m_aimCoroutine;
    WaitForSeconds m_inputDelay;
    #endregion

    TurnBasedManager m_turnManager;

    public LayerMask m_terrainLayer;


    private void Start()
    {
        m_agent = GetComponent<TurnBasedAgent>();
        m_turnManager = TurnBasedManager.Instance;
        m_inputDelay = new WaitForSeconds(m_inputBufferTime);
    }

    private void Update()
    {
        if (m_agent.m_actionComplete)
        {
            CheckInput();
        }
    }

    void CheckInput()
    {
        if (Input.GetAxis (m_movementAxisX) != 0 || Input.GetAxis(m_movementAxisY) != 0)
        {


                m_aimCoroutine = StartCoroutine(InputBufferCoroutine());
            
            
        }
    }

    IEnumerator InputBufferCoroutine()
    {
        m_moveCoroutineStarted = true;
        
        Vector2 movement = new Vector2(Input.GetAxis(m_movementAxisX), Input.GetAxis(m_movementAxisY));
        movement = new Vector2(Mathf.Abs(movement.x) > m_inputSensitivity ? movement.x : 0, Mathf.Abs(movement.y) > m_inputSensitivity ? movement.y : 0);
        if (movement.x != 0)
        {
            movement = new Vector2(movement.x / (movement.x / m_turnManager.m_moveDistance) * Mathf.Sign(movement.x), movement.y);
        }
        if (movement.y != 0)
        {
            movement = new Vector2(movement.x, movement.y / (movement.y / m_turnManager.m_moveDistance) * Mathf.Sign(movement.y));
        }
        Debug.DrawLine(transform.position, (movement.normalized * movement.magnitude) + (Vector2)transform.position);
        if (!Physics2D.CircleCast(transform.position, .25f, movement.normalized, movement.magnitude,m_terrainLayer))
        {
            
            if (movement.magnitude != 0)
            {
                m_agent.Action_Move(movement + (Vector2)transform.position);
            }
            else
            {
                print("Rotate me");
            }
        }
        
        
        
        yield return null;
        //print("Movmeent: " + movement + " | transform: " + (Vector2)transform.position + " | Combined: " + (movement + (Vector2)transform.position));
        //print("Circle cast says no");

    }
}
