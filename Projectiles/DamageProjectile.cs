using UnityEngine;
using System.Collections;

public class DamageProjectile : Projectile
{
    public int damage;
    [SerializeField] private float flashingInterval;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ICanTakeHit takeHit))
        {
            if (!takeHit.DamageProjectileHit(shipWhoFired, damage, direction, projectileTransform.position))
                return;
        }

        CollisionInvoke();
        Destroy(gameObject);
    }



    public void AttackUltimateVisionEffect()
    {
        projectileTransform.localScale *= 4;
        trail.startWidth *= 2;
        trail.time *= 2;

        trail.startColor = mainRenderer.color = References.Instance.colors.digits_damage_color;

        //не используем мигаение, потому что этобольше похоже на взрывающийся снаряд.

        //StartCoroutine(FlashingRed());
    }




    //private IEnumerator FlashingRed()
    //{
    //    var delay = new WaitForSeconds(flashingInterval);
    //    while (true)
    //    {
    //        mainRenderer.color = Color.red;
    //        trail.startColor = Color.red;
    //        yield return delay;

    //        mainRenderer.color = Color.white;
    //        trail.startColor = Color.white;
    //        yield return delay;
    //    }
    //}

}
