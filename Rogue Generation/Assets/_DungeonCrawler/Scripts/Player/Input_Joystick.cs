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
        m_inputPressed = CheckInput();   
    }
    public override bool CheckInput()
    {
        bool inputChecked = false;
        float vert = 0, horiz = 0;
        if (Mathf.Abs(m_joystick.Vertical) > m_deadzone)
        {
            vert = Mathf.Sign(m_joystick.Vertical);
            inputChecked = true;
        }
        if (Mathf.Abs(m_joystick.Horizontal) > m_deadzone)
        {
            horiz = Mathf.Sign(m_joystick.Horizontal);
            inputChecked = true;
        }

        m_movementDirection = new Vector2(horiz, vert);

        return inputChecked;

    }
}
