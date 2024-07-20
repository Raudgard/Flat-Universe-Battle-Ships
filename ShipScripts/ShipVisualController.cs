using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using System;
using UnityEngine.ParticleSystemJobs;
using System.Linq;

/// <summary>
/// Контролирует визуальные эффекты корабля.
/// </summary>
public class ShipVisualController : MonoBehaviour
{
    #region Initialization

    [SerializeField] protected Ship ship;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public SpriteRenderer backLightRenderer;
    [SerializeField] protected SpriteRenderer healthBarRenderer;
    [SerializeField] protected SpriteRenderer healthBackgroundBarRenderer;
    [SerializeField] protected SpriteRenderer USPBarRenderer;
    private Global_Controller global_Controller;

    public Color originalShipColor;
    public Color originalShipBacklightColor;
    public Color originalShipHealthBarColor;
    public Color originalShipHealthBackgroundBarColor;
    public Color originalShipUSPBarColor;

    /// <summary>
    /// Длительность визуального эффекта.
    /// </summary>
    public enum VisualEffectDuration
    {
        /// <summary>
        /// Короткий. Одноразовый. Сработал и уничтожился.
        /// </summary>
        Short = 0,

        /// <summary>
        /// Постоянный. Включаемый и выключаемый скриптом. Постоянно прикреплен к объекту, как отдельный GameObject.
        /// </summary>
        Permanent = 1
    }

    private event Action<ParticleSystem, VisualEffectDuration> VisualEffectCreated;
    //private event Action<GameObject, VisualEffectDuration> VisualEffectGOCreated;



    [SerializeField]
    private GameObject debrisFromDamage;
    public float[] debrisSize = new float[4];

    [SerializeField]
    private Transform USPBarTransform;

    [Range(0, 1)]
    [SerializeField]
    private float m_USPBarFillAmount;

    public float USPBarFillAmount { get { return m_USPBarFillAmount; }
        set
        {
            var m_value = Mathf.Clamp01(value);
            m_USPBarFillAmount = m_value;
            USPBarTransform.localScale = new Vector3(m_value, USPBarTransform.localScale.y, USPBarTransform.localScale.z);
        }
    }


    [SerializeField] private GameObject healthBars;
    [SerializeField] private Transform healthBarTransform;

    public float HealthBarFillAmount
    {
        set
        {
            float m_value = Mathf.Clamp01(value);
            if (m_value < 1)
            {
                healthBars.SetActive(true);
                healthBarTransform.localScale = new Vector3(m_value, healthBarTransform.localScale.y, healthBarTransform.localScale.z);
            }
            else
            {
                healthBars.SetActive(false);
            }
        }
    }



