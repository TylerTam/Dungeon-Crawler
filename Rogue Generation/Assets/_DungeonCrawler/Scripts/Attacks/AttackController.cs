using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goes on the entity objects
//Allows them to pick which move to use
public class AttackController : MonoBehaviour
{

    public LayerMask m_enemyMask;

    [Header("Attack properties")]
    public List<AttackType_Base> m_attackType;

    public bool m_attackComplete;

    [HideInInspector]
    public bool m_attackAnimComplete = false;



    #region Components On Objects
    [HideInInspector]
    public Animator m_attackAnimator;

    private TurnBasedAgent m_actionAgent;

    private Entity_MovementController m_movementController;
    private Health m_entityHealth;
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
        m_attackAnimator = GetComponent<Animator>();
        m_actionAgent = GetComponent<TurnBasedAgent>();
        m_movementController = GetComponent<Entity_MovementController>();
        m_entityHealth = GetComponent<Health>();
    }

    /// <summary>
    /// Performs the selected attack
    /// </summary>
    public void StartAttack(int p_chosenAttack)
    {

        m_attackComplete = false;
        m_attackAnimComplete = false;
        m_currentAttack = m_attackType[p_chosenAttack];
        m_currentAttack.StartAttack(this, m_movementController.m_facingDir);
        StartCoroutine(CheckAttackAnimation());

    }


    /// <summary>
    /// The ienumerator that runs until the animation is complete
    /// When the animation is complete, create the attack effects
    /// </summary>

    private IEnumerator CheckAttackAnimation()
    {
        while (!m_attackAnimComplete)
        {
            yield return null;
        }
        ChangeToIdleAnimation();

        m_currentAttack.CreateAttackEffects(this);
        StartCoroutine(CheckAllActions());

    }

    //The coroutine that runs to check if all the actions are completed from this attack
    //IE, all the attack anims, all the hurt anims, etc.
    //IE. The thunderbolts, or projectiles as well as the different hurt enemy animations
    private IEnumerator CheckAllActions()
    {
        bool allActionsComplete = false;
        while (!allActionsComplete)
        {

            for (int i = 0; i < m_newActions.Count; i++)
            {
                if (m_newActions[i].AttackAnimComplete())
                {
                    m_newActions.Remove(m_newActions[i]);
                    i -= 1;
                }
                else
                {
                    break;
                }
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
            m_currentAttack.StartAttack(this, m_movementController.m_facingDir);
        }
        else
        {
            m_attackComplete = true;
            m_currentAttackAmount = 0;
        }
    }

    #region Attacked Functionality
    private bool m_attackedComplete = true;
    private Coroutine m_attackedCoroutine;
    private int m_takenDamage;
    Health.DamageType m_takenDamageType;

    public void ApplyAttackedDamage(int p_takenDamage, Health.DamageType p_damageType)
    {
        m_takenDamage = p_takenDamage;
        m_takenDamageType = p_damageType;
    }

    /// <summary>
    /// Inherited from the TurnBasedAction Interface<br/>
    /// Used to determine whether the hurt animation is complete.
    /// </summary>
    public bool AttackAnimComplete()
    {
        if (m_attackedCoroutine == null)
        {
            m_attackedComplete = false;
            m_attackedCoroutine = StartCoroutine(AttackedCoroutine());
        }
        else
        {
            if (m_attackedComplete)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator AttackedCoroutine()
    {
        Debug.Log("Put hurt message here");
        /*
        GameObject damageObject = Instantiate (m_damageObjectPrefab, transform.position, Quaternion.identity);
        damageObject.GetComponent<InWorldUI>().m_uiText.text = -m_takenDamage.ToString();
        */
        yield return m_entityHealth.TakeDamage(m_takenDamage, m_takenDamageType);



        if (m_entityHealth.m_defeated)
        {
            m_entityHealth.Defeated();
        }
        m_attackAnimComplete = true;

    }
    #endregion

    #region Animator Called Events

    public void AttackAnimationCompleted()
    {
        m_attackAnimComplete = true;
    }


    public void ChangeToAttackAnimation()
    {
        m_attackAnimator.SetInteger("FacingX", (int)m_movementController.m_facingDir.x);
        m_attackAnimator.SetInteger("FacingY", (int)m_movementController.m_facingDir.y);

        m_attackAnimator.SetTrigger("IsAttacking");
    }
    public void ChangeToIdleAnimation()
    {
        m_attackAnimator.SetTrigger("IsIdle");
    }



    #endregion
}
