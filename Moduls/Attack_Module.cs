using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

namespace MODULES
{

    public class Attack_Module : Module
    {
        public Coroutine toFightCoroutine;
        public float timeToSearchingEnemy = 5;

        [Tooltip("Расстояние перед кораблем, на котором создается контейнер снарядов.")]
        public float distanceFrontFromShipToCreateProjectile = 0.25f;

        /// <summary>
        /// Срабатывает после создания снаряда и придания ему всех характеристик.
        /// </summary>
        public event Action<ProjectileСontainer, Module> onProjectileCreated;

        [SerializeField] private bool canReachEnemy = false;
        /// <summary>
        /// Показывает достает ли корабль до врага. То есть расстояние до врага сейчас меньше, чем расстояние удара?
        /// Обновляется в процессе боя.
        /// </summary>
        public bool CanReachEnemy { get => canReachEnemy; private set => canReachEnemy = value; }

        private void Awake()
        {
            moduleType = Moduls.ATTACK_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.attack_damage = Mathf.RoundToInt(ModuleData[LevelOfModule]);
            ship.Enemys_detected += EnemyDetected;
            ship.search_for_enemy += SearchForEnemy;
        }

        private new static readonly float[] ModuleData =
        {
            0,
            10f,    10.1f,  10.2f,  10.3f,  10.41f, 10.51f, 10.62f, 10.72f, 10.83f, 10.94f,
            11.05f, 11.16f, 11.27f, 11.38f, 11.49f, 11.61f, 11.73f, 11.84f, 11.96f, 12.08f,
            12.2f,  12.32f, 12.45f, 12.57f, 12.7f,  12.82f, 12.95f, 13.08f, 13.21f, 13.35f,
            13.48f, 13.61f, 13.75f, 13.89f, 14.03f, 14.17f, 14.31f, 14.45f, 14.6f,  14.74f,
            14.89f, 15.04f, 15.19f, 15.34f, 15.49f, 15.65f, 15.8f,  15.96f, 16.12f, 16.28f,
            16.45f, 16.61f, 16.78f, 16.94f, 17.11f, 17.29f, 17.46f, 17.63f, 17.81f, 17.99f,
            18.17f, 18.35f, 18.53f, 18.72f, 18.9f,  19.09f, 19.28f, 19.48f, 19.67f, 19.87f,
            20.07f, 20.27f, 20.47f, 20.68f, 20.88f, 21.09f, 21.3f,  21.52f, 21.73f, 21.95f,
            22.17f, 22.39f, 22.61f, 22.84f, 23.07f, 23.3f,  23.53f, 23.77f, 24f,    24.24f,
            24.49f, 24.73f, 24.98f, 25.23f, 25.48f, 25.74f, 25.99f, 26.25f, 26.52f, 26.78f,
            27.58f, 28.41f, 29.26f, 30.14f, 31.05f, 31.98f, 32.94f, 33.92f, 34.94f, 35.99f,

        };

        public override int LevelOfModule
        {
            get
            {
                return levelOfModule;
            }
            set
            {
                if (value < 1)
                    levelOfModule = 1;
                else if (value > ModuleData.Length - 1)
                    levelOfModule = ModuleData.Length - 1;
                else
                    levelOfModule = value;
            }
        }
        public static int GetMaxLevel() => ModuleData.Length - 1;

        private void EnemyDetected(List<Ship> enemyShip, List<Ship> _)
        {
            if (ship.State == Ship.States.IDLE || ship.State == Ship.States.TO_USP || ship.State == Ship.States.SEARCHING_FOR_ENEMY)
            {
                //print("5. Att_Mod Attack the enemy. ship: " + ship.gameObject.GetInstanceID());
                if (IsThereCloserEnemy()) { toFightCoroutine = StartCoroutine(ToFight(ship.enemyCurrent)); /*print("new enemy!");*/ }
            }
        }

        /// <summary>
        /// Проверяет есть ли более близкий враг
        /// </summary>
        /// <returns></returns>
        private bool IsThereCloserEnemy()
        {
            var distancesSqrAndShips = new Dictionary<float, Ship>();
            ship.discoveredEnemies = ship.discoveredEnemies.Except(ship.discoveredEnemies.Where(ship => ship == null || ship.IsInvisible)).ToList();
            
            for (int i = 0; i < ship.discoveredEnemies.Count; i++)
            {
                var distanceSqr = (transform.position - ship.discoveredEnemies[i].transform.position).sqrMagnitude;
                if (!distancesSqrAndShips.Keys.Contains(distanceSqr))
                    distancesSqrAndShips.Add(distanceSqr, ship.discoveredEnemies[i]);
            }

            if (distancesSqrAndShips.Count > 0)
            {
                var minDistance = distancesSqrAndShips.Keys.Min<float>();
                var enemy = distancesSqrAndShips[minDistance];
                ship.enemyCurrent = enemy;

                return true;
            }
            else
            {
                return false;
            }
        }