    private void Start()
    {
        global_Controller = Global_Controller.Instance;

        if (ship.IsOriginal)
        {
            originalShipColor = spriteRenderer.color;
            originalShipBacklightColor = backLightRenderer.color;
            originalShipHealthBarColor = healthBarRenderer.color;
            originalShipHealthBackgroundBarColor = healthBackgroundBarRenderer.color;
            originalShipUSPBarColor = USPBarRenderer.color;
        }
        else
        {
            originalShipColor = ship.motherShip.shipVisualController.originalShipColor;
            originalShipBacklightColor = ship.motherShip.shipVisualController.originalShipBacklightColor;
            originalShipHealthBarColor = ship.motherShip.shipVisualController.originalShipHealthBarColor;
            originalShipHealthBackgroundBarColor = ship.motherShip.shipVisualController.originalShipHealthBackgroundBarColor;
            originalShipUSPBarColor = ship.motherShip.shipVisualController.originalShipUSPBarColor;
            reproductionVS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }


    #endregion



    #region Damage taken

    /// <summary>
    /// Визуальный эффект в виде взрыва и генерации осколков корабля. При получении урона от обычного снаряда.
    /// </summary>
    /// <param name="impactPoint"></param>
    /// <param name="damage"></param>
    public virtual void ExplosionAndDebrisWhenDamageTaken(int damage, Vector2 direction, Vector3 impactPoint)
    {
        //осколки и взрыв видны у невидимых юнитов

        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        GameObject debris = Instantiate(debrisFromDamage);

        debris.transform.position = impactPoint;
        debris.transform.parent = BattleSceneController.Instance.Debris_Transform;
        var debrisSizeCoef = DebrisSize(ship.healthMax, damage);
        debris.transform.localScale *= ship.changedSize * debrisSizeCoef;


        var explosion = Instantiate(Prefabs.Instance.damageRecievedExplosion);
        explosion.transform.position = new Vector3(impactPoint.x, impactPoint.y, -3);
        explosion.transform.localScale *= ship.changedSize * debrisSizeCoef;


        global_Controller.StartCoroutine(global_Controller.VisualizationOfDamage(damage, ship.healthMax, transform.position, direction, References.Instance.colors.digits_damage_color));

        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystemGameObject(debris, TransparencyDegree.BarelySee);
        //    SetTransparancyForParticleSystemGameObject(explosion, TransparencyDegree.BarelySee);
        //}
    }


    protected float DebrisSize(float shipMaxHealth, int damage)
    {
        var relativeDamage = damage / shipMaxHealth;

        if (relativeDamage > 0 && relativeDamage <= 0.25f) return debrisSize[0];
        else if (relativeDamage > 0.25f && relativeDamage <= 0.5f) return debrisSize[1];
        else if (relativeDamage > 0.5f && relativeDamage <= 0.75f) return debrisSize[2];
        else if (relativeDamage > 0.75f) return debrisSize[3];
        else return debrisSize[0];


    }

    /// <summary>
    /// Визуальный эффект при срабатывании ультимейта модуля Armor.
    /// </summary>
    /// <param name="impactPoint"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public virtual void ArmorUltimateDamageTaken(Vector2 direction, Vector3 impactPoint)
    {
        var effect = Instantiate(Prefabs.Instance.damageRecievedArmorUltimate);
        effect.transform.position = new Vector3(impactPoint.x, impactPoint.y, -3);
        //float angle = Vector2.Angle(Vector2.right, direction);
        Vector2 perpendicular = Vector2.Perpendicular(direction);
        float angle = Vector2.Angle(perpendicular, new Vector2(direction.x, 0));
        effect.transform.localRotation = Quaternion.Euler(0, 0, angle);

        global_Controller.StartCoroutine(global_Controller.VisualizationOfDamage(0, ship.healthMax, transform.position, direction, Color.white));

        //effect.transform.localScale *= ship.changedSize;
    }



    #endregion



    #region Reproduction

    [SerializeField] private ParticleSystem reproductionVS = null;

    public virtual void ReproductionBegun()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        if (reproductionVS == null)
        {
            reproductionVS = Instantiate(Prefabs.Instance.reproductionVisualEffect);
            reproductionVS.transform.SetParent(transform);
            reproductionVS.transform.localPosition = new Vector3(0, 0, 2);
            reproductionVS.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            //RegisterAndSetTransparansyForVisualEffect(glowSparkles);
            VisualEffectCreated?.Invoke(reproductionVS, VisualEffectDuration.Permanent);
        }
        else
        {
            reproductionVS.Play(true);
        }
    }

    public virtual void ReproductionEnd()
    {
        if (reproductionVS != null)
        {
            //Destroy(reproductionVS);
            reproductionVS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            //reproductionVS = null;
        }
    }

    #endregion



    #region USP taken

    //[SerializeField] private float timeOfUSPTakenColor;

    public virtual void USP_Taken()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        var USPTakeEffect = Instantiate(Prefabs.Instance.USPTakeEffect);
        USPTakeEffect.transform.parent = ship.transform;
        USPTakeEffect.transform.localPosition = new Vector3(0, 0, -5);
        USPTakeEffect.transform.localScale = Vector3.one;

        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystem(USPTakeEffect, TransparencyDegree.BarelySee);
        //}

