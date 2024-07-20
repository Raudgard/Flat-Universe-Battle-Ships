using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class Prefabs : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Singleton

    private static Prefabs instance;
    public static Prefabs Instance => instance;


    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void OnBeforeSerialize()
    {
        //Debug.Log("onbefore");

    }

    public void OnAfterDeserialize()
    {
        if (instance == null)
            instance = this;
    }



    #endregion

    #region Prefabs

    public Ship ship;
    public USP uspPrefab;
    public PreUSP preUSP;
    public GameObject shockWaveFromUSPCreation;
    public GameObject explosionOfDestroyedShip;
    public DepartingDigits departingDigits;
    public GameObject bordersPrefab;
    public BackGround_Lights BackGround_Lights_prefab;
    public DamageProjectile damageProjectile;
    public StunProjectile stunProjectile;
    public HealProjectile healProjectile;
    public DamageProjectile projectile_USP_Power;
    public Projectile—ontainer projectileContainer;
    public Ancile ancile;


    public Healing_orb healing_Orb;

    public PowerBar powerBar;

    #region visualEffectController



    [Foldout("Visual Effects")] public ParticleSystem healEffect;
    [Foldout("Visual Effects")] public ParticleSystem USPTakeEffect;
    [Foldout("Visual Effects")] public ParticleSystem teleportationChargeEffect;
    [Foldout("Visual Effects")] public ParticleSystem patriotModuleEffect;
    [Foldout("Visual Effects")] public ParticleSystem patriotModuleUltimateEffect;
    [Foldout("Visual Effects")] public GameObject stunAnimationEffect;
    [Foldout("Visual Effects")] public ParticleSystem iceEffect;

    [Foldout("Visual Effects")] public ParticleSystem reproductionVisualEffect;
    [Foldout("Visual Effects")] public ParticleSystem damageRecievedExplosion;
    [Foldout("Visual Effects")] public ParticleSystem damageRecievedArmorUltimate;
    [Foldout("Visual Effects")] public ParticleSystem engineFlame;
    [Foldout("Visual Effects")] public ParticleSystem teleportationStartEffect;
    [Foldout("Visual Effects")] public ParticleSystem teleportationEndEffect;
    [Foldout("Visual Effects")] public ParticleSystem highlightEnemyShipEffect;
    [Foldout("Visual Effects")] public TrailRenderer USPTrail;
    //public TrailRenderer engineFlameTrail;



    #endregion




    #endregion



}
