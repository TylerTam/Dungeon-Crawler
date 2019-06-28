using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goes on the entity objects
public class AttackController : MonoBehaviour
{
    List<AttackTypeContainer> m_attackType;
    int m_currentChosenAttack;


    public bool m_attackComplete;

    public void StartAttack(int p_chosenAttack)
    {
        m_attackType[p_chosenAttack].StartAttack(this);
        m_currentChosenAttack = p_chosenAttack;
        m_attackComplete = false;
        StartCoroutine(CheckActionCompletion());
    }

    //Runs until the attack is complete. When it is complete, gameplay continues
    IEnumerator CheckActionCompletion()
    {
        while (!m_attackType[m_currentChosenAttack].AttackComplete())
        {
            yield return null;
        }
        m_attackComplete = true;

    }
}
