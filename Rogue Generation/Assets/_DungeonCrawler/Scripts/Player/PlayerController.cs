using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolActivationEvent : UnityEvent<bool> { }
public class PlayerController : MonoBehaviour
{

    #region K&M Input
    /*
    [Header("Input Values")]
    public float m_inputSensitivity = .5f;
    public string m_movementAxisX = "Horizontal", m_movementAxisY = "Vertical";
    public float m_inputBufferTime = 0f;
    private bool m_moveCoroutineStarted;
    private Coroutine m_aimCoroutine;
    private WaitForSeconds m_inputDelay;*/
    #endregion

    #region Components
    Input_Base m_inputType;

    private TurnBasedManager m_turnManager;
    private TurnBasedAgent m_agent;
    public LayerMask m_terrainLayer;



    #endregion

    #region UI
    private bool m_mapOpened = false;
    #endregion

    #region Unity Events
    public BoolActivationEvent m_screenCenterTapped = new BoolActivationEvent();
    #endregion

    private void Start()
    {
        m_agent = GetComponent<TurnBasedAgent>();
        m_turnManager = TurnBasedManager.Instance;
        m_inputType = GetComponent<Input_Base>();



    }

    private void Update()
    {
        if (m_agent.m_actionComplete)
        {
            if (m_inputType.CheckInput())
            {
                if (m_inputType.m_movementDirection == Vector2.zero)
                {
                    m_mapOpened = !m_mapOpened;
                    m_screenCenterTapped.Invoke(m_mapOpened);
                }
                else
                {
                    MoveCharacter(m_inputType.m_movementDirection);
                }
            }
        }
    }

    #region M&K Controls
    /// <summary>
    /// Used to move using the K&M
    /// </summary>
    /*private void CheckInput()
    {
        if (Input.GetAxis(m_movementAxisX) != 0 || Input.GetAxis(m_movementAxisY) != 0)
        {
            m_aimCoroutine = StartCoroutine(InputBufferCoroutine());
        }
    }

   

    private IEnumerator InputBufferCoroutine()
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

        MoveCharacter(movement);


        yield return null;
        //print("Movmeent: " + movement + " | transform: " + (Vector2)transform.position + " | Combined: " + (movement + (Vector2)transform.position));
        //print("Circle cast says no");

    }*/
    #endregion

    private void MoveCharacter(Vector2 p_movement)
    {

        Debug.DrawLine(transform.position, (p_movement.normalized * p_movement.magnitude) + (Vector2)transform.position);
        if (!Physics2D.CircleCast(transform.position, .25f, p_movement.normalized, p_movement.magnitude, m_terrainLayer))
        {

            if (p_movement.magnitude != 0)
            {
                m_agent.Action_Move(p_movement + (Vector2)transform.position);
            }
            else
            {
                print("Rotate me");
            }
        }

    }
}
