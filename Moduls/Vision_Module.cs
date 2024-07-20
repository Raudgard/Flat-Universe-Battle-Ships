using Tools;
using System.Linq;
using UnityEngine;

namespace MODULES
{
    public class Vision_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.VISION_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.VisionRadius = ModuleData[LevelOfModule];

            if (levelOfModule > References.Instance.settings.moduleUltimateLevel)
            {
                UnityTools.ExecuteWithDelay(CheckEnemyShipsOnStartBattle, 2);
                EventManager.Instance.AddListener(EVENT_TYPE.SHIP_CREATED, CheckCreatedShip);
            }

        }

        private new static readonly float[] ModuleData =
        {
            5f,
            5.03f,  5.05f,  5.08f,  5.1f,   5.13f,  5.15f,  5.18f,  5.2f,   5.23f,  5.26f,
            5.28f,  5.31f,  5.33f,  5.36f,  5.39f,  5.42f,  5.44f,  5.47f,  5.5f,   5.52f,
            5.55f,  5.58f,  5.61f,  5.64f,  5.66f,  5.69f,  5.72f,  5.75f,  5.78f,  5.81f,
            5.84f,  5.87f,  5.89f,  5.92f,  5.95f,  5.98f,  6.01f,  6.04f,  6.07f,  6.1f,
            6.13f,  6.17f,  6.2f,   6.23f,  6.26f,  6.29f,  6.32f,  6.35f,  6.38f,  6.42f,
            6.45f,  6.48f,  6.51f,  6.55f,  6.58f,  6.61f,  6.64f,  6.68f,  6.71f,  6.74f,
            6.78f,  6.81f,  6.85f,  6.88f,  6.91f,  6.95f,  6.98f,  7.02f,  7.05f,  7.09f,
            7.12f,  7.16f,  7.2f,   7.23f,  7.27f,  7.3f,   7.34f,  7.38f,  7.41f,  7.45f,
            7.49f,  7.53f,  7.56f,  7.6f,   7.64f,  7.68f,  7.72f,  7.76f,  7.79f,  7.83f,
            7.87f,  7.91f,  7.95f,  7.99f,  8.03f,  8.07f,  8.11f,  8.15f,  8.19f,  8.23f,
            8.36f,  8.48f,  8.61f,  8.74f,  8.87f,  9f, 9.14f,  9.27f,  9.41f,  9.56f,

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


        private void CheckEnemyShipsOnStartBattle()
        {
            var enemyShips = Global_Controller.Instance.allShips.Where(s => s.team != ship.team);
            foreach (var enemyShip in enemyShips)
            {
                if(enemyShip.TryGetComponent(out Stealth_Module stealth_Module))
                {
                    //Debug.Log("В корабле есть Stealth Module. Подписываюсь на исчезновение.");
                    stealth_Module.BecameInvisible += EnemyShipBecameInvisible;
                }
            }
        }

        private void CheckCreatedShip(EVENT_TYPE event_type, Component Sender, object Param = null)
        {
            var shipNew = (Ship)Sender;
            if(shipNew.team != ship.team && shipNew.TryGetComponent(out Stealth_Module stealth_Module))
            {
                //Debug.Log("В новом корабле есть Stealth Module. Подписываюсь на исчезновение.");
                stealth_Module.BecameInvisible += EnemyShipBecameInvisible;
            }
        }

        /// <summary>
        /// Если вражеский корабль в зоне видимости исчезает, то он тут же становится видимым. То есть не дает ему исчезнуть пока в зоне видимости.
        /// </summary>
        /// <param name="enemyShip"></param>
        /// <param name="stealth_Module"></param>
        private void EnemyShipBecameInvisible(Ship enemyShip, Stealth_Module stealth_Module)
        {
            if (ship != null && enemyShip != null && !stealth_Module.IsAlreadyHighlighting && Global_Controller.DoISeeThisShip(ship, enemyShip))
            {
                stealth_Module.IsAlreadyHighlighting = true;
                ship.shipVisualController.VisionUltimateEffect(enemyShip);
                stealth_Module.OnDetected();

                UnityTools.ExecuteWithDelay(delegate { if (stealth_Module != null) stealth_Module.IsAlreadyHighlighting = false; }, 1);
            }

        }


    }
}