using UnityEngine;
using System.Collections;

namespace MODULES
{
    public class DeathEating_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.DEATHEATING_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            EventManager.Instance.AddListener(EVENT_TYPE.SHIP_DESTROYED, DeathEating);
        }

        private new static readonly float[] ModuleData =
        {
        0,
        30f,    30.3f,  30.6f,  30.91f, 31.22f, 31.53f, 31.85f, 32.16f, 32.49f, 32.81f,
        33.14f, 33.47f, 33.8f,  34.14f, 34.48f, 34.83f, 35.18f, 35.53f, 35.88f, 36.24f,
        36.61f, 36.97f, 37.34f, 37.71f, 38.09f, 38.47f, 38.86f, 39.25f, 39.64f, 40.04f,
        40.44f, 40.84f, 41.25f, 41.66f, 42.08f, 42.5f,  42.92f, 43.35f, 43.79f, 44.22f,
        44.67f, 45.11f, 45.56f, 46.02f, 46.48f, 46.94f, 47.41f, 47.89f, 48.37f, 48.85f,
        49.34f, 49.83f, 50.33f, 50.83f, 51.34f, 51.86f, 52.37f, 52.9f,  53.43f, 53.96f,
        54.5f,  55.05f, 55.6f,  56.15f, 56.71f, 57.28f, 57.85f, 58.43f, 59.02f, 59.61f,
        60.2f,  60.8f,  61.41f, 62.03f, 62.65f, 63.27f, 63.91f, 64.55f, 65.19f, 65.84f,
        66.5f,  67.17f, 67.84f, 68.52f, 69.2f,  69.89f, 70.59f, 71.3f,  72.01f, 72.73f,
        73.46f, 74.19f, 74.94f, 75.68f, 76.44f, 77.21f, 77.98f, 78.76f, 79.55f, 80.34f,
        82.75f, 85.23f, 87.79f, 90.42f, 93.14f, 95.93f, 98.81f, 101.77f, 104.83f, 107.97f,

        };


        private static readonly float[] Radius =
        {
            0,
            0.8f,   0.81f,  0.82f,  0.82f,  0.83f,  0.84f,  0.85f,  0.86f,  0.87f,  0.87f,
            0.88f,  0.89f,  0.9f,   0.91f,  0.92f,  0.93f,  0.94f,  0.95f,  0.96f,  0.97f,
            0.98f,  0.99f,  1f,     1.01f,  1.02f,  1.03f,  1.04f,  1.05f,  1.06f,  1.07f,
            1.08f,  1.09f,  1.1f,   1.11f,  1.12f,  1.13f,  1.14f,  1.16f,  1.17f,  1.18f,
            1.19f,  1.2f,   1.22f,  1.23f,  1.24f,  1.25f,  1.26f,  1.28f,  1.29f,  1.3f,
            1.32f,  1.33f,  1.34f,  1.36f,  1.37f,  1.38f,  1.4f,   1.41f,  1.42f,  1.44f,
            1.45f,  1.47f,  1.48f,  1.5f,   1.51f,  1.53f,  1.54f,  1.56f,  1.57f,  1.59f,
            1.61f,  1.62f,  1.64f,  1.65f,  1.67f,  1.69f,  1.7f,   1.72f,  1.74f,  1.76f,
            1.77f,  1.79f,  1.81f,  1.83f,  1.85f,  1.86f,  1.88f,  1.9f,   1.92f,  1.94f,
            1.96f,  1.98f,  2f,     2.02f,  2.04f,  2.06f,  2.08f,  2.1f,   2.12f,  2.14f,
            2.21f,  2.27f,  2.34f,  2.41f,  2.48f,  2.56f,  2.63f,  2.71f,  2.8f,   2.88f,

        };

        /// <summary>
        /// Прибавка к максимуму ХП при сработке ультимейта в процентах от maxHealth погибшего корабля.
        /// </summary>
        private static readonly float[] addingToMaxHPpercentage =
        {
            0f,
            2f, 2.02f,  2.04f,  2.06f,  2.08f,  2.1f,   2.12f,  2.14f,  2.17f,  2.19f,
            2.21f,  2.23f,  2.25f,  2.28f,  2.3f,   2.32f,  2.35f,  2.37f,  2.39f,  2.42f,
            2.44f,  2.46f,  2.49f,  2.51f,  2.54f,  2.56f,  2.59f,  2.62f,  2.64f,  2.67f,
            2.7f,   2.72f,  2.75f,  2.78f,  2.81f,  2.83f,  2.86f,  2.89f,  2.92f,  2.95f,
            2.98f,  3.01f,  3.04f,  3.07f,  3.1f,   3.13f,  3.16f,  3.19f,  3.22f,  3.26f,
            3.29f,  3.32f,  3.36f,  3.39f,  3.42f,  3.46f,  3.49f,  3.53f,  3.56f,  3.6f,
            3.63f,  3.67f,  3.71f,  3.74f,  3.78f,  3.82f,  3.86f,  3.9f,   3.93f,  3.97f,
            4.01f,  4.05f,  4.09f,  4.14f,  4.18f,  4.22f,  4.26f,  4.3f,   4.35f,  4.39f,
            4.43f,  4.48f,  4.52f,  4.57f,  4.61f,  4.66f,  4.71f,  4.75f,  4.8f,   4.85f,
            4.9f,   4.95f,  5f,     5.05f,  5.1f,   5.15f,  5.2f,   5.25f,  5.3f,   5.36f,
            5.52f,  5.68f,  5.85f,  6.03f,  6.21f,  6.4f,   6.59f,  6.78f,  6.99f,  7.2f,
        };

        /// <summary>
        /// Максимальное значение макс ХП, выше которого не будет увеличиваться макс ХП.
        /// </summary>
        private static readonly int[] maxHPmaxValue =
        {
            0,
            300,    303,    306,    309,    312,    315,    318,    322,    325,    328,
            331,    335,    338,    341,    345,    348,    352,    355,    359,    362,
            366,    370,    373,    377,    381,    385,    389,    392,    396,    400,
            404,    408,    412,    417,    421,    425,    429,    434,    438,    442,
            447,    451,    456,    460,    465,    469,    474,    479,    484,    489,
            493,    498,    503,    508,    513,    519,    524,    529,    534,    540,
            545,    550,    556,    562,    567,    573,    579,    584,    590,    596,
            602,    608,    614,    620,    626,    633,    639,    645,    652,    658,
            665,    672,    678,    685,    692,    699,    706,    713,    720,    727,
            735,    742,    749,    757,    764,    772,    780,    788,    795,    803,
            828,    852,    878,    904,    931,    959,    988,    1018,   1048,   1080,
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


        private void DeathEating(EVENT_TYPE eVENT_TYPE, Component deadShip, object shipWhoKillHim)
        {
            Ship _deadShip = (Ship)deadShip;
            float radius = Radius[levelOfModule];

            if((_deadShip.transform.position - ship.transform.position).sqrMagnitude < radius * radius)
            {
                bool ultimate = false;
                int addingMaxHP = 0;
                //Этот модуль имеет постоянный ультимейт. Не от процента. Ну или 100% вероятности сработки.
                //К максимуму ХП прибавляет 10% от макс ХП уничтоженного корабля.
                if (LevelOfModule >= References.Instance.settings.moduleUltimateLevel)
                {
                    ultimate = true;
                    addingMaxHP = (int)(_deadShip.healthMax * addingToMaxHPpercentage[levelOfModule] / 100);
                }

                int healAmount = (int)(_deadShip.healthMax * ModuleData[LevelOfModule] / 100);
                var healing_orb = Instantiate(Prefabs.Instance.healing_Orb);
                healing_orb.Initialize(healAmount, ship, _deadShip.transform.position, ultimate, addingMaxHP, maxHPmaxValue[levelOfModule]);
            }

        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Radius[levelOfModule]);
        }

    }
}