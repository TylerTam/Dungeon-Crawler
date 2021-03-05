using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack_Type_Projectile_", menuName = "Attacks/AttackTypes/Projectile", order = 0)]
public class AttackType_Projectile : AttackType_Base
{
    public GameObject m_spawnedProjecticle;



    public override WeaponEffect_Base CreateAttackEffects(AttackController p_attackerController, List<AttackController> p_affectedControllers, Vector2 p_facingDir)
    {
        
        GameObject currentProjectile = ObjectPooler.instance.NewObject(m_spawnedProjecticle, p_attackerController.transform.position, Quaternion.identity);

        bool foundTarget = false;
        if (p_affectedControllers != null)
        {
            if (p_affectedControllers.Count > 0)
            {
                if (p_affectedControllers[0] != null)
                {
                    foundTarget = true;
                    currentProjectile.GetComponent<WeaponEffect_Projectile>().SetupProjectile(p_affectedControllers[0].transform.position);
                }
            }
        }

        if (!foundTarget)
        {
            Vector2 startingPos = p_attackerController.transform.position;
            int dirX = (int)((Mathf.Abs(p_facingDir.x) < 0.5f) ? 0 : Mathf.Sign(p_facingDir.x));
            int dirY = (int)((Mathf.Abs(p_facingDir.y) < 0.5f) ? 0 : Mathf.Sign(p_facingDir.y));
            for (int i = 0; i < m_range; i++)
            {
                if(DungeonGenerationManager.Instance.GetWallCheck(startingPos.x + (i* dirX),startingPos.y + (i * dirY)) == GlobalVariables.m_wallCell)
                {
                    startingPos = startingPos + new Vector2(((i-1) * dirX), ((i-1) * dirY));
                    foundTarget = true;
                    break;
                }
            }
            if (!foundTarget)
            {
                startingPos = startingPos + new Vector2(dirX, dirY) * m_range;
            }
            currentProjectile.GetComponent<WeaponEffect_Projectile>().SetupProjectile(startingPos);
        }

       
        return currentProjectile.GetComponent<WeaponEffect_Base>();

    }
}
