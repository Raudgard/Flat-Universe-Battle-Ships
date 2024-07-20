using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Telekinesis_Module : Module
    {
        private SearchingForUSP searchingForUSP;
        private Coroutine telekines;
        private Coroutine telekinesUltimate;
        public float USPSpeed = 5;
        private USP uspUltimate;

        private void Awake()
        {
            moduleType = Moduls.TELEKINESIS_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            //EventManager.Instance.AddListener(EVENT_TYPE.STATUS_CHANGED, USPAttraction);
            ship.StateChanged += CheckState;
            searchingForUSP = ship.searchingForUSP;

            if(BattleSceneController.Instance != null)
                BattleSceneController.Instance.isNeedTrailForUSP = true;
        }

        private new static readonly float[] ModuleData =
        {
        0,
        1.5f,   1.52f,  1.53f,  1.55f,  1.56f,  1.58f,  1.59f,  1.61f,  1.62f,  1.64f,
        1.66f,  1.67f,  1.69f,  1.71f,  1.72f,  1.74f,  1.76f,  1.78f,  1.79f,  1.81f,
        1.83f,  1.85f,  1.87f,  1.89f,  1.9f,   1.92f,  1.94f,  1.96f,  1.98f,  2f,
        2.02f,  2.04f,  2.06f,  2.08f,  2.1f,   2.12f,  2.15f,  2.17f,  2.19f,  2.21f,
        2.23f,  2.26f,  2.28f,  2.3f,   2.32f,  2.35f,  2.37f,  2.39f,  2.42f,  2.44f,
        2.47f,  2.49f,  2.52f,  2.54f,  2.57f,  2.59f,  2.62f,  2.64f,  2.67f,  2.7f,
        2.73f,  2.75f,  2.78f,  2.81f,  2.84f,  2.86f,  2.89f,  2.92f,  2.95f,  2.98f,
        3.01f,  3.04f,  3.07f,  3.1f,   3.13f,  3.16f,  3.2f,   3.23f,  3.26f,  3.29f,
        3.33f,  3.36f,  3.39f,  3.43f,  3.46f,  3.49f,  3.53f,  3.56f,  3.6f,   3.64f,
        3.67f,  3.71f,  3.75f,  3.78f,  3.82f,  3.86f,  3.9f,   3.94f,  3.98f,  4.02f,
        4.14f,  4.26f,  4.39f,  4.52f,  4.66f,  4.8f,   4.94f,  5.09f,  5.24f,  5.4f
    };  //расстояние до еды, с которого начинает действовать притягивание

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

        private void CheckState()
        {
            if (ship.State == Ship.States.TO_USP)
            {
                if (telekines == null)
                    telekines = StartCoroutine(Telekines());

                if (UltimateImpactAction() && telekinesUltimate == null && searchingForUSP.secondDistanceUSP != null)
                {
                    //Debug.Log($"Telekinesis Ultimate! ship: {ship.name}");
                    uspUltimate = searchingForUSP.secondDistanceUSP;
                    telekinesUltimate = StartCoroutine(TelekinesUltimate());
                }
            }
            else if (telekines != null)// если статус изменился во время притягивания еды, то еда перестает притягиваться и ее переменная возвращается в false, чтоб могли притягивать другие микроты
            {
                StopCoroutine(telekines);
                telekines = null;

                if (ship.USPCurrent != null)
                    ship.USPCurrent.isMoving = false;
            }
            else if (telekinesUltimate != null)// если статус изменился во время притягивания еды, то еда перестает притягиваться и ее переменная возвращается в false, чтоб могли притягивать другие микроты
            {
                StopCoroutine(telekinesUltimate);
                telekinesUltimate = null;

                if (uspUltimate != null)
                    uspUltimate.isMoving = false;
            }

        }

        private IEnumerator Telekines()
        {
            while (ship.State == Ship.States.TO_USP && ship.USPCurrent != null)
            {
                Vector3 toShip = ship.transform.position - ship.USPCurrent.transform.position;
                float distanceSqr = toShip.sqrMagnitude;

                if (distanceSqr < (ModuleData[LevelOfModule] - ship.radiusSize) * (ModuleData[LevelOfModule] - ship.radiusSize) && ship.USPCurrent.isMoving == false) //сравниваем с квадратом разности установленного расстояния и радиуса микрота (чтоб этот радиус не учитывался)
                {
                    

                    ship.USPCurrent.isMoving = true;
                    while (ship.USPCurrent != null)
                    {
                        toShip = ship.transform.position - ship.USPCurrent.transform.position;
                        ship.USPCurrent.transform.Translate(toShip * Time.fixedDeltaTime * USPSpeed);

                        yield return new WaitForFixedUpdate();
                    }
                }

                yield return null;
            }

            telekines = null;
        }


        private IEnumerator TelekinesUltimate()
        {
            while (ship.State == Ship.States.TO_USP && uspUltimate != null)
            {
                Vector3 toShip = ship.transform.position - uspUltimate.transform.position;
                float distanceSqr = toShip.sqrMagnitude;

                if (distanceSqr < (ModuleData[LevelOfModule] - ship.radiusSize) * (ModuleData[LevelOfModule] - ship.radiusSize) && uspUltimate.isMoving == false) //сравниваем с квадратом разности установленного расстояния и радиуса микрота (чтоб этот радиус не учитывался)
                {
                    

                    uspUltimate.isMoving = true;
                    while (uspUltimate != null)
                    {
                        toShip = ship.transform.position - uspUltimate.transform.position;
                        uspUltimate.transform.Translate(toShip * Time.fixedDeltaTime * USPSpeed);

                        yield return new WaitForFixedUpdate();
                    }
                }

                yield return null;
            }

            telekinesUltimate = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, ModuleData[levelOfModule]);
        }

    }
}