using UnityEngine;
using System.Linq;

public class StunProjectile : Projectile
{
    public float stunTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out ICanTakeHit takeHit))
        {
            if (!takeHit.StunProjectileHit(shipWhoFired, stunTime, direction, projectileTransform.position))
                return;
        }

        CollisionInvoke();
        Destroy(gameObject);
    }

    public void UltimateEffect()
    {
        stunTime *= 2;
        projectileTransform.localScale *= 4;
        trail.startWidth *= 2;
        trail.time *= 2;
        mainRenderer.color = References.Instance.colors.stun_color;

    }





}
