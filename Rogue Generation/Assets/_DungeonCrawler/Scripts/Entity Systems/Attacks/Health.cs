using System.Collections;
using UnityEngine;

//public class HealthEvent : UnityEngine.Events.UnityEvent { }
public class Health : MonoBehaviour
{
    public bool m_defeated;
    public int m_maxHealth;
    public int m_currentHealth;

    private EntityContainer m_entityContainer;

    public bool m_damageAnimComplete;
    //public HealthEvent m_onDamageTaken, m_onDefeatedEvent;


    public void InitializeData(EntityContainer p_cont, EntityRuntimeStats p_runtimeStats)
    {
        m_entityContainer = p_cont;
        m_maxHealth = p_runtimeStats.m_currentStats.m_health;
        m_currentHealth = m_maxHealth;
        Respawn();
    }
    public void Respawn()
    {
        m_currentHealth = m_maxHealth;
        m_defeated = false;
    }

    public int CalculateDamage(int p_damageTaken, AttackType_Base.AttackType p_damageType)
    {
        int damageTaken = p_damageTaken;

        switch (p_damageType)
        {
            case AttackType_Base.AttackType.PhysicalAttack:
                damageTaken -= m_entityContainer.m_runtimeStats.m_currentStats.m_defense;
                break;
            case AttackType_Base.AttackType.Magic:
                damageTaken -= m_entityContainer.m_runtimeStats.m_currentStats.m_resistance;
                break;
        }
        if (damageTaken <= 0)
        {
            damageTaken = 1;
        }
        return damageTaken;
    }
    public IEnumerator TakeDamage(int p_damageTaken, AttackType_Base.AttackType p_damageType)
    {
        m_damageAnimComplete = false;
        
        m_currentHealth -= CalculateDamage(p_damageTaken, p_damageType);
        if (m_currentHealth <= 0)
        {
            m_entityContainer.m_entityVisualManager.SwitchToDefeatedAnimation();
            m_defeated = true;
        }
        else
        {
            m_entityContainer.m_entityVisualManager.SwitchToHurtAnimation();
        }
        while (!m_damageAnimComplete)
        {
            yield return null;
        }

        if (m_defeated)
        {
            TurnBasedManager.Instance.AgentDefeated(m_entityContainer.m_turnBasedAgent);
        }
    }

    public void HurtAnimationCompleted()
    {
        m_damageAnimComplete = true;
    }
    public void Defeated()
    {
        m_entityContainer.m_turnBasedAgent.RemoveCellAttendence();
        ObjectPooler.instance.ReturnToPool(gameObject);

    }


}

