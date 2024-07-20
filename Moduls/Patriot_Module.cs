using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MODULES
{
    public class Patriot_Module : Module
    {
        [SerializeField] private int originalDamage;
        //[SerializeField] private List<int> buffs = new List<int>();

        private void Awake()
        {
            moduleType = Moduls.PATRIOT_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(GetOriginalDamageAndAddListener());

            //if (buffs.Count > 0)
            //{
            //    buffs.Clear();
            //}
        }

        //бафф к атаке в %
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


        /// <summary>
        /// Время действия баффа.
        /// </summary>
        private static readonly float[] BuffTimeData =
        {
        0,
        20f,    20.2f,  20.4f,  20.61f, 20.81f, 21.02f, 21.23f, 21.44f, 21.66f, 21.87f,
        22.09f, 22.31f, 22.54f, 22.76f, 22.99f, 23.22f, 23.45f, 23.69f, 23.92f, 24.16f,
        24.4f,  24.65f, 24.89f, 25.14f, 25.39f, 25.65f, 25.91f, 26.16f, 26.43f, 26.69f,
        26.96f, 27.23f, 27.5f,  27.77f, 28.05f, 28.33f, 28.62f, 28.9f,  29.19f, 29.48f,
        29.78f, 30.08f, 30.38f, 30.68f, 30.99f, 31.3f,  31.61f, 31.93f, 32.24f, 32.57f,
        32.89f, 33.22f, 33.55f, 33.89f, 34.23f, 34.57f, 34.92f, 35.27f, 35.62f, 35.97f,
        36.33f, 36.7f,  37.06f, 37.43f, 37.81f, 38.19f, 38.57f, 38.95f, 39.34f, 39.74f,
        40.14f, 40.54f, 40.94f, 41.35f, 41.76f, 42.18f, 42.6f,  43.03f, 43.46f, 43.9f,
        44.33f, 44.78f, 45.23f, 45.68f, 46.13f, 46.6f,  47.06f, 47.53f, 48.01f, 48.49f,
        48.97f, 49.46f, 49.96f, 50.46f, 50.96f, 51.47f, 51.99f, 52.51f, 53.03f, 53.56f,
        55.17f, 56.82f, 58.53f, 60.28f, 62.09f, 63.95f, 65.87f, 67.85f, 69.88f, 71.98f,

        };



        private void Buff(EVENT_TYPE eVENT_TYPE, Component component, object shipWhoKillHim)
        {
            Ship destroyedShip = component.GetComponent<Ship>();
            if (destroyedShip.team != ship.team)
            {
                //print("погиб не свой!");
                return;
            }

            Vector3 firstShipPosition = ship.transform.position;
            Vector3 secondShipPosition = component.transform.position;

            float distanceSqr = (firstShipPosition - secondShipPosition).sqrMagnitude;

            if (distanceSqr < (ship.VisionRadius + destroyedShip.radiusSize) * (ship.VisionRadius + destroyedShip.radiusSize))
            {
                //print("Атака увеличена!! имя = " + ship.name);
                int buffValue = (int)(originalDamage * ModuleData[LevelOfModule] / 100);
                if(UltimateImpactAction())
                {
                    buffValue *= 2;
                    ship.shipVisualController.PatriotModuleUltimateEffect();
                }
                else
                {
                    ship.shipVisualController.PatriotModuleEffect();
                }
                ship.attack_damage += buffValue;
                //buffs.Add(buffValue);

                if (gameObject.activeSelf)
                    StartCoroutine(BuffDuration(buffValue));
            }
        }

        private IEnumerator BuffDuration(int buffValue)
        {
            yield return new WaitForSeconds(BuffTimeData[LevelOfModule]);

            if (gameObject != null)
            {
                //print("вермя баффа истекло у микрота " + ship.name);
                //if(buffs.Contains(buffValue))
                //{
                //    buffs.Remove(buffValue);
                //}

                ship.attack_damage -= buffValue;
            }
        }


        //ждем 1 кадр и находим оригинальный дамаг и подписываемся на событие уничтожение микрота
        private IEnumerator GetOriginalDamageAndAddListener()
        {
            if (/*!gameObject.activeSelf || */SceneManager.GetActiveScene() != SceneManager.GetSceneByName("ScirmishScene"))
                yield break;
            
            yield return null;
            
            //print("addListener " + ship.name);
            EventManager.Instance.AddListener(EVENT_TYPE.SHIP_DESTROYED, Buff);
            if(ship.IsOriginal)
                originalDamage = ship.attack_damage;
        }
    }
}