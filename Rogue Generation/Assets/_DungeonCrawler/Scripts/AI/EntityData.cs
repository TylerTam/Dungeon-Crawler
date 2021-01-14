using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData_", menuName = "Entity Data", order = 0)]
public class EntityData : ScriptableObject
{
    public RuntimeAnimatorController m_animController;

    [Header("Sprites")]

    [Tooltip("There should be 16 attack sprites")]
    public List<Sprite> m_attackSprites;

    [Tooltip("There should be 16 hurt sprites")]
    public List<Sprite> m_hurtSprites;

    [Tooltip("Should be 16 idle sprites")]
    public List<Sprite> m_idleSprites;

    [Tooltip("There should be 24 movement Sprites")]
    public List<Sprite> m_movementSprites;

    [Tooltip("There should be 16 sleep sprites")]
    public List<Sprite> m_sleepSprites;

    [Tooltip("There should be 16 special attack sprites")]
    public List<Sprite> m_specialAttackSprites;

}
