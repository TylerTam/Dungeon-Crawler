using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData_", menuName = "Entity Data", order = 0)]
public class EntityData : ScriptableObject
{
    public string m_entityName;
    public RuntimeAnimatorController m_animController;

    [Header("Stats")]
    public EntityStats m_baseStats;
    public List<EntityStats> m_statIncreasePerLevel;

    [Header("Attacks")]
    public AttackType_Base m_defaultAttack;
    public List<LearnedAttack> m_attacks;

    [System.Serializable]
    public struct LearnedAttack
    {
        public int m_learnedLevel;
        public AttackSlots m_attack;
    }

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


    public int GetCurrentAttackIndex(int p_currentLevel)
    {
        if (m_attacks.Count <= 0) return -1;
        for (int i = 0; i < m_attacks.Count; i++)
        {
            if(m_attacks[i].m_learnedLevel > p_currentLevel)
            {
                return i - 1;
            }
        }
        return m_attacks.Count-1;
    }
}

[System.Serializable]
public class AttackSlots
{
    public AttackType_Base m_attack;
    public int m_maxAmount;
    public int m_attacksLeft;
    public bool m_drainAttackOnUse;
}

