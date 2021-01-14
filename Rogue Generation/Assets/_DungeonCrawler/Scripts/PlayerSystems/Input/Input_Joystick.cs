using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Joystick : Input_Base
{

    public enum InputType { Keyboard, ScreenJoystick , ScreenRaycast}
    public InputType m_currentInputType;
    public GameObject m_joystickCanvas;
    public Joystick m_joystick;
    public UIRaycastInput m_raycastInput;
    public float m_deadzone;
    public bool m_inputPressed;

    private bool m_skipTurn = false;
    public override void Start()
    {
        base.Start();

        m_raycastInput = UIRaycastInput.Instance;
        if (m_currentInputType != InputType.ScreenJoystick)
        {
            m_joystickCanvas.SetActive(false);
        }


    }
    private void Update()
    {
        if (m_turnAgent.m_actionComplete && m_canPerform)
        {
            CheckInput();
        }
    }
    public override void CheckInput()
    {
        bool movementPressed = false;

        m_movementDirection = GetInput(out movementPressed);
        if (movementPressed)
        {
            m_movementController.MoveCharacter(m_movementDirection);
        }
        else if (m_skipTurn)
        {
            m_skipTurn = false;
            m_movementController.SkipMovement();
        }

    }

    public Vector3 GetInput(out bool p_movementPressed)
    {
        float vert = 0, horiz = 0;
        p_movementPressed = false;
        switch (m_currentInputType)
        {
            case InputType.ScreenJoystick:
                if (Mathf.Abs(m_joystick.Vertical) > m_deadzone)
                {
                    vert = Mathf.Sign(m_joystick.Vertical);
                    p_movementPressed = true;
                }
                if (Mathf.Abs(m_joystick.Horizontal) > m_deadzone)
                {
                    horiz = Mathf.Sign(m_joystick.Horizontal);
                    p_movementPressed = true;
                }
                break;
            case InputType.Keyboard:
                horiz = Input.GetAxis("Horizontal");
                vert = Input.GetAxis("Vertical");
                if (Mathf.Abs(horiz) > 0)
                {
                    horiz = Mathf.Sign(horiz);
                    p_movementPressed = true;
                }
                if (Mathf.Abs(vert) > 0)
                {
                    vert = Mathf.Sign(vert);
                    p_movementPressed = true;
                }
                break;
            case InputType.ScreenRaycast:
                Debug.Log("ScreenRaycast");
                if(Mathf.Abs(m_raycastInput.m_input.magnitude) > 0)
                {
                    p_movementPressed = true;
                }
                horiz = m_raycastInput.m_input.x;
                vert = m_raycastInput.m_input.y;
                break;

        }
        return new Vector3(horiz, vert, 0);
    }
    public override void CenterPressed()
    {
        throw new System.NotImplementedException();
    }

    public override void ButtonPressed(int p_buttonIndex)
    {
        throw new System.NotImplementedException();
    }

    public void SkipTurnButton()
    {
        m_skipTurn = true;
    }
}
