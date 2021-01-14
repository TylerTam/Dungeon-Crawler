using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastInput : MonoBehaviour
{
    public static UIRaycastInput Instance;
    public Vector2 m_input;

    private bool m_draging;

    private enum TouchState { Initial, Movement, Attack }
    private TouchState m_touchState;
    public EntityContainer m_playerEntityContainer;

    [Header("Movement Keys")]
    public List<GameObject> m_movementKeys;
    public List<Vector2> m_movementKeyDirections;

    [Header("Attack Keys")]
    public GameObject m_attackKey;
    public List<GameObject> m_secondaryAttackKeys;


    private GameObject m_currentTapped;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_playerEntityContainer = PlayerDungeonManager.Instance.m_playerEntityContainer;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_touchState = TouchState.Initial;
            m_draging = true;
        }

        if (m_draging)
        {
            PerformDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            FinishDrag();
            m_input = Vector2.zero;
            m_draging = false;
            m_currentTapped = null;
        }
    }



    private void PerformDrag()
    {

        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);



        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject == m_currentTapped)
            {
                return;
            }


            switch (m_touchState)
            {
                case TouchState.Initial:
                    m_currentTapped = res.gameObject;
                    if (res.gameObject == m_attackKey)
                    {
                        m_touchState = TouchState.Attack;
                        return;
                    }

                    if (m_movementKeys.Contains(res.gameObject))
                    {
                        m_touchState = TouchState.Movement;
                        m_input = m_movementKeyDirections[m_movementKeys.IndexOf(res.gameObject)];
                        return;
                    }
                    break;

                case TouchState.Attack:
                    //Do the expanding UI check here
                    break;

                case TouchState.Movement:


                    if (m_movementKeys.Contains(res.gameObject))
                    {
                        m_currentTapped = res.gameObject;
                        m_touchState = TouchState.Movement;
                        m_input = m_movementKeyDirections[m_movementKeys.IndexOf(res.gameObject)];
                        return;
                    }
                    else
                    {
                        m_currentTapped = null;
                    }
                    break;
            }

        }
        if (hitUI.Count == 0)
        {
            switch (m_touchState)
            {
                case TouchState.Initial:
                    break;
                case TouchState.Movement:
                    m_currentTapped = null;
                    m_input = Vector2.zero;
                    break;
                case TouchState.Attack:
                    break;
            }
        }

    }

    private void FinishDrag()
    {
        switch (m_touchState)
        {
            case TouchState.Initial:
                break;
            case TouchState.Attack:
                ///Select the attack here;
                if (m_currentTapped == m_attackKey)
                {
                    m_playerEntityContainer.m_turnBasedAgent.Action_Attack(0);
                }
                break;
            case TouchState.Movement:
                break;
        }
    }
}
