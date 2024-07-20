using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Эффект (результат), который происходит при попадании снаряда в цель.
    /// </summary>
    public enum Effect
    {
        Damage,
        Stun,
        Electro,
        USP_Power,
        Push,

        Heal,
    }


    //public Effect effect;
    public SpriteRenderer mainRenderer;
    public TrailRenderer trail;
    public Transform projectileTransform;
    public CircleCollider2D circleCollider2D;
    public Vector2 direction;

    public Ship shipWhoFired;
    public event Action collisionEvent;

    protected void CollisionInvoke()
    {
        collisionEvent.Invoke();
    }
    

    
}
