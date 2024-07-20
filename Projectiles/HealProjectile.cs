using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;

public class HealProjectile : Projectile
{
    public int healPoints;
    public float chanceHealDebuff;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICanTakeHit canTakeHit)/* && ship.gameObject != null*/)
        {
            canTakeHit.HealProjectileHit(shipWhoFired, healPoints, chanceHealDebuff, direction, projectileTransform.position);
        }

        CollisionInvoke();
        Destroy(gameObject);

    }


    public void AttackUltimateVisionEffect()
    {
        projectileTransform.localScale *= 2;
        trail.startWidth *= 2;
        trail.time *= 2;

    }
}
