using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tools;

/// <summary>
/// Содержит один или несколько снарядов. Летит к цели, вращая эти снаряды по оси Z.
/// </summary>
public class ProjectileСontainer : MonoBehaviour, INeedUpdate, INeedFixUpdate
{
    public float speed;
    public Vector2 direction;
    public float rotationSpeed;
    [SerializeField] private ProjectilePositionsInContainer[] projectilePositions;


    public Projectile[] projectiles;
    private Transform _transform;
    private int projectilesCount = 0;

    [Tooltip("Урон, присваиваемый основному (первому) снаряду.")]
    public int damage;

    private void Awake()
    {
        _transform = transform;
        Updater.Instance.RegisterNeedUpdateObject(this as INeedUpdate);
        Updater.Instance.RegisterNeedUpdateObject(this as INeedFixUpdate);
    }

    public void AddProjectile(Projectile projectile)
    {
        projectile.transform.SetParent(transform);
        projectile.collisionEvent += CheckProjectileAfterCollision;
        projectiles[projectilesCount] = projectile;
        var positions = projectilePositions[projectilesCount];
        projectilesCount++;

        for (int i = 0; i < projectilesCount; i++)
        {
            projectiles[i].projectileTransform.localPosition = positions.localPositions[i];
        }
    }



    public void UpdateMe()
    {
        _transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

    }

    public void FixUpdateMe()
    {
        _transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
    }



    private void CheckProjectileAfterCollision()
    {
        projectilesCount--;

        if (projectilesCount == 0)
        {
            Updater.Instance.UnregisterNeedUpdateObject(this as INeedFixUpdate);
            Updater.Instance.UnregisterNeedUpdateObject(this as INeedUpdate);
            Destroy(gameObject);
        }
    }

}


/// <summary>
/// Расположение снарядов внутри контейнера.
/// </summary>
[System.Serializable]
public class ProjectilePositionsInContainer
{
    public Vector2[] localPositions;

    //Если снаряд один, то он располагается в центре окружности. Если больше одного, то снаряды располагаются на окружности.

    //градусы на окружности:
    //для 2 снарядов: 0, 180.
    //для 3 снарядов: 90, 210, 330.
    //для 4 снарядов: 45, 135, 225, 315.
    //для 5 снарядов: 18, 90, 162, 234, 306.
    //для 6 снарядов: 0, 60, 120, 180, 240, 300.

}
