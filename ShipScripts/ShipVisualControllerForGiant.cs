using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using UnityEngine.Scripting;
using System;

/// <summary>
/// Контролирует цвет спрайта корабля в зависимости от его состояния и случившихся событий.
/// </summary>
public class ShipVisualControllerForGiant : ShipVisualController
{
    #region Initialization

    private Colors colors;
    private GameEngineAssistant gameEngineAssistant;
    private GameObject debrisFromDamage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colors = References.Instance.colors;
        ship = GetComponent<Ship>();
        gameEngineAssistant = References.Instance.gameEngineAssistant;
        //canvas = GetComponentInChildren<Canvas>();
        backLightRenderer = gameObject.transform.Find("Backlight").GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Damage taken

    //[SerializeField] private float timeOfDamageTakenColor;

    public override void ExplosionAndDebrisWhenDamageTaken(int damage, Vector2 direction, Vector3 impactPoint)
    {
        GameObject debris = Instantiate(debrisFromDamage);

        debris.transform.position = impactPoint;
        //debris.transform.parent = ship.battle_Scene_Controller.Debris;
        debris.transform.parent = BattleSceneController.Instance?.Debris_Transform;

        var debrisSizeCoef = DebrisSize(ship.healthMax, damage);
        debris.transform.localScale *= ship.changedSize * debrisSizeCoef;

        StartCoroutine(DamageVisualisation());
    }

    private IEnumerator DamageVisualisation()
    {
        //spriteRenderer.color = colors.damage_color;

        yield return new WaitForSecondsRealtime(References.Instance.settings.timeOfDamageTakenColor);

        if (gameObject != null)
        {
            if (ship.State == Ship.States.REPRODUCTION || ship.State == Ship.States.DIGESTION)
            {
                spriteRenderer.color = colors.USP_color;
            }
            else
                spriteRenderer.color = Color.white;
        }
    }

    #endregion

    #region Reproduction

    public override void ReproductionBegun()
    {
        spriteRenderer.color = colors.USP_color;
    }

    public override void ReproductionEnd()
    {
        spriteRenderer.color = Color.white;
    }

    #endregion

    #region USP taken

    public override void USP_Taken()
    {
        //StartCoroutine(USP_Taken_Visualisation());

        var USPTakeEffect = Instantiate(Prefabs.Instance.USPTakeEffect);
        USPTakeEffect.transform.parent = ship.transform;
        USPTakeEffect.transform.localPosition = new Vector3(0, 0, -5);
        USPTakeEffect.transform.localScale = Vector3.one;

        //giant part
        ParticleSystem particle = USPTakeEffect.GetComponent<ParticleSystem>();
        var main = particle.main;
        main.startSizeMultiplier = ship.changedSize * 0.4f;
        var minMaxCurve = particle.emission.GetBurst(0).count;
        minMaxCurve.constant *= ship.changedSize;

        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.velocityScale *= ship.changedSize; //в Инспекторе speedScale.
    }

    #endregion

    #region Digestion
    public void DigestionStart()
    {
        spriteRenderer.color = colors.USP_color;
    }

    public void DigestionEnd()
    {
        spriteRenderer.color = Color.white;
    }

    #endregion

    #region Heal

    //[SerializeField] private GameObject healEffect;

    public override void Heal()
    {
        var healEffect = Instantiate(Prefabs.Instance.healEffect);
        healEffect.transform.parent = ship.transform;
        healEffect.transform.localPosition = new Vector3(0, 0, -5);
        healEffect.transform.localScale = Vector3.one;

        //giant part
        ParticleSystem particle = healEffect.GetComponent<ParticleSystem>();
        var main = particle.main;
        main.startSizeMultiplier = ship.changedSize * 0.4f;
        var minMaxCurve = particle.emission.GetBurst(0).count;
        minMaxCurve.constant *= ship.changedSize;
        
        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.velocityScale *= ship.changedSize; //в Инспекторе speedScale.
    }

    #endregion

    #region Stun

    private Ice_anim_controller ice_Anim_Controller = null;
    //[SerializeField] private GameObject stunAnimationEffect;

    public override void Stun(float stunTime)
    {
        var stunEffect = Instantiate(Prefabs.Instance.stunAnimationEffect);
        stunEffect.transform.parent = ship.transform;
        stunEffect.transform.localPosition = new Vector3(0, 0, -3);
        stunEffect.transform.localScale = Vector3.one;

        ice_Anim_Controller = stunEffect.GetComponent<Ice_anim_controller>();
    }
    
