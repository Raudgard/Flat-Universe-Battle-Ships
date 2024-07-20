using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Regeneration_Module : Module
    {
        public float regenerationRate = 4.0f; //один раз во сколько секунд срабатывает регенерация
        private int regenerationValue;
        private Coroutine regenerationCoroutine = null;
        //private Player_Data player_Data;
        private Global_Controller global_Controller;
        //private Color healingColor = new Color(0.2f, 1, 0);
        //private VisualEffectsController visualEffectsController;
        private Colors colors;
        private WaitForSeconds delay;
        private void Awake()
        {
            moduleType = Moduls.REGENERATION_MODULE;
            global_Controller = Global_Controller.Instance;
            //visualEffectsController = GetComponent<VisualEffectsController>();
            colors = References.Instance.colors;
            delay = new WaitForSeconds(regenerationRate);

        }

        protected override void Start()
        {
            base.Start();
            regenerationValue = Mathf.RoundToInt(ModuleData[levelOfModule]);
            ship.takeHitComponent.DamageTaked += Regeneration;
        }

        private new static readonly float[] ModuleData =
        {
            0,
            5f, 5.05f,  5.1f,   5.15f,  5.2f,   5.26f,  5.31f,  5.36f,  5.41f,  5.47f,
            5.52f,  5.58f,  5.63f,  5.69f,  5.75f,  5.8f,   5.86f,  5.92f,  5.98f,  6.04f,
            6.1f,   6.16f,  6.22f,  6.29f,  6.35f,  6.41f,  6.48f,  6.54f,  6.61f,  6.67f,
            6.74f,  6.81f,  6.87f,  6.94f,  7.01f,  7.08f,  7.15f,  7.23f,  7.3f,   7.37f,
            7.44f,  7.52f,  7.59f,  7.67f,  7.75f,  7.82f,  7.9f,   7.98f,  8.06f,  8.14f,
            8.22f,  8.31f,  8.39f,  8.47f,  8.56f,  8.64f,  8.73f,  8.82f,  8.9f,   8.99f,
            9.08f,  9.17f,  9.27f,  9.36f,  9.45f,  9.55f,  9.64f,  9.74f,  9.84f,  9.93f,
            10.03f, 10.13f, 10.24f, 10.34f, 10.44f, 10.55f, 10.65f, 10.76f, 10.87f, 10.97f,
            11.08f, 11.19f, 11.31f, 11.42f, 11.53f, 11.65f, 11.77f, 11.88f, 12f,    12.12f,
            12.24f, 12.37f, 12.49f, 12.61f, 12.74f, 12.87f, 13f,    13.13f, 13.26f, 13.39f,
            13.79f, 14.21f, 14.63f, 15.07f, 15.52f, 15.99f, 16.47f, 16.96f, 17.47f, 18f,
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


        public void Regeneration()
        {
            if (regenerationCoroutine == null)
                regenerationCoroutine = StartCoroutine(RegenerationGo());
        }


        private IEnumerator RegenerationGo()
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

            while (ship.HealthCurrent < ship.healthMax)
            {
                int _regenerationValue = UltimateImpactAction() ? regenerationValue * 2 : regenerationValue;
                ship.HealthCurrent += regenerationValue;
                global_Controller.StartCoroutine(global_Controller.VisualizationOfDamage(_regenerationValue, ship.healthMax, ship.transform.position, new Vector2(0, 0.5f), colors.healing_color, 0.0f));
                ship.shipVisualController.Heal();

                yield return delay;
            }

            regenerationCoroutine = null;
        }
    }
}