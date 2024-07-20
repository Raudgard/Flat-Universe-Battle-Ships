using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tools;

/// <summary>
/// �������� ���� ��� ��������� ��������. ����� � ����, ������ ��� ������� �� ��� Z.
/// </summary>
public class Projectile�ontainer : MonoBehaviour, INeedUpdate, INeedFixUpdate
{
    public float speed;
    public Vector2 direction;
    public float rotationSpeed;
    [SerializeField] private ProjectilePositionsInContainer[] projectilePositions;


    public Projectile[] projectiles;
    private Transform _transform;
    private int projectilesCount = 0;

    [Tooltip("����, ������������� ��������� (�������) �������.")]
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
/// ������������ �������� ������ ����������.
/// </summary>
[System.Serializable]
public class ProjectilePositionsInContainer
{
    public Vector2[] localPositions;

    //���� ������ ����, �� �� ������������� � ������ ����������. ���� ������ ������, �� ������� ������������� �� ����������.

    //������� �� ����������:
    //��� 2 ��������: 0, 180.
    //��� 3 ��������: 90, 210, 330.
    //��� 4 ��������: 45, 135, 225, 315.
    //��� 5 ��������: 18, 90, 162, 234, 306.
    //��� 6 ��������: 0, 60, 120, 180, 240, 300.

}
