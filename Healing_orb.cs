using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

/// <summary>
/// Летит к кораблю и подлечивает его при достижении.
/// </summary>
public class Healing_orb: MonoBehaviour, INeedUpdate
{

    [SerializeField] private int heal_value;
    [SerializeField] private Ship targetShip;

    [SerializeField] private float speed;
    /// <summary>
    /// Увеличивать или нет максимальное ХП. Ультимейт.
    /// </summary>
    [SerializeField] private bool increaseMaxHealth = false;

    /// <summary>
    /// На сколько увеличивать макс ХП.
    /// </summary>
    [SerializeField] private int increasingMaxHealthValue;

    /// <summary>
    /// Максимальное значение макс ХП, выше которого не будет увеличиваться макс ХП.
    /// </summary>
    [SerializeField] private int maxHealthMaxValue;


    private float sqrRadiusOfTargetShip;



    public void Initialize(int heal_value, Ship targetShip, Vector3 startPosition, bool increaseMaxHealth, int increasingMaxHealthValue, int maxHealthMaxValue)
    {
        this.heal_value = heal_value;
        this.targetShip = targetShip;
        transform.position = startPosition;
        sqrRadiusOfTargetShip = targetShip.radiusSize * targetShip.radiusSize;
        this.increaseMaxHealth = increaseMaxHealth;
        this.increasingMaxHealthValue = increasingMaxHealthValue;
        this.maxHealthMaxValue = maxHealthMaxValue;

        Updater.Instance.RegisterNeedUpdateObject(this);
    }


    //private void Update()
    //{
    //    if (targetShip != null)
    //    {
    //        Vector2 heading = targetShip.transform.position - transform.position;
    //        float distance = heading.magnitude;
    //        Vector2 direction = heading / distance;


    //        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    //        if ((transform.position - targetShip.transform.position).sqrMagnitude < sqrRadiusOfTargetShip)
    //        {
    //            HealTargetShip(direction);
    //        }
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    public void UpdateMe()
    {
        if (targetShip != null)
        {
            Vector2 heading = targetShip.transform.position - transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;


            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            if ((transform.position - targetShip.transform.position).sqrMagnitude < sqrRadiusOfTargetShip)
            {
                HealTargetShip(direction);
            }
        }
        else
        {
            Updater.Instance.UnregisterNeedUpdateObject(this);
            Destroy(gameObject);
        }
    }

    private void HealTargetShip(Vector2 directionOfImpact)
    {
        if (targetShip != null)
        {
            if (increaseMaxHealth)
            {
                int gipoteticalMaxHP = targetShip.healthMax + increasingMaxHealthValue;
                targetShip.healthMax = gipoteticalMaxHP > maxHealthMaxValue ? maxHealthMaxValue : gipoteticalMaxHP;
                Debug.Log($"ship: {targetShip.name}, maxHP: {targetShip.healthMax}");

            }

            targetShip.takeHitComponent.HealProjectileHit(null, heal_value, 0, directionOfImpact, transform.position);
            Updater.Instance.UnregisterNeedUpdateObject(this);
            Destroy(gameObject);
        }
    }

    
}
