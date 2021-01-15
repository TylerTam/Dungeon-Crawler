using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisualEvents : UnityEngine.Events.UnityEvent { }
public class EntityVisualManager : MonoBehaviour
{
    public SpriteRenderer m_sRend;
    public Animator m_spriteAnimator;
    
    public EntityData m_entityData;
    public Health m_entityHealth;
    public enum CurrentState { Movement,Hurt,Idle,Attack,SpecialAttack,Sleep,Defeated}
    public CurrentState m_currentAnimationState;
    public Vector2 m_currentFacingDir;
    public Vector2 m_currentMovementDir;

    [System.Serializable]
    public struct VisualEvents
    {
        public EntityVisualEvents m_hurtAnimComplete;
        public EntityVisualEvents m_defeatedAnimComplete;
    }

    private void Start()
    {
        m_entityHealth = transform.parent.GetComponent<Health>();
    }
    public void AssignEntityData(EntityData p_data)
    {
        m_entityData = p_data;
        m_spriteAnimator.runtimeAnimatorController = p_data.m_animController;
        m_spriteAnimator.enabled = true;
        SwitchToIdleAnimation();
        UpdateFacingDir(new Vector2(0, -1));

    }

    public void DisableAnimator()
    {
        m_spriteAnimator.enabled = false;
    }

    private void OnDisable()
    {
        m_sRend.color = new Color(1, 1, 1, 1);
    }
    #region Animation Switch Functions
    public void UpdateFacingDir(Vector2 p_facingDir)
    {
        m_spriteAnimator.SetInteger("FacingX", (int)p_facingDir.x);
        m_spriteAnimator.SetInteger("FacingY", (int)p_facingDir.y);
        m_currentFacingDir = p_facingDir;
    }

    public void SwitchToIdleAnimation()
    {
        if (m_currentAnimationState != CurrentState.Idle || m_currentFacingDir != m_currentMovementDir)
        {
            m_currentMovementDir = m_currentFacingDir;
            m_currentAnimationState = CurrentState.Idle;
            m_spriteAnimator.SetTrigger("Idle");
        }
    }

    public void SwitchToMovingAnimation()
    {
        if (m_currentAnimationState != CurrentState.Movement || m_currentFacingDir != m_currentMovementDir)
        {
            m_currentAnimationState = CurrentState.Movement;
            m_currentMovementDir = m_currentFacingDir;
            m_spriteAnimator.SetTrigger("Movement");
        }
    }

    #region Attack Related Animations

    public void SwitchToPhysicalAttackAnimation()
    {
        m_currentAnimationState = CurrentState.Attack;
        m_spriteAnimator.SetTrigger("Attack");
    }

    public void SwitchToSpecialAttackAnimation()
    {
        m_currentAnimationState = CurrentState.SpecialAttack;
        m_spriteAnimator.SetTrigger("SpecialAttack");
    }
    #endregion

    #region Attacked Animations
    public void SwitchToHurtAnimation()
    {
        m_currentAnimationState = CurrentState.Hurt;
        m_spriteAnimator.SetTrigger("Hurt");
    }

    public void HurtAnimationComplete()
    {
        
        Debug.Log("Hurt Animation Complete");
    }

    public void SwitchToDefeatedAnimation()
    {
        m_currentAnimationState = CurrentState.Defeated;
        m_spriteAnimator.SetTrigger("Defeated");
    }
    public void DefeatedAnimationComplete()
    {
        Debug.Log("Defeated Animation Complete");
    }
    #endregion

    #endregion


    #region Animation Sprites



    public void ChangeAttackSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_attackSprites[p_index];
        //Debug.Log("Change Attack sprite: " + p_index);
    }


    public void ChangeHurtSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_hurtSprites[p_index];
        //Debug.Log("Change Hurt sprite: " + p_index);
    }

    public void HurtAnimComplete()
    {
        m_entityHealth.HurtAnimationCompleted();
    }

    public void DefeatAnimComplete()
    {
        DisableAnimator();
        m_entityHealth.HurtAnimationCompleted();
    }


    public void ChangeIdleSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_idleSprites[p_index];
        //Debug.Log("Change Idle sprite: " + p_index);
    }


    private bool m_showOtherLeg;
    public void ChangeMovementPausedSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_movementSprites[p_index];
    }
    public void ChangeMovementSprite(int p_index)
    {
        if (m_showOtherLeg)
        {
            m_showOtherLeg = false;
            m_sRend.sprite = m_entityData.m_movementSprites[p_index + 1];
        }
        else
        {
            m_showOtherLeg = true;
            m_sRend.sprite = m_entityData.m_movementSprites[p_index];
        }
        //Debug.Log("Change Move sprite: " + p_index);
    }



    public void ChangeSleepSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_sleepSprites[p_index];
        //Debug.Log("Change Sleep sprite: " + p_index);
    }



    public void ChangeSpecialAttackSprite(int p_index)
    {
        m_sRend.sprite = m_entityData.m_specialAttackSprites[p_index];
        //Debug.Log("Change SPC sprite: " + p_index);

    }




    
    #endregion
}
