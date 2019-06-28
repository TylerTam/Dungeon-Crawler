using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Input_Base : MonoBehaviour
{
    public Vector2 m_movementDirection;
    public abstract bool CheckInput();
}
