using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRuntimeStats : MonoBehaviour
{
    public EntityStats m_currentStats;
    public EntityData m_currentData;
    public EntityContainer m_entContainer;
    public int m_currentLevel;

    public void AssignData(EntityContainer p_container, EntityData p_currentData, int p_currentLevel)
    {
        m_currentLevel = p_currentLevel;

        m_currentData = p_currentData;


        m_currentStats = new EntityStats(p_currentData.m_baseStats);
        for (int i = 0; i < m_currentLevel - 1; i++)
        {
            if (i == m_currentData.m_statIncreasePerLevel.Count) break;
            m_currentStats.IncreaseStatsByLevel(m_currentData.m_statIncreasePerLevel[i]);
        }
    }

}

[System.Serializable]
public class EntityStats
{

    public EntityStats(EntityStats p_assign)
    {
        m_health = p_assign.m_health;
        m_attack = p_assign.m_attack;
        m_defense = p_assign.m_defense;
        m_magic = p_assign.m_magic;
        m_resistance = p_assign.m_resistance;
    }
    public int m_health;
    public int m_attack;
    public int m_defense;
    public int m_magic;
    public int m_resistance;

    public void IncreaseStatsByLevel(EntityStats p_increaseAmount)
    {
        m_health += p_increaseAmount.m_health;
        m_attack += p_increaseAmount.m_attack;
        m_defense += p_increaseAmount.m_defense;
        m_magic += p_increaseAmount.m_magic;
        m_resistance += p_increaseAmount.m_resistance;
    }

}
