using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using System.Linq;
using System;

namespace MODULES
{
    public class Medicus_Module : Module
    {
        //public float debuffHealChance = 0f;
        //private bool isDirectSightFrend = false;

        //private Avoiding_Obstacle avoidingComponent;
        //private Battle_Scene_Controller battle_Scene_Controller;
        private Global_Controller global_Controller;
        private GameEngineAssistant gameEngineAssistant;
        private Player_Data player_Data;
        //private Rotation_Controller rotation_Control;

        public Coroutine healingCoroutine;

        public Ship injuredShip = null;

        [Tooltip("Расстояние перед кораблем, на котором создается контейнер снарядов.")]
        public float distanceFrontFromShipToCreateProjectile = 0.25f;
        //private LayerMask layerOfShips;

        /// <summary>
        /// Срабатывает после создания снаряда и придания ему всех характеристик.
        /// </summary>
        public event Action<ProjectileСontainer, Module> onProjectileCreated;

        private void Awake()
        {
            moduleType = Moduls.MEDICUS_MODULE;
            //avoidingComponent = ship.avoiding_Obstacle;
            //rotation_Control = ship.rotation_Controller;
        }

        protected override void Start()
        {
            base.Start();
            //layerOfShips = LayerMask.GetMask("ShipLayer");
            
            global_Controller = Global_Controller.Instance;
            player_Data = References.Instance.player_Data;
            gameEngineAssistant = References.Instance.gameEngineAssistant;
            //if (battle_Scene_Controller != null)
            StartCoroutine(SearchingForInjured());
        }

        public static new int[] ModuleData =
        {
        0,
        5,  5,  5,  5,  5,  5,  5,  5,  5,  5,
        6,  6,  6,  6,  6,  6,  6,  6,  6,  6,
        6,  6,  6,  6,  6,  6,  6,  7,  7,  7,
        7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
        7,  8,  8,  8,  8,  8,  8,  8,  8,  8,
        8,  8,  8,  8,  9,  9,  9,  9,  9,  9,
        9,  9,  9,  9,  9,  10, 10, 10, 10, 10,
        10, 10, 10, 10, 10, 11, 11, 11, 11, 11,
        11, 11, 11, 11, 12, 12, 12, 12, 12, 12,
        12, 12, 12, 13, 13, 13, 13, 13, 13, 13,
        14, 14, 15, 15, 16, 16, 16, 17, 17, 18,

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

        public static float[] DebuffHealChance =
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
        27.58f, 28.41f, 29.26f, 30.14f, 31.05f, 31.98f, 32.94f, 33.92f, 34.94f, 35.99f
    };
        public static int GetMaxLevel() => ModuleData.Length - 1;


