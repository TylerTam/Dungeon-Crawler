using System.Collections;
using UnityEngine;

public class WeaponEffect_Base : MonoBehaviour
{
    public virtual IEnumerator PlayWeaponEffect()
    {
        yield return null;
    }
}