    //public override void StunEnd()
    //{
    //    ice_Anim_Controller.Resume();
    //}

    #endregion

    #region Evasion

    //[SerializeField] private float speedOfVanishing;
    //[SerializeField] private float minAlphaChannel;

    public override void EvasionStart()
    {
        StartCoroutine(EvasionStartCoro());
    }

    private IEnumerator EvasionStartCoro()
    {
        var originalColor = spriteRenderer.color;
        var colorTrancelucent = new Color(originalColor.r, originalColor.g, originalColor.b, spriteRenderer.color.a / 5);
        var fixupdatetime = new WaitForFixedUpdate();
        float vanishingSpeed = References.Instance.settings.speedOfVanishing;
        float t = 1;

        while (t > 0)
        {
            spriteRenderer.color = Color.Lerp(colorTrancelucent, originalColor, t);
            t -= vanishingSpeed;
            yield return fixupdatetime;
        }
    }

    public override void EvasionEnd()
    {
        StartCoroutine(EvasionEndCoro());
    }

    private IEnumerator EvasionEndCoro()
    {
        var originalColor = spriteRenderer.color;
        var colorTrancelucent = new Color(originalColor.r, originalColor.g, originalColor.b, spriteRenderer.color.a / 5);
        var fixupdatetime = new WaitForFixedUpdate();
        float vanishingSpeed = References.Instance.settings.speedOfVanishing;
        float t = 0;

        while (t < 1)
        {
            spriteRenderer.color = Color.Lerp(colorTrancelucent, originalColor, t);
            t += vanishingSpeed;
            yield return fixupdatetime;

        }
    }

    #endregion

    #region Teleportation

    public override void Teleportation(Teleportation_Module TP_module, float teleportationSpeed, Action teleportationEndAction)
    {
        StartCoroutine(TeleportartionGo(TP_module, teleportationSpeed, teleportationEndAction));
    }

    private IEnumerator TeleportartionGo(Teleportation_Module TP_module, float teleportationSpeed, Action teleportationEndAction)
    {
        var _teleportation_Start = Instantiate(Prefabs.Instance.teleportationStartEffect, ship.transform);
        _teleportation_Start.transform.localPosition = new Vector3(0, 0, -2);

        //giant part
        var main = _teleportation_Start.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        main.startSizeMultiplier = ship.changedSize;

        yield return new WaitForSeconds(1);

        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        spriteRenderer.enabled = false;
        ship.shipBars.SetActive(false);
        backLightRenderer.enabled = false;

        //ship.mainCollider.enabled = false;
        _teleportation_Start.transform.SetParent(null);
        var moveDirectionTemp = ship.moveDirection;
        var newPosition = TP_module.GetNewPosition();
        ship.moveDirection = Vector2.zero;
        var teleportation_End = Instantiate(Prefabs.Instance.teleportationEndEffect, new Vector3(newPosition.x, newPosition.y, -2), Quaternion.identity);
        //teleportation_E.transform.localScale = new Vector3(ship.changedSize, ship.changedSize, ship.changedSize);
         
        //giant part
        //размер светового пятна
            main = teleportation_End.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startSizeMultiplier = ship.changedSize;

            //размер шара из частиц
            var curve = teleportation_End.GetComponent<ParticleSystem>().velocityOverLifetime.radial.curve;
            print("frame[0].value before = " + curve.keys[0].value);
            AnimationCurve animationCurve = new AnimationCurve(curve.keys);
            animationCurve.keys[0].value *= ship.changedSize;
            print("new animationCurve frame[0].value = " + animationCurve.keys[0].value);
            curve = animationCurve;
            //curve.keys[0].value *= ship.changedSize;
            print("frame[0].value after = " + curve.keys[0].value);


        yield return new WaitForSeconds(0.5f);

        ship.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
        //ship.mainCollider.enabled = enabled;
        teleportation_End.transform.parent = ship.transform;
        //spriteRenderer.color = Color.white;
        spriteRenderer.enabled = true;
        ship.shipBars.SetActive(true);
        backLightRenderer.enabled = true;

        if (!ship.movingFromForce)
        {
            //print("NOT moving from force");
            ship.moveDirection = moveDirectionTemp;
        }
    }

    #endregion
}