        //RegisterAndSetTransparansyForVisualEffect(USPTakeEffect);
        VisualEffectCreated?.Invoke(USPTakeEffect, VisualEffectDuration.Short);
    }

    #endregion



    #region Heal

    public virtual void Heal()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        var healEffect = Instantiate(Prefabs.Instance.healEffect);
        healEffect.transform.parent = ship.transform;
        healEffect.transform.localPosition = new Vector3(0, 0, -5);
        healEffect.transform.localScale = Vector3.one;

        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystem(healEffect, TransparencyDegree.BarelySee);
        //}
        //RegisterAndSetTransparansyForVisualEffect(healEffect);
        VisualEffectCreated?.Invoke(healEffect, VisualEffectDuration.Short);

    }

    #endregion



    #region Stun

    //private Ice_anim_controller ice_Anim_Controller = null;
    //[SerializeField] private GameObject stunAnimationEffect;

    //public virtual void Stun()
    //{
    //    var stunEffect = Instantiate(Prefabs.Instance.stunAnimationEffect);
    //    stunEffect.transform.parent = ship.transform;
    //    stunEffect.transform.localPosition = new Vector3(0, 0, -3);
    //    stunEffect.transform.localScale = Prefabs.Instance.stunAnimationEffect.transform.localScale;

    //    ice_Anim_Controller = stunEffect.GetComponent<Ice_anim_controller>();
    //}

    //public virtual void StunEnd()
    //{
    //    ice_Anim_Controller.Resume();
    //}


    public virtual void Stun(float stunTime)
    {
        ParticleSystem stunEffect = Instantiate(Prefabs.Instance.iceEffect);
        stunEffect.transform.parent = ship.transform;
        stunEffect.transform.localPosition = new Vector3(0, 0, -3);
        stunEffect.transform.localScale = Prefabs.Instance.iceEffect.transform.localScale;
        var main = stunEffect.main;
        main.startLifetime = stunTime;

        //ice_Anim_Controller = stunEffect.GetComponent<Ice_anim_controller>();
    }





    #endregion



    #region Evasion

    private Coroutine evasionCoroutine = null;

    public virtual void EvasionStart()
    {
        evasionCoroutine = StartCoroutine(EvasionStartCoro());
    }

    private IEnumerator EvasionStartCoro()
    {
        var originalColor = ship.IsInvisible ? spriteRenderer.color : originalShipColor;
        var colorTrancelucent = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a / 5);
        var fixupdatetime = new WaitForFixedUpdate();
        float vanishingSpeed = References.Instance.settings.speedOfVanishing;
        float t = 1;

        while (t > 0)
        {
            spriteRenderer.color = Color.Lerp(colorTrancelucent, originalColor, t);
            t -= vanishingSpeed;
            yield return fixupdatetime;
        }
        evasionCoroutine = null;
    }

    public virtual void EvasionEnd()
    {
        evasionCoroutine = StartCoroutine(EvasionEndCoro());
    }

    private IEnumerator EvasionEndCoro()
    {
        var originalColor = ship.IsInvisible ? spriteRenderer.color : originalShipColor;
        var colorTrancelucent = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a / 5);
        var fixupdatetime = new WaitForFixedUpdate();
        float vanishingSpeed = References.Instance.settings.speedOfVanishing;

        float t = 0;

        while (t < 1)
        {
            spriteRenderer.color = Color.Lerp(colorTrancelucent, originalColor, t);
            t += vanishingSpeed;
            yield return fixupdatetime;

        }
        evasionCoroutine = null;
    }

    #endregion



    #region Teleportation

    public virtual void Teleportation(Teleportation_Module TP_module, float teleportationSpeed, Action teleportationEndAction)
    {
        StartCoroutine(TeleportartionGo(TP_module, teleportationSpeed, teleportationEndAction));
    }

    private IEnumerator TeleportartionGo(Teleportation_Module TP_module, float teleportationSpeed, Action teleportationEndAction)
    {
        ParticleSystem _teleportation_Start = null;
        //if (!ship.IsInvisible)
        //{
            _teleportation_Start = Instantiate(Prefabs.Instance.teleportationStartEffect, ship.transform);
            _teleportation_Start.transform.localPosition = new Vector3(0, 0, -2);
            var particleSystems = _teleportation_Start.GetComponentsInChildren<ParticleSystem>();
            var mains = particleSystems.Select(ps => ps.main).ToArray();
            mains[0].startSizeMultiplier *= ship.changedSize;
            mains[0].simulationSpeed = teleportationSpeed;
            mains[1].startSizeMultiplier *= ship.changedSize;
            mains[1].simulationSpeed = teleportationSpeed;
        //}

        //DefineAndSetTransparancyForVisualEffect(_teleportation_Start);
        VisualEffectCreated?.Invoke(_teleportation_Start, VisualEffectDuration.Short);
        VisualEffectCreated?.Invoke(particleSystems[1], VisualEffectDuration.Short);

        float pauseTime = 1 / teleportationSpeed;
        yield return new WaitForSeconds(pauseTime);

        spriteRenderer.enabled = false;
        ship.shipBars.SetActive(false);
        backLightRenderer.enabled = false;

        if (_teleportation_Start != null)
        {
            _teleportation_Start.transform.SetParent(null);
        }

        var moveDirectionTemp = ship.moveDirection;
        var newPosition = TP_module.GetNewPosition();
        ship.moveDirection = Vector2.zero;

        ParticleSystem teleportation_End = null;
        //if (!ship.IsInvisible)
        //{
            teleportation_End = Instantiate(Prefabs.Instance.teleportationEndEffect, new Vector3(newPosition.x, newPosition.y, -2), Quaternion.identity);
        //}

        //DefineAndSetTransparancyForVisualEffect(teleportation_End);
        particleSystems = teleportation_End.GetComponentsInChildren<ParticleSystem>();
        VisualEffectCreated?.Invoke(teleportation_End, VisualEffectDuration.Short);
        VisualEffectCreated?.Invoke(particleSystems[1], VisualEffectDuration.Short);


        yield return new WaitForSeconds(0.25f);

        ship.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
        if (teleportation_End != null)
        {
            teleportation_End.transform.parent = ship.transform;
        }

        spriteRenderer.enabled = true;
        ship.shipBars.SetActive(true);
        backLightRenderer.enabled = true;

        if (!ship.movingFromForce)
        {
            ship.moveDirection = moveDirectionTemp;
        }

        teleportationEndAction.Invoke();
    }

    public virtual void TeleportationCharge()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        var teleportationChargeEffect = Instantiate(Prefabs.Instance.teleportationChargeEffect);
        teleportationChargeEffect.transform.parent = ship.transform;
        teleportationChargeEffect.transform.localPosition = new Vector3(0, 0, -5);
        teleportationChargeEffect.transform.localScale = Vector3.one;

        VisualEffectCreated?.Invoke(teleportationChargeEffect, VisualEffectDuration.Short);
        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystem(teleportationChargeEffect, TransparencyDegree.BarelySee);
        //}

    }


    #endregion



    #region Invisibility

    private Coroutine becomeInvisibleCoroutine = null;
    private Coroutine becomeVisibleCoroutine = null;

    /// <summary>
    /// Степень прозрачности.
    /// </summary>
    private enum TransparencyDegree
    {
        /// <summary>
        /// Полная видимость.
        /// </summary>
        FullVisibility,

        /// <summary>
        /// Видно едва-едва.
        /// </summary>
        BarelySee,

        /// <summary>
        /// Полная прозрачность. Фактически не отображается.
        /// </summary>
        FullTransparancy
    }

    /// <summary>
    /// Список визуальных эффектов, реализованных в системе частиц, и подлежащих учету при назначении прозрачности в Stealth_Module.
    /// </summary>
    private List<ParticleSystem> visualEffects_PS_ForTransparansy = new List<ParticleSystem>();

    
    public void StealthModuleInitialize()
    {
        VisualEffectCreated += ShipVisualController_VisualEffectCreated;
    }

    private void ShipVisualController_VisualEffectCreated(ParticleSystem particleSystem, VisualEffectDuration visualEffectDuration)
    {
        if (visualEffectDuration == VisualEffectDuration.Permanent)
        {
            RegisterAndSetTransparansyForVisualEffect(particleSystem);
        }
        else if (visualEffectDuration == VisualEffectDuration.Short)
        {
            DefineAndSetTransparancyForVisualEffect(particleSystem);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    //private void ShipVisualController_VisualEffectGOCreated(GameObject gameObjectPS, VisualEffectDuration visualEffectDuration)
    //{
    //    if (visualEffectDuration == VisualEffectDuration.Permanent)
    //    {
    //        RegisterAndSetTransparansyForVisualEffect(gameObjectPS);
    //    }
    //    else if (visualEffectDuration == VisualEffectDuration.Short)
    //    {
    //        DefineAndSetTransparancyForVisualEffect(gameObjectPS);
    //    }
    //    else
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    /// <summary>
    /// Запускает процесс исчезновения.
    /// </summary>
    /// <param name="timeToDisappearing">Время для полного исчезновения.</param>
    /// <param name="successfulBecameInvisible">Колбэк при успешном исчезновении.</param>
    public void BecameInvisible(float timeToDisappearing, Action successfulBecameInvisible)
    {
        becomeInvisibleCoroutine = StartCoroutine(BecomeInvisible(timeToDisappearing, successfulBecameInvisible));
    }

    private IEnumerator BecomeInvisible(float timeToDisappearing, Action successfulBecameInvisibleCallback)
    {
        var originalColor = spriteRenderer.color;
        var originalBacklightColor = backLightRenderer.color;
        var originalHealthBarColor = healthBarRenderer.color;
        var originalHealthBackgroundBarColor = healthBackgroundBarRenderer.color;
        var originalUSPBarColor = USPBarRenderer.color;

        float alphaChannelValue = ship.team == 1 ? References.Instance.settings.minAlphaChannelForStealthUserPlayer : 0;

        var colorTransparent = new Color(originalColor.r, originalColor.g, originalColor.b, alphaChannelValue);
        var colorBacklightTransparent = new Color(originalBacklightColor.r, originalBacklightColor.g, originalBacklightColor.b, alphaChannelValue);
        var colorHealthBarTransparent = new Color(originalHealthBarColor.r, originalHealthBarColor.g, originalHealthBarColor.b, alphaChannelValue);
        var colorHealthBackgroundBarTransparent = new Color(originalHealthBackgroundBarColor.r, originalHealthBackgroundBarColor.g, originalHealthBackgroundBarColor.b, alphaChannelValue);
        var colorUSPBarTransparent = new Color(originalUSPBarColor.r, originalUSPBarColor.g, originalUSPBarColor.b, alphaChannelValue);

        float currentTime = timeToDisappearing;
        float t = 1;

        while (t > 0)
        {
            if (evasionCoroutine != null)
            {
                currentTime -= Time.deltaTime;
                t = currentTime / timeToDisappearing;
                yield return null;
            }


            spriteRenderer.color = Color.Lerp(colorTransparent, originalColor, t);
            backLightRenderer.color = Color.Lerp(colorBacklightTransparent, originalBacklightColor, t);
            healthBarRenderer.color = Color.Lerp(colorHealthBarTransparent, originalHealthBarColor, t);
            healthBackgroundBarRenderer.color = Color.Lerp(colorHealthBackgroundBarTransparent, originalHealthBackgroundBarColor, t);
            USPBarRenderer.color = Color.Lerp(colorUSPBarTransparent, originalUSPBarColor, t);

            currentTime -= Time.deltaTime;
            t = currentTime / timeToDisappearing;
            yield return null;
        }

        spriteRenderer.color = colorTransparent;
        backLightRenderer.color = colorBacklightTransparent;
        healthBarRenderer.color = colorHealthBarTransparent;
        healthBackgroundBarRenderer.color = colorHealthBackgroundBarTransparent;
        USPBarRenderer.color = colorUSPBarTransparent;

        for (int i = 0; i < visualEffects_PS_ForTransparansy.Count; i++)
        {
            SetTransparancyForParticleSystem(visualEffects_PS_ForTransparansy[i], ship.team == 1 ? TransparencyDegree.BarelySee : TransparencyDegree.FullTransparancy);
        }


        successfulBecameInvisibleCallback?.Invoke();
        becomeInvisibleCoroutine = null;
    }


    /// <summary>
    /// Запускает процесс появления невидимки.
    /// </summary>
    public void BecameVisible(Action becameVisible)
    {
        if (becomeInvisibleCoroutine != null)
        {
            StopCoroutine(becomeInvisibleCoroutine);
            becomeInvisibleCoroutine = null;
            //Debug.Log($"StopCoroutine(becomeInvisibleCoroutine)");
        }

        if (becomeVisibleCoroutine == null)
        {
            becomeVisibleCoroutine = StartCoroutine(BecomeVisible(becameVisible));
        }
    }

    private IEnumerator BecomeVisible(Action becameVisible)
    {
        var colorTransparent = spriteRenderer.color;
        var colorBacklightTransparent = backLightRenderer.color;
        var colorHealthBarTransparent = healthBarRenderer.color;
        var colorHealthBackgroundBarTransparent = healthBackgroundBarRenderer.color;
        var colorUSPBarTransparent = USPBarRenderer.color;

        float timeToBecameVisible;
        float currentTime = timeToBecameVisible = References.Instance.settings.timeToBecameVisible;

        float t = 1;

        while (t > 0)
        {
            if (evasionCoroutine != null)
            {
                currentTime -= Time.deltaTime;
                t = currentTime / timeToBecameVisible;
                yield return null;
            }


            spriteRenderer.color = Color.Lerp(originalShipColor, colorTransparent, t);
            backLightRenderer.color = Color.Lerp(originalShipBacklightColor, colorBacklightTransparent, t);
            healthBarRenderer.color = Color.Lerp(originalShipHealthBarColor, colorHealthBarTransparent, t);
            healthBackgroundBarRenderer.color = Color.Lerp(originalShipHealthBackgroundBarColor, colorHealthBackgroundBarTransparent, t);
            USPBarRenderer.color = Color.Lerp(originalShipUSPBarColor, colorUSPBarTransparent, t);

            currentTime -= Time.deltaTime;
            t = currentTime / timeToBecameVisible;
            yield return null;
        }

        spriteRenderer.color = originalShipColor;
        backLightRenderer.color = originalShipBacklightColor;
        healthBarRenderer.color = originalShipHealthBarColor;
        healthBackgroundBarRenderer.color = originalShipHealthBackgroundBarColor;
        USPBarRenderer.color = originalShipUSPBarColor;

        for (int i = 0; i < visualEffects_PS_ForTransparansy.Count; i++)
        {
            SetTransparancyForParticleSystem(visualEffects_PS_ForTransparansy[i], TransparencyDegree.FullVisibility);
        }

        becomeVisibleCoroutine = null;
        becameVisible?.Invoke();
    }

    ///// <summary>
    ///// Регистрирует визуальный эффект в список эффектов и назначает ему прозрачность, реализованных в системе частиц, представленных в префабах как GameObject, и подлежащих учету при назначении прозрачности в Stealth_Module.
    ///// Для длительных эффектов!
    ///// </summary>
    ///// <param name="particleSystemGO"></param>
    //private void RegisterAndSetTransparansyForVisualEffect(GameObject particleSystemGO)
    //{
    //    ParticleSystem particleSystem;
    //    if (!particleSystemGO.TryGetComponent(out particleSystem))
    //    {
    //        Debug.LogWarning("There is no particle system!!!");
    //    }
    //    else
    //    {
    //        RegisterAndSetTransparansyForVisualEffect(particleSystem);
    //    }
        
    //}

    /// <summary>
    /// Регистрирует визуальный эффект в список эффектов и назначает ему прозрачность, реализованных в системе частиц, представленных в префабах как ParticleSystem, и подлежащих учету при назначении прозрачности в Stealth_Module.
    /// Для длительных эффектов!
    /// </summary>
    /// <param name="particleSystem"></param>
    private void RegisterAndSetTransparansyForVisualEffect(ParticleSystem particleSystem)
    {
        if (!visualEffects_PS_ForTransparansy.Contains(particleSystem))
        {
            visualEffects_PS_ForTransparansy.Add(particleSystem);
        }

        DefineAndSetTransparancyForVisualEffect(particleSystem);
    }


    ///// <summary>
    ///// Определяет и устанавливает прозрачность для визуального эффекта в зависимости от состояния корабля (видим/невидим) и команды.
    ///// </summary>
    ///// <param name="particleSystem"></param>
    //private void DefineAndSetTransparancyForVisualEffect(GameObject gameObjectPS)
    //{
    //    ParticleSystem particleSystem;
    //    if (!gameObjectPS.TryGetComponent(out particleSystem))
    //    {
    //        Debug.LogWarning("There is no particle system!!!");
    //    }
    //    else
    //    {
    //        DefineAndSetTransparancyForVisualEffect(particleSystem);
    //    }
    //}

    /// <summary>
    /// Определяет и устанавливает прозрачность для визуального эффекта в зависимости от состояния корабля (видим/невидим) и команды.
    /// </summary>
    /// <param name="particleSystem"></param>
    private void DefineAndSetTransparancyForVisualEffect(ParticleSystem particleSystem)
    {
        if (ship.IsInvisible)
        {
            if (ship.team != 1)
            {
                SetTransparancyForParticleSystem(particleSystem, TransparencyDegree.FullTransparancy);
            }
            else
            {
                SetTransparancyForParticleSystem(particleSystem, TransparencyDegree.BarelySee);
            }
        }
        else
        {
            SetTransparancyForParticleSystem(particleSystem, TransparencyDegree.FullVisibility);
        }
    }

    ///// <summary>
    ///// Назначает прозрачность для различных визуальных эффектов, сделанных через ParticleSystem. Необходимо в основном для кораблей со Stealth_Module.
    ///// </summary>
    ///// <param name="gameObject"></param>
    ///// <param name="transparencyDegree"></param>
    //private void SetTransparancyForParticleSystem(GameObject gameObject, TransparencyDegree transparencyDegree)
    //{
    //    ParticleSystem particleSystem;
    //    if (!gameObject.TryGetComponent(out particleSystem))
    //    {
    //        Debug.LogWarning("There is no particle system!!!");
    //    }
    //    else
    //    {
    //        SetTransparancyForParticleSystem(particleSystem, transparencyDegree);
    //    }
    //}

    /// <summary>
    /// Назначает прозрачность для различных визуальных эффектов, сделанных через ParticleSystem. Необходимо в основном для кораблей со Stealth_Module.
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <param name="transparencyDegree"></param>
    private void SetTransparancyForParticleSystem(ParticleSystem particleSystem, TransparencyDegree transparencyDegree)
    {
        //Debug.Log($"ship: {ship.name}, particleSystem: {particleSystem.name}, transparencyDegree: {transparencyDegree}");
        var alphaChannel = transparencyDegree switch
        {
            TransparencyDegree.BarelySee => 0.2f,
            TransparencyDegree.FullTransparancy => 0f,
            TransparencyDegree.FullVisibility => 1f,
            _ => throw new NotImplementedException()
        };

        var mainModule = particleSystem.main;
        ParticleSystem.MinMaxGradient minMaxGradient = mainModule.startColor;
        minMaxGradient.color = new Color(minMaxGradient.color.r, minMaxGradient.color.g, minMaxGradient.color.b, alphaChannel);
        mainModule.startColor = minMaxGradient;
    }

    private void SetTransparancyForTrailRenderer(TrailRenderer trailRenderer, TransparencyDegree transparencyDegree)
    {
        var alphaKeys = trailRenderer.colorGradient.alphaKeys;

        var alphaChannel = transparencyDegree switch
        {
            TransparencyDegree.BarelySee => 0.2f,
            TransparencyDegree.FullTransparancy => 0f,
            TransparencyDegree.FullVisibility => 1f,
            _ => throw new NotImplementedException()
        };

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alphaChannel;
        }
    }

    #endregion



    #region Patriot module


    public virtual void PatriotModuleEffect()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        var patriotEffect = Instantiate(Prefabs.Instance.patriotModuleEffect);
        patriotEffect.transform.parent = ship.transform;
        patriotEffect.transform.localPosition = new Vector3(0, 0, -5);
        patriotEffect.transform.localScale = Vector3.one;

        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystem(patriotEffect, TransparencyDegree.BarelySee);
        //}

        //DefineAndSetTransparancyForVisualEffect(patriotEffect);
        VisualEffectCreated?.Invoke(patriotEffect, VisualEffectDuration.Short);

    }

    public virtual void PatriotModuleUltimateEffect()
    {
        //if (ship.IsInvisible && ship.team != 1)
        //{
        //    return;
        //}

        var patriotEffect = Instantiate(Prefabs.Instance.patriotModuleUltimateEffect);
        patriotEffect.transform.parent = ship.transform;
        patriotEffect.transform.localPosition = new Vector3(0, 0, -5);
        patriotEffect.transform.localScale = Vector3.one;

        //if (ship.IsInvisible && ship.team == 1)
        //{
        //    SetTransparancyForParticleSystem(patriotEffect, TransparencyDegree.BarelySee);
        //}

        //DefineAndSetTransparancyForVisualEffect(patriotEffect);
        VisualEffectCreated?.Invoke(patriotEffect, VisualEffectDuration.Short);

    }



    #endregion



    #region Movespeed

    [SerializeField] private ParticleSystem engineFlameMain;
    [SerializeField] private ParticleSystem engineFlameSmoke;


    public void AddEngineFlameParticleSystem()
    {
        if (engineFlameMain == null)
        {
            engineFlameMain = Instantiate(Prefabs.Instance.engineFlame);
            engineFlameMain.transform.parent = ship.transform;
            engineFlameMain.transform.localPosition = new Vector3(0, -1, 5);
            engineFlameMain.transform.localScale = Vector3.one;
        }

        engineFlameSmoke = engineFlameMain.GetComponentsInChildren<ParticleSystem>()[1];
        engineFlameMain.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        //RegisterAndSetTransparansyForVisualEffect(engineFlameMain);
        //RegisterAndSetTransparansyForVisualEffect(engineFlameSmoke);

        VisualEffectCreated?.Invoke(engineFlameMain, VisualEffectDuration.Permanent);
        VisualEffectCreated?.Invoke(engineFlameSmoke, VisualEffectDuration.Permanent);
    }

    public void EngineFlameOn()
    {
        //if (ship.IsInvisible)
        //{
        //    if (ship.team != 1)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        SetTransparancyForParticleSystem(engineFlameMain, TransparencyDegree.BarelySee);
        //        SetTransparancyForParticleSystem(engineFlameSmoke, TransparencyDegree.BarelySee);
        //    }
        //}
        //else
        //{
        //    SetTransparancyForParticleSystem(engineFlameMain, TransparencyDegree.FullVisibility);
        //    SetTransparancyForParticleSystem(engineFlameSmoke, TransparencyDegree.FullVisibility);
        //}

        engineFlameMain.Play(true);
    }

    public void EngineFlameOff()
    {
        engineFlameMain.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    #endregion


    #region Vision Module

    public void VisionUltimateEffect(Ship enemyShip)
    {
        var effect = Instantiate(Prefabs.Instance.highlightEnemyShipEffect);
        effect.transform.SetParent(ship.transform);
        effect.transform.localScale = Vector3.one;
        effect.transform.position = ship.transform.position;

        StartCoroutine(HighligtingStealthEnemy(enemyShip, effect));

    }


    private IEnumerator HighligtingStealthEnemy(Ship enemyShip, ParticleSystem highlightEffect)
    {
        var size = highlightEffect.sizeOverLifetime;
        var circleOfLight = highlightEffect.GetComponentsInChildren<ParticleSystem>()[1];
        var shipTransform = ship.transform;
        var enemyTransform = enemyShip.transform;

        while (enemyShip != null && highlightEffect != null)
        {
            
            var vectorToEnemy = enemyTransform.position - shipTransform.position;
            float angle = Vector2.SignedAngle(Vector2.up, vectorToEnemy);
            highlightEffect.transform.localRotation = Quaternion.Euler(0, 0, angle - shipTransform.eulerAngles.z);
            var distance = Vector2.Distance(shipTransform.position, enemyTransform.position);
            //Debug.Log($"distance: {distance}");
            size.yMultiplier = distance;
            circleOfLight.transform.position = new Vector3(enemyTransform.position.x, enemyTransform.position.y, circleOfLight.transform.position.z);

            yield return null;
        }
    }

    #endregion
}
