using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ai Type", menuName = "AI/AI Type", order = 0)]
public class AIType_Base : ScriptableObject
{
    public Sprite m_aiBaseSprite;
    public RuntimeAnimatorController m_animController;
    public LayerMask m_detectionBlockingMask;
    
}
