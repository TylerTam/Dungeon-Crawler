using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastInput : MonoBehaviour
{
    public static UIRaycastInput Instance;
    public Vector2 m_input;

    private bool m_draging;


    private enum TouchState { Initial, Movement, Attack, Toggle }
    private TouchState m_touchState;



    public EntityContainer m_playerEntityContainer;

    [Header("Movement Keys")]
    public List<GameObject> m_movementKeys;
    public List<Vector2> m_movementKeyDirections;

    public enum MovementState { Moving, Aiming }
    [Header("Aiming / Movement toggle")]
    public MovementState m_currentMovementState;
    public GameObject m_aimingToggleUI;
    public UnityEngine.UI.Text m_aimingText;

    [Header("Attack Keys")]
    public GameObject m_attackKey;
    public GameObject m_attackSelectionGroup;
    public List<SecondaryAttackButtons> m_secondaryAttackKeys;
    public int m_attackIndex;
    public AttackController m_playerAttacks;

    private GameObject m_currentTapped;

    [System.Serializable]
    public class SecondaryAttackButtons
    {
        public GameObject m_attackButtonObject;
        public TMPro.TextMeshProUGUI m_buttonText;
    }
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


    public List<RaycastResult> GetRayHit()
    {
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);
        return hitUI;
    }
    private void PerformDrag()
    {

        List<RaycastResult> hitUI = GetRayHit();



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
                    if (res.gameObject.transform.parent.gameObject == m_attackKey)
                    {
                        m_touchState = TouchState.Attack;
                        m_attackSelectionGroup.SetActive(true);

                        m_attackKey.SetActive(false);
                        for (int i = 0; i < m_secondaryAttackKeys.Count; i++)
                        {
                            m_secondaryAttackKeys[i].m_attackButtonObject.SetActive(false);
                        }

                        for (int i = 0; i < m_playerAttacks.m_allAttacks.Count; i++)
                        {
                            m_secondaryAttackKeys[i].m_attackButtonObject.SetActive(true);
                            m_secondaryAttackKeys[i].m_buttonText.text = m_playerAttacks.m_allAttacks[i].m_attack.m_attackName;
                        }
                        return;
                    }

                    if(res.gameObject == m_aimingToggleUI)
                    {
                        m_touchState = TouchState.Toggle;
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

                List<RaycastResult> rayHit = GetRayHit();

                bool hitAttack = false;
                int attackIndex = 0;
                foreach(RaycastResult hit in rayHit)
                {
                    foreach(SecondaryAttackButtons attacks in m_secondaryAttackKeys)
                    {
                        if(attacks.m_attackButtonObject == hit.gameObject.transform.parent.gameObject)
                        {
                            attackIndex = m_secondaryAttackKeys.IndexOf(attacks);
                            hitAttack = true;
                            break;
                        }
                    }
                    if (hitAttack)
                    {
                        break;
                    }
                }

                m_attackSelectionGroup.SetActive(false);
                m_attackKey.SetActive(true);

                if (hitAttack)
                {
                    Debug.Log("Hit Index: " + attackIndex);
                    if (m_playerEntityContainer.m_attackController.CanUseAttack(attackIndex))
                    {
                        m_playerEntityContainer.m_turnBasedAgent.Action_Attack(attackIndex);
                    }
                }
                break;
            case TouchState.Movement:
                break;
            case TouchState.Toggle:
                if (m_currentMovementState == MovementState.Aiming)
                {
                    m_currentMovementState = MovementState.Moving;
                    m_aimingText.text = "Moving";
                }
                else if (m_currentMovementState == MovementState.Moving)
                {
                    m_currentMovementState = MovementState.Aiming;
                    m_aimingText.text = "Aiming";
                }
                break;
        }
    }
}
