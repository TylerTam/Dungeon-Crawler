using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffect_Projectile : WeaponEffect_Base
{
    public float m_timePerUnit;

    private Vector3 m_targetPosition;
    public void SetupProjectile(Vector3 p_targetPosition)
    {
        m_targetPosition = p_targetPosition;
    }

    public override IEnumerator PlayWeaponEffect()
    {
        Vector3 startingPos = transform.position;
        float timer = 0;
        float lerpTime = m_timePerUnit * (int)Vector3.Distance(startingPos, m_targetPosition);
        while (timer < lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPos, m_targetPosition, timer / lerpTime);
        }

        transform.position = m_targetPosition;
        yield return null;
    }
}
