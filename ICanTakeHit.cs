using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Все, что может получать удар.
/// </summary>
public interface ICanTakeHit
{
    //public Action<int, Vector2, Vector3> EffectFromDamageProjectile { get; set; }

    /// <summary>
    /// Попадание обычным снарядом урона.
    /// </summary>
    /// <param name="shipWhoFired"></param>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    /// <param name="impactPoint"></param>
    /// <returns></returns>
    public bool DamageProjectileHit(Ship shipWhoFired, int damage, Vector2 direction, Vector3 impactPoint);

    /// <summary>
    /// Попадание снарядом заморозки.
    /// </summary>
    /// <param name="shipWhoFired"></param>
    /// <param name="stunTime"></param>
    /// <param name="direction"></param>
    /// <param name="impactPoint"></param>
    public bool StunProjectileHit(Ship shipWhoFired, float stunTime, Vector2 direction, Vector3 impactPoint);

    /// <summary>
    /// Попадание лечащим снарядом.
    /// </summary>
    /// <param name="shipWhoFired"></param>
    /// <param name="healValue"></param>
    /// <param name="chanceToDebuff"></param>
    /// <param name="direction"></param>
    /// <param name="impactPoint"></param>
    public void HealProjectileHit(Ship shipWhoFired, int healValue, float chanceToDebuff, Vector2 direction, Vector3 impactPoint);


}
