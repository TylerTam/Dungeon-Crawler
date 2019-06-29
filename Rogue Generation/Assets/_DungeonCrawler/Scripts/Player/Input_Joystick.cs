using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Joystick : Input_Base
{
    public Joystick m_joystick;
    public float m_deadzone;
    public bool m_inputPressed;
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

        float vert = 0, horiz = 0;
        if (Mathf.Abs(m_joystick.Vertical) > m_deadzone)
        {
            vert = Mathf.Sign(m_joystick.Vertical);
            movementPressed = true;
        }
        if (Mathf.Abs(m_joystick.Horizontal) > m_deadzone)
        {
            horiz = Mathf.Sign(m_joystick.Horizontal);
            movementPressed = true;
        }
        m_movementDirection = new Vector2(horiz, vert);

        if (movementPressed)
        {
            m_movementController.MoveCharacter(m_movementDirection);
        }
        
        

    }

    public override void CenterPressed()
    {
        throw new System.NotImplementedException();
    }

    public override void ButtonPressed(int p_buttonIndex)
    {
        throw new System.NotImplementedException();
    }
}
