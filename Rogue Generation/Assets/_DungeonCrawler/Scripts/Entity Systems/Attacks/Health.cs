using System.Collections;
using UnityEngine;

//public class HealthEvent : UnityEngine.Events.UnityEvent { }
public class Health : MonoBehaviour
{
    public enum DamageType { Def, Res, Poision }
    public bool m_defeated;
    public int m_maxHealth;
    public int m_currentHealth;

    private EntityContainer m_entityContainer;


    public bool m_damageAnimComplete;
    //public HealthEvent m_onDamageTaken, m_onDefeatedEvent;

    private void Start()
    {
        m_entityContainer = GetComponent<EntityContainer>();
    }

    private void OnEnable()
    {
        Respawn();
    }
    public void Respawn()
    {
        m_currentHealth = m_maxHealth;
        m_defeated = false;
    }
    public IEnumerator TakeDamage(int p_damageTaken, DamageType p_damageType)
    {
        m_damageAnimComplete = false;
        m_currentHealth -= p_damageTaken;
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
    }

    public void HurtAnimationCompleted()
    {
        if (m_defeated)
        {
            TurnBasedManager.Instance.AgentDefeated(m_entityContainer.m_turnBasedAgent);
        }
        m_damageAnimComplete = true;
    }
    public void Defeated()
    {
        ObjectPooler.instance.ReturnToPool(gameObject);
    }


}

