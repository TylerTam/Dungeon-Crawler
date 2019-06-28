using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Android : Input_Base
{
    private Camera m_mainCamera;

    [Range(0f,.5f)]
    public float m_buttonsPercentWidth, m_buttonsPercentHeight;
    private Vector2 m_screenPercent;

    public Joystick m_joystick;

    private void Start()
    {
        m_mainCamera = Camera.main;
        m_screenPercent = new Vector2(m_buttonsPercentWidth * m_mainCamera.pixelWidth, m_buttonsPercentHeight * m_mainCamera.pixelHeight);
        print((m_mainCamera.pixelWidth - m_screenPercent.x) + " | " + (m_mainCamera.pixelHeight - m_screenPercent.y));
    }
    public override bool CheckInput()
    {
        if (Input.touchCount > 0)
        {
            m_movementDirection = Vector2.zero;
            int vertMove = 0, horizMove = 0;
            Touch newTouch = Input.GetTouch(0);
            if (newTouch.position.x > m_mainCamera.pixelWidth - m_screenPercent.x)
            {
                horizMove = 1;
            }
            else if (newTouch.position.x < m_screenPercent.x)
            {
                horizMove = -1;
            }
            if (newTouch.position.y > m_mainCamera.pixelHeight - m_screenPercent.y)
            {
                vertMove = 1;
            }
            else if (newTouch.position.y < m_screenPercent.y)
            {
                vertMove = -1;
            }

            m_movementDirection = new Vector2(horizMove,vertMove);
            return true;

        }
        else
        {
            return false;
        }
    }
}