        public IEnumerator ToFight(Ship enemyShip)
        {
            if (enemyShip == null || ship.team == enemyShip.team)
            {
                toFightCoroutine = null;
                if (ship.State == Ship.States.FIGHT)
                    ship.Idle();
                yield break;
            }



            ship.enemyCurrent = enemyShip;
            ship.State = Ship.States.FIGHT;

            float attackDistanceSqr = ship.enemyCurrent.radiusSize + ship.radiusSize + ship.attack_range;
            attackDistanceSqr *= attackDistanceSqr;

            while (ship.enemyCurrent != null && !ship.enemyCurrent.IsInvisible && ship.State == Ship.States.FIGHT)
            {
                IsThereCloserEnemy();
                yield return StartCoroutine(GettingIntoPosition());
                //print("1.");
                if (IsSomethingWrong()) yield break;

                Vector2 attackVector = transform.position - ship.enemyCurrent.transform.position;
                ship.rotation_Controller.Rotate(-attackVector);
                while (ship.rotation_Controller.rotatingCoroutine != null) yield return null; //ждем пока повернется, иначе иногда стреляет боком.
                float distanceSqr = attackVector.sqrMagnitude;
                //print("distanceSqr = " + distanceSqr + "    attackDistance = " + attackDistanceSqr);

                //сравниваем квадрат расстояния между краями коллайдеров микротов и квадрат дистанции атаки
                //квадраты, а не оригиналы, чтобы быстрее считал, т.к. корень задерживает рассчет
                if (distanceSqr > attackDistanceSqr)
                {
                    CanReachEnemy = false;
                    ship.moveDirection = (-attackVector).normalized;
                    yield return new WaitForSeconds(0.5f);
                    //print("2.");

                    if (IsSomethingWrong()) yield break;

                    toFightCoroutine = StartCoroutine(ToFight(ship.enemyCurrent));
                    yield break;
                }
                else
                {
                    CanReachEnemy = true;
                }

                //print("3.");

                if (IsSomethingWrong()) yield break;

                //print("GOT HIM!!!");
                if (ship.State == Ship.States.FIGHT)
                {
                    //ship.DeselerateSmoothly();        // не очень подходит, потому что в бою большого количества кораблей выглядит наоборот не очень естественно,
                                                        // и плюс, корабли слишком много толкают лишний раз союзников
                    ship.moveDirection = Vector2.zero;
                    ship.InvokeMoveDirectionZeroEvent();
                }

                //если готов бить, то бьет и ждем время, зависящее от скорости атаки
                if (ship.isReadyToHit)
                {
                    yield return StartCoroutine(HitEnemy());
                }
                //если не готов бить, то ждем пока не будет готов, затем бьет
                else
                {
                    while (!ship.isReadyToHit && ship.State == Ship.States.FIGHT)
                        yield return null;

                    if (IsSomethingWrong()) yield break;

                    //HitEnemy();
                    yield return StartCoroutine(HitEnemy());

                }

                //Debug.Log($"Attack module reload_time: {ship.reload_time}. frame: {Time.frameCount}");
                yield return new WaitForSeconds(ship.reload_time);
            }

            if (ship.State != Ship.States.STUNNED)
            {
                ship.enemyCurrent = null;
                toFightCoroutine = null;
                CanReachEnemy = false;
                //ship.isDirectSightEnemy = false;
                if (ship.State == Ship.States.FIGHT)
                    ship.Idle();
            }
        }

        /// <summary>
        /// Проверяет свою позицию по отношению к врагу и обходит препятствия, если есть необходимость.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GettingIntoPosition()
        {
            while (ship.enemyCurrent != null && !Global_Controller.IsDirectSightShip(ship, ship.enemyCurrent/*, out _*/))
            {
                IsThereCloserEnemy();
                yield return ship.avoiding_Obstacle.avoidShipCoroutine = ship.avoiding_Obstacle.StartCoroutine(ship.avoiding_Obstacle.AvoidObstacle(ship.enemyCurrent.gameObject));
            }
        }

        /// <summary>
        /// Проверяет состояние корабля и переменных после какого-либо действия, чтобы не было ошибок.
        /// </summary>
        private bool IsSomethingWrong()
        {
            if (ship.State != Ship.States.FIGHT)
            {
                //print("ship: " + ship.gameObject.name + ". Ship state != FIGHT");
                toFightCoroutine = null;
                return true;
            }

            if (ship.enemyCurrent == null)
            {
                //print("ship: " + ship.gameObject.name + ". enemy == null");
                toFightCoroutine = null;
                CanReachEnemy = false;

                if (ship.State == Ship.States.FIGHT)
                    ship.Idle();
                return true;
            }

            if (!Global_Controller.MustSeeShip(ship, ship.enemyCurrent))
            {
                //print("ship: " + ship.gameObject.name + ". NON must see enemy");
                ship.enemyCurrent = null;
                toFightCoroutine = null;
                if (ship.State == Ship.States.FIGHT)
                    ship.Idle();
                return true;
            }

            return false;
        }