        private IEnumerator SearchingForInjured()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                if (ship.State == Ship.States.IDLE || ship.State == Ship.States.TO_USP)
                {
                    StartCoroutine(Search(global_Controller.ships[ship.team]));
                }
            }
        }



        private IEnumerator Search(List<Ship> frendTeam)
        {
            //float distanceSqr;
            //Ship frendShip;
            List<Ship> injuredFrends = frendTeam.Where(frendShip => IsFrendShipInjuredOrDebuff(frendShip)).ToList();

            uint jobNumber = gameEngineAssistant.JobsCounter;
            yield return gameEngineAssistant.StartCoroutine(gameEngineAssistant.CheckShipsInVisionRadius(ship, injuredFrends, jobNumber));
            var resultsOfJob = gameEngineAssistant.CheckShipsInVisionRadius_Results[jobNumber];

            for (int i = 0; i < resultsOfJob.Count; i++)
            {
                if (resultsOfJob[i] == ship)
                    continue;

                if (!Global_Controller.MustSeeShip(ship, resultsOfJob[i])) // проверка на то, что лежит между микротами. Если там есть что-то кроме микротов, то значит врага "не видно"
                {
                    //print("невозможно увидеть. Есть преграда");
                    continue;
                }

                if (healingCoroutine == null)
                {
                    injuredShip = resultsOfJob[i];
                    healingCoroutine = StartCoroutine(Healing());
                    break;
                }
            }

            gameEngineAssistant.CheckShipsInVisionRadius_Results.Remove(jobNumber);

        }




        /// <summary>
        /// Возвращает true, если дружеский микрот ранен; или если на нем есть дебафф, а у этого микрота debuffHealChance > 0.
        /// </summary>
        /// <param name="frendShip"></param>
        /// <returns></returns>
        private bool IsFrendShipInjuredOrDebuff(Ship frendShip)
        {
            if ((frendShip.HealthCurrent < frendShip.healthMax) || (frendShip.isDebuffApplied && DebuffHealChance[levelOfModule] > 0))
            {
                return true;
            }

            return false;
        }


       
        public IEnumerator Healing()
        {
            if (injuredShip == null || !IsFrendShipInjuredOrDebuff(injuredShip))
            {
                healingCoroutine = null;
                if (ship.State == Ship.States.HEALING)
                    ship.Idle();
                yield break;
            }

            ship.State = Ship.States.HEALING;

            float healingDistanceSqr = injuredShip.radiusSize + ship.radiusSize + ship.attack_range;
            healingDistanceSqr *= healingDistanceSqr;

            while (/*gameObject != null && */injuredShip != null && ship.State == Ship.States.HEALING)
            {
                //IsThereCloserEnemy();
                yield return StartCoroutine(GettingIntoPosition());
                //print("1.");
                if (IsSomethingWrong()) yield break;
                
                Vector2 healingVector = transform.position - injuredShip.transform.position;
                ship.rotation_Controller.Rotate(-healingVector);
                while (ship.rotation_Controller.rotatingCoroutine != null) yield return null; //ждем пока повернется, иначе иногда стреляет боком.
                float distanceSqr = healingVector.sqrMagnitude;
                //print("distanceSqr = " + distanceSqr + "    attackDistance = " + attackDistanceSqr);
                if (distanceSqr > healingDistanceSqr)
                {
                    ship.moveDirection = (-healingVector).normalized;
                    yield return new WaitForSeconds(0.5f);
                    //print("2.");

                    if (IsSomethingWrong()) yield break;

                    healingCoroutine = StartCoroutine(Healing());
                    yield break;


                }//сравниваем квадрат расстояния между краями коллайдеров микротов и квадрат дистанции атаки
                 //квадраты, а не оригиналы, чтобы быстрее считал, т.к. корень задерживает рассчет
                 //print("3.");

                if (IsSomethingWrong()) yield break;

                //print("GOT HIM!!!");
                if (ship.State == Ship.States.HEALING)
                {
                    //ship.DeselerateSmoothly();
                    ship.moveDirection = Vector2.zero;
                    ship.InvokeMoveDirectionZeroEvent();
                }

                //если готов бить, то бьет и ждем время, зависящее от скорости атаки
                if (ship.isReadyToHit)
                {
                    yield return StartCoroutine(HealFrend());
                }
                //если не готов бить, то ждем пока не будет готов, затем бьет
                else
                {
                    while (!ship.isReadyToHit && ship.State == Ship.States.HEALING)
                        yield return null;

                    if (IsSomethingWrong()) yield break;

                    //HitEnemy();
                    yield return StartCoroutine(HealFrend());

                }

                yield return new WaitForSeconds(ship.reload_time);
            }

            injuredShip = null;
            healingCoroutine = null;

            //if (ship.State != Ship.States.STUNNED)
            {
                
                //ship.isDirectSightEnemy = false;
                if (ship.State == Ship.States.HEALING)
                    ship.Idle();
            }
        }

        /// <summary>
        /// Проверяет свою позицию по отношению к нуждающемуся в лечении кораблю и обходит препятствия, если есть необходимость.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GettingIntoPosition()
        {
            while (injuredShip != null && !Global_Controller.IsDirectSightShip(ship, injuredShip/*, out _*/))
            {
                yield return ship.avoiding_Obstacle.avoidShipCoroutine = ship.avoiding_Obstacle.StartCoroutine(ship.avoiding_Obstacle.AvoidObstacle(injuredShip.gameObject));
                //IsThereCloserEnemy();
            }
        }

        /// <summary>
        /// Проверяет состояние корабля и переменных после какого-либо действия, чтобы не было ошибок.
        /// </summary>
        private bool IsSomethingWrong()
        {
            if (ship.State != Ship.States.HEALING)
            {
                //print("ship: " + ship.gameObject.name + ". Ship state != FIGHT");
                healingCoroutine = null;
                return true;
            }

            if (injuredShip == null || !IsFrendShipInjuredOrDebuff(injuredShip))
            {
                //print("ship: " + ship.gameObject.name + ". enemy == null");
                healingCoroutine = null;
                if (ship.State == Ship.States.HEALING)
                    ship.Idle();
                return true;
            }

            if (!Global_Controller.MustSeeShip(ship, injuredShip))
            {
                //print("ship: " + ship.gameObject.name + ". NON must see enemy");
                injuredShip = null;
                healingCoroutine = null;
                if (ship.State == Ship.States.HEALING)
                    ship.Idle();
                return true;
            }

            return false;
        }






        private IEnumerator HealFrend()
        {
            while(ship.rotation_Controller.rotatingCoroutine != null)
            {
                yield return null;
            }

            if (injuredShip == null || !IsFrendShipInjuredOrDebuff(injuredShip))
                yield break;

            ship.Hit();//удар произведен

            ProjectileСontainer projectileСontainer = Instantiate(Prefabs.Instance.projectileContainer);
            HealProjectile _projectile = Instantiate(player_Data.healProjectile);

            _projectile.healPoints = ModuleData[levelOfModule];
            if (UltimateImpactAction())
            {
                _projectile.healPoints *= 2;
                _projectile.AttackUltimateVisionEffect();
            }

            _projectile.chanceHealDebuff = DebuffHealChance[levelOfModule];

            Vector2 heading = injuredShip.transform.position - transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            var projectilePosition = PositionOfProjectile(distance);
            while (float.IsNaN(projectilePosition.x))
            {
                print("X coordinate of projectile is NaN! ship is " + gameObject.name + "    transform = " + gameObject.transform);
                yield return null;

                if (ship.enemyCurrent == null)
                    yield break;
                heading = ship.enemyCurrent.transform.position - transform.position;
                distance = heading.magnitude;
                projectilePosition = PositionOfProjectile(distance);
            }

            projectileСontainer.transform.position = projectilePosition;
            projectileСontainer.direction = _projectile.direction = direction;
            projectileСontainer.AddProjectile(_projectile);

            onProjectileCreated?.Invoke(projectileСontainer, this);
            //projectileСontainer.GoTo(direction);



            Vector2 PositionOfProjectile(float _distance)
            {
                float distanceFromOrigin = ship.radiusSize + distanceFrontFromShipToCreateProjectile;//m1
                float distanceToDestination = _distance - distanceFromOrigin;//m2
                float x1 = transform.position.x;
                float x2 = injuredShip.transform.position.x;
                float y1 = transform.position.y;
                float y2 = injuredShip.transform.position.y;
                float X = (distanceToDestination * x1 + distanceFromOrigin * x2) / (distanceFromOrigin + distanceToDestination);
                float Y = (distanceToDestination * y1 + distanceFromOrigin * y2) / (distanceFromOrigin + distanceToDestination);

                return new Vector2(X, Y);
            }
        }

    }
}