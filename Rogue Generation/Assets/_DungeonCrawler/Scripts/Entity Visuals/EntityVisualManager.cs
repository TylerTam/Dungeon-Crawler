using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisualEvents : UnityEngine.Events.UnityEvent { }
public class EntityVisualManager : MonoBehaviour
{
    public Animator m_spriteAnimator;
    
    [System.Serializable]
    public struct VisualEvents
    {
        public EntityVisualEvents m_hurtAnimComplete;
        public EntityVisualEvents m_defeatedAnimComplete;
    }

    public void SwitchToIdleAnimation()
    {
        Debug.Log("Change to idle");
        m_spriteAnimator.SetTrigger("Idle");

    }
    public void SwitchToWalkingAnimation()
    {
        Debug.Log("Switch To walk Animation");
        m_spriteAnimator.SetTrigger("Walk");
    }

    public void UpdateFacingDir(Vector2 p_facingDir)
    {
        Debug.Log("Rotate To Face: " + p_facingDir);
        m_spriteAnimator.SetInteger("FacingX", (int)p_facingDir.x);
        m_spriteAnimator.SetInteger("FacingY", (int)p_facingDir.y);

    }

    #region Attack Related Animations

    public void SwitchToPhysicalAttackAnimation()
    {
        Debug.Log("Change to physical attack animation");
        m_spriteAnimator.SetTrigger("Physical");
    }
    public void SwitchToMagicAttackAnimation()
    {
        Debug.Log("Change To Magic attack animation");
        m_spriteAnimator.SetTrigger("Special");
    }
    #endregion

    #region Attacked Animations
    public void SwitchToHurtAnimation()
    {
        Debug.Log("Change to hurt animation");
        m_spriteAnimator.SetTrigger("Hurt");
    }

    public void HurtAnimationComplete()
    {
        Debug.Log("Hurt Animation Complete");
    }

    public void SwitchToDefeatedAnimation()
    {
        Debug.Log("Change to hurt animation");
        m_spriteAnimator.SetTrigger("Defeated");
    }
    public void DefeatedAnimationComplete()
    {
        Debug.Log("Defeated Animation Complete");
    }
    #endregion

}
