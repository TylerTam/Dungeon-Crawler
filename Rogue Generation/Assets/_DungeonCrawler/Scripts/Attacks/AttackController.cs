using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goes on the entity objects
//Allows them to pick which move to use
public class AttackController : MonoBehaviour
{
    public LayerMask m_enemyMask;
    public Vector2 m_facingDir;

    [Header("Attack properties")]
    public List<AttackType_Base> m_attackType;

    public bool m_attackComplete;

    [HideInInspector]
    public bool m_attackAnimComplete = false;



    #region Components On Objects
    [HideInInspector]
    public Animator m_attackAnimator;

    private TurnBasedAgent m_actionAgent;
    #endregion

    #region Attack type management
    private AttackType_Base m_currentAttack;
    private int m_currentAttackAmount;
    #endregion

    #region Turn management
    //The actions that are spawned from the attack
    //IE. the hurt animations, the actual attack animations (if prefabs are spawned ie thunderbolts)
    [HideInInspector]
    public List<TurnBasedAction> m_newActions = new List<TurnBasedAction>();
    #endregion


    private void Start()
    {
        m_attackAnimator = GetComponent<Animator>();
        m_actionAgent = GetComponent<TurnBasedAgent>();
    }

    /// <summary>
    /// Performs the selected attack
    /// </summary>
    public void StartAttack(int p_chosenAttack)
    {


            m_attackComplete = false;
            m_attackAnimComplete = false;
            m_currentAttack = m_attackType[p_chosenAttack];
            m_currentAttack.StartAttack(this);
            print("Attack Started");

            StartCoroutine(CheckAttackAnimation());
        
    }



    /// <summary>
    /// The ienumerator that runs until the animation is complete
    /// When the animation is complete, create the attack effects
    /// </summary>

    IEnumerator CheckAttackAnimation()
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
                if (m_newActions[i].IComplete())
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
            m_currentAttack.StartAttack(this);
        }
        else
        {
            m_attackComplete = true;
            m_currentAttackAmount = 0;
        }
    }



    #region Animator Called Events

    public void AttackAnimationCompleted()
    {
        m_attackAnimComplete = true;
    }


    public void ChangeToAttackAnimation()
    {
        m_attackAnimator.SetTrigger("IsAttacking");
    }
    public void ChangeToIdleAnimation()
    {
        m_attackAnimator.SetTrigger("IsIdle");
    }
    #endregion
}
