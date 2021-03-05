using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This is used to determine the logic of an attack
/// Any inherited scripts can change the "CreateAttackEffects" so that the weapon effects suit that attack
/// ie. Thunderbolts spawn, or projectiles
/// </summary>
[CreateAssetMenu(fileName = "Attack_Type_Base_", menuName = "Attacks/AttackTypes/Basic", order = 0)]
public class AttackType_Base : ScriptableObject
{
    public enum AttackType { PhysicalAttack, Magic }
    public AttackType m_attackType;
    public int m_baseDamage;
    [Range(0, 1)]
    public float m_changeOfAttack;

    //The type of attack, ie, ranged, projectile, adjactent, full room, etc.
    public AttackHitArea_Base m_attackDetection;


    public int m_range;
    public Vector2Int m_explosionRange;

    //How many attack prefabs to spawn. IE. a multihit move, like furyswipes would have more than one
    public int m_attackSpawnAmount = 0;



    [Header("Attack Animation")]
    public RuntimeAnimatorController m_attackAnimationController;



    public bool IsWithinRange(Vector2 p_attackerPos, Vector2 p_targetPos)
    {
        if (m_attackDetection.IsWithinRange(p_attackerPos, p_targetPos, m_range))
        {
            if (Random.Range(0, 1f) < m_changeOfAttack)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Starts the attack animation, and also passes any enemies that are hit
    /// Called from the attack Controller
    /// </summary>

    public virtual void StartAttack(AttackController p_currentAttacker, Vector2 p_facingDir)
    {
        p_currentAttacker.m_attackAnimator.runtimeAnimatorController = m_attackAnimationController;
        p_currentAttacker.ChangeToAttackAnimation(m_attackType);

        p_currentAttacker.m_newActions.Clear();

        //Pass the hitbox
        p_currentAttacker.m_newActions = m_attackDetection.CommencedActions(p_currentAttacker, p_facingDir, m_range, m_explosionRange);

    }


    //Create the weapons effects. This can be changed in the inherited scripts to better suit the attack.
    public virtual WeaponEffect_Base CreateAttackEffects(AttackController p_attackerController, List<AttackController> p_affectedControllers, Vector2 p_facingDir)
    {
        return null;
    }


    public virtual IEnumerator CreateIndividualAttackEffect(Vector3 p_attackEffectPosition)
    {
        yield return null;
    }






}
