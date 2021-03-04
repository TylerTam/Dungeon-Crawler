using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Input_Base : MonoBehaviour
{
    public static Input_Base Instance;
    public Vector2 m_movementDirection;
    [HideInInspector]
    public Entity_MovementController m_movementController;
    [HideInInspector]
    public TurnBasedAgent m_turnAgent;

    #region Pause Vairables
    [Header("Pause Vairables")]
    public bool m_canPerform = false;

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    public virtual void Start()
    {
        m_movementController = GetComponent<Entity_MovementController>();
        m_turnAgent = GetComponent<TurnBasedAgent>();
    }
    public abstract void CheckInput();

    public abstract void CenterPressed();
    public abstract void ButtonPressed(int p_buttonIndex);
}
