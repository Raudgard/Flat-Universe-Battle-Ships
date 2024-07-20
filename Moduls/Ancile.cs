using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using System;
using UnityEngine.U2D;
using NaughtyAttributes;
using MODULES;

public class Ancile : MonoBehaviour, INeedFixUpdate, ICanTakeHit
{
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField] private SpriteShapeRenderer spriteShapeRenderer;
    private Ancile_Module ancile_Module;
    private Ship ship;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform _transform;
    public Rotation_Controller shipRotationController;
    [SerializeField] private float radius;
    [Tooltip("Коэффициент, отвечающий за удаленность точки визуального эффекта попадания в щит.")]
    [SerializeField] private float backwardCoefficient;

    /// <summary>
    /// Константа зависимости размера вектора касательной от размера дуги щита.
    /// </summary>
    private const float mathCoeff = 1166.6666666f;

    [SerializeField] private Vector3 explosionHitLocalScale;
    [SerializeField] private float changingSpeedWhenStunValue;
    [SerializeField] private float recoveryTime;

    [SerializeField] private int maxHitsCapacity;
    [SerializeField] private int hitsCapacity;
    [SerializeField] private int maxCoverDegrees;
    [SerializeField] private int coverDegrees;

    private bool IsStunned { get; set; } = false;
    private float rotationSpeedOriginal;
    private Coroutine recoveryCoroutine;


    private void Awake()
    {
        _transform = transform;
        Updater.Instance.RegisterNeedUpdateObject(this);
        rotationSpeedOriginal = rotationSpeed;  
    }

    private void Stealth_Module_BecameVisible(Ship ship, Stealth_Module stealth_Module)
    {
        if(ship != null)
            spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r, spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, 1);
    }

    private void Stealth_Module_BecameInvisible(Ship ship, Stealth_Module stealth_Module)
    {
        float alphaChannelValue = ship.team == 1 ? References.Instance.settings.minAlphaChannelForStealthUserPlayer : 0;
        spriteShapeRenderer.color = new Color(spriteShapeRenderer.color.r, spriteShapeRenderer.color.g, spriteShapeRenderer.color.b, alphaChannelValue);
    }

    /// <summary>
    /// Инициализирует щит.
    /// </summary>
    /// <param name="maxHitCount">Максимальное количество ударов с уроном, которое нейтрализует щит.</param>
    /// <param name="coverDegrees">Градусы кривой, на сколько щит закрывает корабль.</param>
    public void Initialize(Ship ship, Ancile_Module ancile_Module, int maxHitsCapacity, int coverDegrees, float recoveryTime)
    {
        this.maxHitsCapacity = hitsCapacity = maxHitsCapacity;
        maxCoverDegrees = this.coverDegrees = coverDegrees;
        this.ship = ship;
        this.ancile_Module = ancile_Module;
        this.recoveryTime = recoveryTime;
        UpdateSpriteShapeToSmaller();

        ship.takeHitComponent.CheckForAncileDamage += TakeHitComponent_CheckForAncileDamage;
        ship.takeHitComponent.CheckForAncileStun += TakeHitComponent_CheckForAncileStun;

        if (ship.TryGetComponent(out Stealth_Module stealth_Module))
        {
            stealth_Module.BecameInvisible += Stealth_Module_BecameInvisible;
            stealth_Module.BecameVisible += Stealth_Module_BecameVisible;
        }

        if (ship.IsInvisible)
        {
            Stealth_Module_BecameInvisible(ship, stealth_Module);
        }
    }


    /// <summary>
    /// Ударил снаряд в щит? true - если щит закрыл корабль. False - если мимо щита пролетел и ударил в корабль.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="touchPosition"></param>
    /// <returns></returns>
    private bool TakeHitComponent_CheckForAncileDamage(Ship shipWhoFired, int damage, Vector2 direction, Vector3 touchPosition)
    {
        if (CheckForShield(touchPosition))
        {
            //Debug.Log($"Ancile. TakeHitComponent_CheckForAncileDamage return true");
            DamageProjectileHit(shipWhoFired, damage, direction, touchPosition);
            return true;
        }
        //Debug.Log($"Ancile. TakeHitComponent_CheckForAncileDamage return false");
        return false;
    }

    /// <summary>
    /// Ударил снаряд в щит? true - если щит закрыл корабль. False - если мимо щита пролетел и ударил в корабль.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private bool TakeHitComponent_CheckForAncileStun(Ship shipWhoFired, float stunTime, Vector2 direction, Vector3 touchPosition)
    {
        if (CheckForShield(touchPosition))
        {
            //Debug.Log($"Ancile. TakeHitComponent_CheckForAncileStun return true");
            StunProjectileHit(shipWhoFired, stunTime, direction, touchPosition);
            return true;
        }
        //Debug.Log($"Ancile. TakeHitComponent_CheckForAncileStun return false");
        return false;
    }

    /// <summary>
    /// Ударил снаряд в щит? true - если щит закрыл корабль. False - если мимо щита пролетел и ударил в корабль.
    /// </summary>
    /// <param name="touchPosition"></param>
    /// <returns></returns>
    private bool CheckForShield(Vector3 touchPosition)
    {
        var signedAngle = Vector2.SignedAngle(Vector2.right, touchPosition - ship.transform.position);
        var originOfAncile = (int)transform.rotation.eulerAngles.z;
        var endOfAncile = originOfAncile + coverDegrees;
        if (signedAngle < 0 || endOfAncile > 360)
        {
            originOfAncile -= 360;
            endOfAncile -= 360;
        }
        bool isAncileTakeHit = signedAngle > originOfAncile && signedAngle < endOfAncile;
        //Debug.Log($"SignedAngle: {signedAngle}, originOfAncile: {originOfAncile}, endOfAncile: {endOfAncile}, isAncileTakeHit: {isAncileTakeHit}");
        return isAncileTakeHit;
    }


    /// <summary>
    /// Обновляет контрольные точки сплайна. Использовать при УМЕНЬШЕНИИ сплайна во избежание ошибки при spline.SetPosition(i, pointsNew[i]).
    /// Здесь точки обновляются в порядке от 0 к i.
    /// </summary>
    [Button]
    private void UpdateSpriteShapeToSmaller()
    {
        Spline spline = spriteShapeController.spline;
        var pointsCount = spline.GetPointCount();
        var pointsNew = GetPointsForSpline();
        //Debug.Log($"pointsNew count: {pointsNew.Length}");

        var center = transform.localPosition;
        //string settingPositions = "SettPosition: ";
        //string factPositions = "FactPosition: ";


        for (int i = 0; i < pointsCount; i++)
        {
            //settingPositions += $"sPos[{i}]: {pointsNew[i]}, ";
            spline.SetPosition(i, pointsNew[i]);
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            Vector2 vectorToPointFromCenter = (Vector2)center - pointsNew[i];
            Vector3 tangentVector = Vector2.Perpendicular(vectorToPointFromCenter);
            float tangentVectorCoeff = coverDegrees / mathCoeff;
            spline.SetRightTangent(i, -tangentVector * tangentVectorCoeff);
            spline.SetLeftTangent(i, tangentVector * tangentVectorCoeff);
            //factPositions += $"fPos[{i}]: {(Vector2)spline.GetPosition(i)}, ";
        }
        //Debug.Log($" {settingPositions}");
        //Debug.Log($" {factPositions}");
        spriteShapeController.RefreshSpriteShape();
    }

    /// <summary>
    /// Обновляет контрольные точки сплайна. Использовать при УВЕЛИЧЕНИИ сплайна во избежание ошибки при spline.SetPosition(i, pointsNew[i]).
    /// Здесь точки обновляются в порядке от i к 0.
    /// </summary>
    private void UpdateSpriteShapeToLarger()
    {
        Spline spline = spriteShapeController.spline;
        var pointsCount = spline.GetPointCount();
        var pointsNew = GetPointsForSpline();
        //Debug.Log($"pointsNew count: {pointsNew.Length}");

        var center = transform.localPosition;
        //string settingPositions = "SettPosition: ";
        //string factPositions = "FactPosition: ";

        for (int i = pointsCount - 1; i > -1; i--)
        {
            //settingPositions += $"sPos[{i}]: {pointsNew[i]}, ";
            spline.SetPosition(i, pointsNew[i]);
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            Vector2 vectorToPointFromCenter = (Vector2)center - pointsNew[i];
            Vector3 tangentVector = Vector2.Perpendicular(vectorToPointFromCenter);
            float tangentVectorCoeff = coverDegrees / mathCoeff;
            spline.SetRightTangent(i, -tangentVector * tangentVectorCoeff);
            spline.SetLeftTangent(i, tangentVector * tangentVectorCoeff);
            //factPositions += $"fPos[{i}]: {(Vector2)spline.GetPosition(i)}, ";
        }
        //Debug.Log($" {settingPositions}");
        //Debug.Log($" {factPositions}");
        spriteShapeController.RefreshSpriteShape();
        //arcCollider.totalAngle = coverDegrees - 7;
        //edgeCollider.points = arcCollider.getPoints(Vector2.zero);
    }

    private Vector2[] GetPointsForSpline()
    {
        int pointsCount = 8;
        int arcCount = pointsCount - 1;
        Vector2[] points = new Vector2[pointsCount];
        //var center = transform.localPosition;
        float ang = 0;
        //Debug.Log($"origin: {center}, pointsCount: {pointsCount}");

        for (int i = 0; i < pointsCount; i++)
        {
            float x = /*center.x + */radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            float y = /*center.y + */radius * Mathf.Sin(ang * Mathf.Deg2Rad);

            points[i] = new Vector2(x, y);
            ang += (float)coverDegrees / arcCount;
        }

        return points;
    }

    public void FixUpdateMe()
    {
        _transform.Rotate(Vector3.forward, rotationSpeed - shipRotationController.currentRotationZValue);
    }

    private void OnDestroy()
    {
        Updater.Instance.UnregisterNeedUpdateObject(this);   
    }

    public bool DamageProjectileHit(Ship shipWhoFired, int damage, Vector2 direction, Vector3 impactPoint)
    {
        if (ancile_Module.UltimateImpactAction())
        {
            var effect = Instantiate(Prefabs.Instance.damageRecievedArmorUltimate);
            effect.transform.position = new Vector3(impactPoint.x, impactPoint.y, -3);
            effect.transform.Translate(-direction * backwardCoefficient);
            Vector2 perpendicular = Vector2.Perpendicular(direction);
            float angle = Vector2.Angle(perpendicular, new Vector2(direction.x, 0));
            effect.transform.localRotation = Quaternion.Euler(0, 0, angle);
            return true;
        }

        var explosion = Instantiate(Prefabs.Instance.damageRecievedExplosion);
        //_transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
        //Vector3 effectPosition =
        explosion.transform.position = new Vector3(impactPoint.x, impactPoint.y, -3);
        explosion.transform.Translate(-direction * backwardCoefficient);
        explosion.transform.localScale = explosionHitLocalScale;

        hitsCapacity--;

        if (hitsCapacity <= 0)
        {
            //Debug.Log($"Ancile take deadly hit. hitsCapacity: {hitsCapacity}");
            ship.takeHitComponent.CheckForAncileDamage -= TakeHitComponent_CheckForAncileDamage;
            ship.takeHitComponent.CheckForAncileStun -= TakeHitComponent_CheckForAncileStun;
            Updater.Instance.UnregisterNeedUpdateObject(this);
            Destroy(gameObject);
            return true;
        }

        coverDegrees = maxCoverDegrees / maxHitsCapacity * hitsCapacity;
        //Debug.Log($"Ancile take hit. hitsCapacity: {hitsCapacity}, coverDegrees: {coverDegrees}");

        UpdateSpriteShapeToSmaller();
        if (recoveryCoroutine == null)
        {
            recoveryCoroutine = StartCoroutine(Recovery());
        }
        return true;
    }

    #region StunProjectileHit
    public bool StunProjectileHit(Ship shipWhoFired, float stunTime, Vector2 direction, Vector3 impactPoint)
    {
        //Debug.Log($"Ancile Stun");

        if (IsStunned)
        {
            return true;
        }

        IsStunned = true;

        var stunEffect = Instantiate(Prefabs.Instance.iceEffect);
        stunEffect.transform.parent = transform;
        stunEffect.transform.localPosition = Vector3.zero;
        stunEffect.transform.localRotation = Quaternion.identity;
        stunEffect.transform.localScale = Prefabs.Instance.iceEffect.transform.localScale;
        var main = stunEffect.main;
        main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 2);

        var emission = stunEffect.emission;
        //var burst = emission.GetBurst(0);
        ParticleSystem.Burst burst = new ParticleSystem.Burst();
        burst.count = 24 * coverDegrees / 360;
        burst.cycleCount = 1;
        burst.repeatInterval = 0.01f;
        emission.SetBurst(0, burst);

        var shape = stunEffect.shape;
        shape.radius = 2;
        shape.radiusThickness = 0;
        shape.arc = coverDegrees;
        shape.arcMode = ParticleSystemShapeMultiModeValue.BurstSpread;
        shape.arcSpread = 0;

        StartCoroutine(DecreasingSpeed());
        StartCoroutine(WaitingForStunEnd(stunTime));

        return true;
    }

   


    private IEnumerator DecreasingSpeed()
    {
        while (rotationSpeed > 0)
        {
            rotationSpeed -= changingSpeedWhenStunValue;
            yield return new WaitForFixedUpdate();
        }
        rotationSpeed = 0;
    }


    public IEnumerator WaitingForStunEnd(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        //ice_Anim_Controller.Resume();
        StartCoroutine(IncreasingSpeed());
    }

    private IEnumerator IncreasingSpeed()
    {
        while (rotationSpeed < rotationSpeedOriginal)
        {
            rotationSpeed += changingSpeedWhenStunValue;
            yield return new WaitForFixedUpdate();
        }
        rotationSpeed = rotationSpeedOriginal;
        IsStunned = false;
    }

    public void HealProjectileHit(Ship shipWhoFired, int damage, float healValue, Vector2 direction, Vector3 impactPoint) { }

    #endregion



    private IEnumerator Recovery()
    {
        yield return new WaitForSeconds(recoveryTime);
        hitsCapacity++;
        coverDegrees += maxCoverDegrees / maxHitsCapacity;
        //Debug.Log($"Ancile recovery. hitsCapacity: {hitsCapacity}, coverDegrees: {coverDegrees}");

        if (coverDegrees > maxCoverDegrees)
        {
            coverDegrees = maxCoverDegrees;
        }

        if (hitsCapacity >= maxHitsCapacity)
        {
            hitsCapacity = maxHitsCapacity;
            recoveryCoroutine = null;
        }
        else
        {
            recoveryCoroutine = StartCoroutine(Recovery());
        }

        UpdateSpriteShapeToLarger();
    }







    #region for debug
    //[Button]
    //public void ShowPoints()
    //{
    //    Spline spline = spriteShapeController.spline;
    //    int pointsCount = spline.GetPointCount();
    //    Debug.Log($"pointsCount: {pointsCount}");

    //    for (int i = 0; i < pointsCount; i++)
    //    {
    //        Vector3 position = spline.GetPosition(i);
    //        Vector3 rightTangent= spline.GetRightTangent(i);
    //        Vector3 leftTangent = spline.GetLeftTangent(i);
    //        Debug.Log($"position[{i}]: {position}, rightTangent: {rightTangent}, leftTangent: {leftTangent}");
    //    }
    //}
    #endregion

}