        private IEnumerator HitEnemy()
        {
            while (ship.rotation_Controller.rotatingCoroutine != null)
            {
                yield return null;
            }

            if (ship.enemyCurrent == null)
                yield break;

            //Projectile _projectile = Instantiate(Prefabs.Instance.projectile);
            //beforeProjectileCreate?.Invoke(this);
            //DamageProjectile _projectile = Instantiate(currentProjectile) as DamageProjectile;

            ProjectileСontainer projectileContainer = Instantiate(Prefabs.Instance.projectileContainer);
            DamageProjectile damageProjectile = Instantiate(Prefabs.Instance.damageProjectile);

            //_projectile.team = ship.team;
            int damage = ship.attack_damage;
            if(UltimateImpactAction())
            {
                damage *= 2;
                damageProjectile.AttackUltimateVisionEffect();
            }

            projectileContainer.damage = damageProjectile.damage = damage;
            //_projectile.chanceToStun = ship.chanceToStun;
            damageProjectile.shipWhoFired = ship;

            Vector2 heading = ship.enemyCurrent.transform.position - transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            var projectilePosition = PositionOfProjectile(distance);
            while(float.IsNaN(projectilePosition.x))
            {
                print("X coordinate of projectile is NaN! ship is " + gameObject.name + "    transform = " + gameObject.transform);
                yield return null;

                if (ship.enemyCurrent == null)
                    yield break;
                heading = ship.enemyCurrent.transform.position - transform.position;
                distance = heading.magnitude;
                projectilePosition = PositionOfProjectile(distance);
            }

            projectileContainer.transform.position = projectilePosition;
            //_projectile.transform.parent = ship.battle_Scene_Controller?.Projectiles;
            projectileContainer.transform.parent = BattleSceneController.Instance?.Projectiles_Transform;
            damageProjectile.direction = projectileContainer.direction = direction;
            projectileContainer.AddProjectile(damageProjectile);

            onProjectileCreated?.Invoke(projectileContainer, this);
            ship.enemyCurrent.OnShotDetected();

            //projectileContainer.GoTo(direction);

            ship.Hit();//удар произведен

            Vector2 PositionOfProjectile(float _distance)
            {
                float distanceFromOrigin = ship.radiusSize + distanceFrontFromShipToCreateProjectile;//m1
                float distanceToDestination = _distance - distanceFromOrigin;//m2
                float x1 = transform.position.x;
                float x2 = ship.enemyCurrent.transform.position.x;
                float y1 = transform.position.y;
                float y2 = ship.enemyCurrent.transform.position.y;
                float X = (distanceToDestination * x1 + distanceFromOrigin * x2) / (distanceFromOrigin + distanceToDestination);
                float Y = (distanceToDestination * y1 + distanceFromOrigin * y2) / (distanceFromOrigin + distanceToDestination);
//#pragma warning disable CS1718 // Выполнено сравнение с той же переменной
//                X = X == X ? X : 100;
//#pragma warning restore CS1718 // Выполнено сравнение с той же переменной
//#pragma warning disable CS1718 // Выполнено сравнение с той же переменной
//                Y = Y == Y ? Y : 100;//проверка X и Y на NaN. Если не равны сами себе, значит NaN и тогда возвращаем результом значения где-то за экраном.
//#pragma warning restore CS1718 // Выполнено сравнение с той же переменной
                

                return new Vector2(X, Y);
            }


        }

        


        private void SearchForEnemy(Vector2 directionFromProjectile)
        {
            if (ship.State != Ship.States.SEARCHING_FOR_ENEMY /*|| ship.State == Ship.States.TO_USP) && searchingForEnemyCoroutine == null*/)
            {
                Vector2 moveDirection = -directionFromProjectile.normalized;
                ship.State = Ship.States.SEARCHING_FOR_ENEMY;
                ship.rotation_Controller.Rotate(moveDirection);
                /*searchingForEnemyCoroutine = */StartCoroutine(SearchingForEnemy(moveDirection));
            }
        }

        private IEnumerator SearchingForEnemy(Vector2 moveDirection)
        {
            ship.moveDirection = moveDirection;
            float time = timeToSearchingEnemy;
            while(ship.State == Ship.States.SEARCHING_FOR_ENEMY && time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            if (ship.State == Ship.States.SEARCHING_FOR_ENEMY) ship.Idle();
            //searchingForEnemyCoroutine = null;
        }


    }
}