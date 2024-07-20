using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using System;

/// <summary>
/// Компонент, отвечающий за получение урона.
/// </summary>
public class ShipTakeHit : MonoBehaviour, ICanTakeHit
{
    [SerializeField] private Ship ship;
    //public Action<int, Vector2, Vector3, Action> Take_Damage;

    /// <summary>
    /// Может скорректировать наносимый урон.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    /// <param name="impactPoint"></param>
    /// <returns></returns>
    public delegate (int damage, Action visualEffect) ImpactOnDamageValue(int damage, Vector2 direction, Vector3 impactPoint);

    /// <summary>
    /// Вызывается после начала получения урона, но до вычисления фактического значения. 
    /// Таким образом через это событие есть возможность повлиять на получаемый урон.
    /// Аргументом является входящий урон. Результат - кортеж (урон, цвет вылетающего числа урона).
    /// </summary>
    public event ImpactOnDamageValue impactOnDamageValue;

    public delegate bool ChanceToEvation();
    public ChanceToEvation IsEvaded { get; set; } = delegate { return false; };

    public event Func<Ship, int, Vector2, Vector3, bool> CheckForAncileDamage;
    public event Func<Ship, float, Vector2, Vector3, bool> CheckForAncileStun;


    /// <summary>
    /// Вызывается при получении урона.
    /// </summary>
    public Action DamageTaked;


    public bool DamageProjectileHit(Ship shipWhoFired, int damage, Vector2 direction, Vector3 impactPoint)
    {
        if (shipWhoFired != null)
            ship.shipLastToDoDamage = shipWhoFired;
        else
            ship. shipLastToDoDamage = null;

        if (ship.IsStateFree)
        {
            ship.search_for_enemy?.Invoke(direction);
        }

        if(CheckForAncileDamage != null && CheckForAncileDamage.Invoke(shipWhoFired, damage, direction, impactPoint))
        {
            return true;
        }

        if (IsEvaded())
        {
            return false;
        }

        if (impactOnDamageValue != null)
        {
            var results = impactOnDamageValue(damage, direction, impactPoint);
            ship.HealthCurrent -= results.damage;
            results.visualEffect.Invoke();
        }
        else
        {
            ship.HealthCurrent -= damage;
            ship.shipVisualController.ExplosionAndDebrisWhenDamageTaken(damage, direction, impactPoint);
        }

        DamageTaked?.Invoke();
        return true;
    }


    public bool StunProjectileHit(Ship shipWhoFired, float stunTime, Vector2 direction, Vector3 impactPoint)
    {
        if (shipWhoFired != null)
            ship.shipLastToDoDamage = shipWhoFired;
        else
            ship.shipLastToDoDamage = null;

        if (CheckForAncileStun != null && CheckForAncileStun.Invoke(shipWhoFired, stunTime, direction, impactPoint))
        {
            return true;
        }

        if (IsEvaded())
        {
            return false;
        }

        ship.StunProjectileHit(shipWhoFired, stunTime, direction, impactPoint);
        return true;
    }



    public void HealProjectileHit(Ship shipWhoFired, int healPoints, float chanceToDebuff, Vector2 direction, Vector3 impactPoint)
    {
        int _healApplied = (ship.HealthCurrent + healPoints > ship.healthMax) ? (ship.healthMax - ship.HealthCurrent) : healPoints;
        ship.HealthCurrent += healPoints;

        if (ship.isDebuffApplied && GameEngineAssistant.GetProbability(chanceToDebuff))
        {
            ship.isDebuffApplied = false;
            print("debuff is healed");
        }

        Global_Controller.Instance.StartCoroutine(Global_Controller.Instance.VisualizationOfDamage(_healApplied, ship.healthMax, transform.position, direction, References.Instance.colors.digits_healing_color));
        ship.shipVisualController.Heal();
    }








}

