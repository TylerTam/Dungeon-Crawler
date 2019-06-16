using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//The prefab spawned by the entities
public abstract class AttackTypeContainer : MonoBehaviour
{
    public AttackType_Base m_attackFunction;

    AttackController m_currentAttacker;
    List<TurnBasedAction> m_newActions;

    bool m_attackComplete = false;

    public int m_attackSpawnAmount = 0;
    int m_currentAttackAmount;


    //Ends the attack, if it is compelte
    public bool AttackComplete()
    {
        return m_attackComplete;
    }

    //Gets all the enemies that are hit by this attack
    public virtual void StartAttack(AttackController p_currentAttacker)
    {
        m_attackComplete = false;
        m_currentAttacker = p_currentAttacker;
        m_newActions.Clear();
        m_newActions = m_attackFunction.CommencedActions();
        StartCoroutine(CheckAttackAnimation());
    }

    public abstract bool AttackAnimComplete();
    IEnumerator CheckAttackAnimation()
    {
        while (!AttackAnimComplete())
        {
            yield return null;
        }
        StartCoroutine(CheckAllActions());

    }

    //The coroutine that runs to check if all the actions are completed from this attack
    //IE, all the attack anims, all the hurt anims, etc.
    IEnumerator CheckAllActions()
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
            if(m_newActions.Count == 0)
            {
                allActionsComplete = true;
            }
        }

        //Allow for multiple attacks, IE Fury attack
        if (m_currentAttackAmount < m_attackSpawnAmount)
        {
            m_currentAttackAmount++;
            StartAttack(m_currentAttacker);
        }
        else
        {
            m_attackComplete = true;
            m_currentAttackAmount = 0;
        }
    }


}
