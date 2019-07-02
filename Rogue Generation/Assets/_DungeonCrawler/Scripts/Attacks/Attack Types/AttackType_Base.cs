using UnityEngine;


/// <summary>
/// This is used to determine the logic of an attack
/// Any inherited scripts can change the "CreateAttackEffects" so that the weapon effects suit that attack
/// ie. Thunderbolts spawn, or projectiles
/// </summary>
[CreateAssetMenu(fileName = "Attack Type Base", menuName = "Attack Types / Basic", order = 0)]
public class AttackType_Base : ScriptableObject
{

    //The type of attack, ie, ranged, projectile, adjactent, full room, etc.
    public AttackHitArea_Base m_attackFunction;


    //How many attack prefabs to spawn. IE. a multihit move, like furyswipes would have more than one
    public int m_attackSpawnAmount = 0;


    [Header("Attack Animation")]
    public RuntimeAnimatorController m_attackAnimationController;




    /// <summary>
    /// Starts the attack animation, and also passes any enemies that are hit
    /// Called from the attack Controller
    /// </summary>
    
    public virtual void StartAttack(AttackController p_currentAttacker)
    {
        p_currentAttacker.m_attackAnimator.runtimeAnimatorController = m_attackAnimationController;
        p_currentAttacker.ChangeToAttackAnimation();

        p_currentAttacker.m_newActions.Clear();

        //Pass the hitbox
        p_currentAttacker.m_newActions = m_attackFunction.CommencedActions(p_currentAttacker);
        
    }


    //Create the weapons effects. This can be changed in the inherited scripts to better suit the attack.
    public virtual void CreateAttackEffects(AttackController p_attackController)
    {
        Debug.Log("Create weapon effects here");
    }

    

    


}
