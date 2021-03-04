using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goes on the entity objects
//Allows them to pick which move to use
public class AttackController : MonoBehaviour
{


    [Header("Unique Attack properties")]
    public List<AttackSlots> m_allAttacks;



    [HideInInspector]
    public bool m_attackAnimComplete = false;



    #region Components On Objects
    [HideInInspector]
    public Animator m_attackAnimator;

    public EntityContainer m_entityContainer;
    #endregion

    #region Attack type management
    private AttackType_Base m_currentAttack;
    private int m_currentAttackAmount;
    #endregion

    #region Turn management
    //The actions that are spawned from the attack
    //IE. the hurt animations, the actual attack animations (if prefabs are spawned ie thunderbolts)
    [HideInInspector]
    public List<AttackController> m_newActions = new List<AttackController>();
    #endregion


    private void Start()
    {
        m_entityContainer = GetComponent<EntityContainer>();
        m_attackAnimator = GetComponent<Animator>();
    }

    public void InitializeAttack(EntityData p_entityData, int p_level)
    {
        m_allAttacks.Clear();

        AttackSlots defaultAttack = new AttackSlots();
        defaultAttack.m_attack = p_entityData.m_defaultAttack;
        defaultAttack.m_drainAttackOnUse = false;
        defaultAttack.m_attacksLeft = defaultAttack.m_maxAmount = 100;
        m_allAttacks.Add(defaultAttack);


        int attackIndexOfCurrentLevel = p_entityData.GetCurrentAttackIndex(p_level);
        if (attackIndexOfCurrentLevel < 0) return;

        for (int i = 0; i < 4; i++)
        {
            if (attackIndexOfCurrentLevel - i < 0) return;
            AttackSlots newSlot = new AttackSlots();
            newSlot.m_attack = p_entityData.m_attacks[attackIndexOfCurrentLevel - i].m_attack.m_attack;
            newSlot.m_attacksLeft = newSlot.m_maxAmount = p_entityData.m_attacks[attackIndexOfCurrentLevel - i].m_attack.m_maxAmount;
            newSlot.m_drainAttackOnUse = p_entityData.m_attacks[attackIndexOfCurrentLevel - i].m_attack.m_drainAttackOnUse;
            m_allAttacks.Add(newSlot);
        }
    }

    public bool CanPerformAttack(Vector3 p_targetPos, out List<int> p_attack)
    {
        p_attack = new List<int>();
        foreach (AttackSlots attack in m_allAttacks)
        {
            if (attack.m_attacksLeft <= 0) continue;

            if (attack.m_attack.m_attackDetection.IsWithinRange(transform.position, p_targetPos))
            {
                p_attack.Add(m_allAttacks.IndexOf(attack));
            }
        }

        if (p_attack.Count > 0)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Performs the selected attack
    /// </summary>
    public IEnumerator StartAttack(int p_chosenAttack)
    {
        m_attackAnimComplete = false;
        m_currentAttack = m_allAttacks[p_chosenAttack].m_attack;

        if (m_allAttacks[p_chosenAttack].m_drainAttackOnUse)
        {
            m_allAttacks[p_chosenAttack].m_attacksLeft--;
        }
        m_currentAttack.StartAttack(this, m_entityContainer.m_movementController.m_facingDir);

        Debug.Log("Attack Anim Start");
        while (!m_attackAnimComplete)
        {
            yield return null;
        }
        Debug.Log("Attack Anim Finished");
        ChangeToIdleAnimation();

        m_currentAttack.CreateAttackEffects(this);



        #region Perform All actions of this attack
        int damageAmount = m_currentAttack.m_baseDamage;
        if (m_currentAttack.m_attackType == AttackType_Base.AttackType.PhysicalAttack)
        {
            damageAmount += m_entityContainer.m_runtimeStats.m_currentStats.m_attack;
        }
        else if (m_currentAttack.m_attackType == AttackType_Base.AttackType.Magic)
        {
            damageAmount += m_entityContainer.m_runtimeStats.m_currentStats.m_magic;
        }
        Debug.Log("Entity: " + gameObject.name + " | Damage: " + damageAmount + " | Damage Type: " + m_currentAttack.m_attackType);

        bool allActionsComplete = false;
        while (!allActionsComplete)
        {
            for (int i = 0; i < m_newActions.Count; i++)
            {
                yield return StartCoroutine(m_newActions[i].AttackAnimComplete(damageAmount, m_currentAttack.m_attackType));

                m_newActions.Remove(m_newActions[i]);
                i -= 1;

            }
            yield return null;
            if (m_newActions.Count == 0)
            {
                allActionsComplete = true;
            }
        }
        //Allow for multiple attacks, IE Fury attack
        if (m_currentAttackAmount < m_currentAttack.m_attackSpawnAmount)
        {
            m_currentAttackAmount++;
            m_currentAttack.StartAttack(this, m_entityContainer.m_movementController.m_facingDir);
        }
        else
        {
            m_currentAttackAmount = 0;
        }
        #endregion

    }



    #region Attacked Functionality
    /// <summary>
    /// Inherited from the TurnBasedAction Interface<br/>
    /// Used to determine whether the hurt animation is complete.
    /// </summary>
    public IEnumerator AttackAnimComplete(int p_damageAmount, AttackType_Base.AttackType p_attackType)
    {
        //TODO: Put hurt message here
        //        Debug.Log("Put hurt message here");
        /*
        GameObject damageObject = Instantiate (m_damageObjectPrefab, transform.position, Quaternion.identity);
        damageObject.GetComponent<InWorldUI>().m_uiText.text = -m_takenDamage.ToString();
        */
        yield return m_entityContainer.m_entityHealth.TakeDamage(p_damageAmount, p_attackType);

        if (m_entityContainer.m_entityHealth.m_defeated)
        {
            m_entityContainer.m_entityHealth.Defeated();
        }
    }

    #endregion

    #region Animator Called Events

    public void AttackAnimationCompleted()
    {
        m_attackAnimComplete = true;
        m_entityContainer.m_entityVisualManager.SwitchToIdleAnimation();
    }


    public void ChangeToAttackAnimation(AttackType_Base.AttackType p_attackType)
    {
        switch (p_attackType)
        {
            case AttackType_Base.AttackType.PhysicalAttack:
                m_entityContainer.m_entityVisualManager.SwitchToPhysicalAttackAnimation();
                break;
            case AttackType_Base.AttackType.Magic:
                m_entityContainer.m_entityVisualManager.SwitchToSpecialAttackAnimation();
                break;
        }

        m_attackAnimator.SetInteger("FacingX", (int)m_entityContainer.m_movementController.m_facingDir.x);
        m_attackAnimator.SetInteger("FacingY", (int)m_entityContainer.m_movementController.m_facingDir.y);

        m_attackAnimator.SetTrigger("IsAttacking");
    }
    public void ChangeToIdleAnimation()
    {
        m_attackAnimator.SetTrigger("IsIdle");
    }



    #endregion
}
