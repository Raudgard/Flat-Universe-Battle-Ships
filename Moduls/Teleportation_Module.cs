using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using NaughtyAttributes;

namespace MODULES
{
    public class Teleportation_Module : Module
    {
        private Attack_Module Attack_Module { get; set; } = null;

        private System.Action timerEndAction { get; set; }

        [SerializeField] private bool militaryMode;

        public bool MilitaryMode
        {
            get => militaryMode;
            set
            {
                if (value)
                {
                    timerEndAction = AddOneCharge;

                    //ship.AddActionToShotDetectedEvent(ShotDetectedHandler, true);
                    ship.ShotDetected += ShotDetectedHandler;
                    Debug.Log("Teleportation Module set to Military Mode.");
                }
                else
                {
                    timerEndAction = Teleportation;
                    ship.ShotDetected -= ShotDetectedHandler;

                    Debug.Log("Teleportation Module set to Normal Mode.");
                }
                militaryMode = value;
            }
        }

        [SerializeField] private int teleportationCharges = 0;
        private int TeleportationCharges { get => teleportationCharges; set => teleportationCharges = value; }

        /// <summary>
        /// В процессе телепортации.
        /// </summary>
        private bool IsTeleportating { get; set; } = false;

        public float shift = 0.05f;


        private void Awake()
        {
            moduleType = Moduls.TELEPORTATION_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            Attack_Module = ship.Modules.Where(module => module is Attack_Module).SingleOrDefault() as Attack_Module;


            MilitaryMode = levelOfModule >= References.Instance.settings.moduleUltimateLevel;
            TeleportationCharges = 0;

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ScirmishScene"))
                StartCoroutine(Timer());

        }

        /// <summary>
        /// время до следующей телепортации
        /// </summary>
        private new static readonly float[] ModuleData =
        {
        0,
        15f,    14.85f, 14.7f,  14.55f, 14.41f, 14.26f, 14.12f, 13.98f, 13.84f, 13.7f,
13.57f, 13.43f, 13.3f,  13.16f, 13.03f, 12.9f,  12.77f, 12.64f, 12.52f, 12.39f,
12.27f, 12.15f, 12.02f, 11.9f,  11.79f, 11.67f, 11.55f, 11.44f, 11.32f, 11.21f,
11.1f,  10.98f, 10.87f, 10.77f, 10.66f, 10.55f, 10.45f, 10.34f, 10.24f, 10.14f,
10.03f, 9.93f,  9.83f,  9.74f,  9.64f,  9.54f,  9.45f,  9.35f,  9.26f,  9.17f,
9.08f,  8.98f,  8.89f,  8.81f,  8.72f,  8.63f,  8.54f,  8.46f,  8.37f,  8.29f,
8.21f,  8.13f,  8.04f,  7.96f,  7.88f,  7.81f,  7.73f,  7.65f,  7.57f,  7.5f,
7.42f,  7.35f,  7.27f,  7.2f,   7.13f,  7.06f,  6.99f,  6.92f,  6.85f,  6.78f,
6.71f,  6.65f,  6.58f,  6.51f,  6.45f,  6.38f,  6.32f,  6.26f,  6.19f,  6.13f,
6.07f,  6.01f,  5.95f,  5.89f,  5.83f,  5.77f,  5.72f,  5.66f,  5.6f,   5.55f,
5.38f,  5.22f,  5.06f,  4.91f,  4.76f,  4.62f,  4.48f,  4.35f,  4.22f,  4.09f,




        };  

        /// <summary>
        /// Расстояние телепортации
        /// </summary>
        private static readonly float[] DistanceOfTeleportation =
        {
        0,
        0.35f,  0.354f, 0.357f, 0.361f, 0.364f, 0.368f, 0.372f, 0.375f, 0.379f, 0.383f,
        0.387f, 0.39f,  0.394f, 0.398f, 0.402f, 0.406f, 0.41f,  0.415f, 0.419f, 0.423f,
        0.427f, 0.431f, 0.436f, 0.44f,  0.444f, 0.449f, 0.453f, 0.458f, 0.462f, 0.467f,
        0.472f, 0.476f, 0.481f, 0.486f, 0.491f, 0.496f, 0.501f, 0.506f, 0.511f, 0.516f,
        0.521f, 0.526f, 0.532f, 0.537f, 0.542f, 0.548f, 0.553f, 0.559f, 0.564f, 0.57f,
        0.576f, 0.581f, 0.587f, 0.593f, 0.599f, 0.605f, 0.611f, 0.617f, 0.623f, 0.63f,
        0.636f, 0.642f, 0.649f, 0.655f, 0.662f, 0.668f, 0.675f, 0.682f, 0.689f, 0.695f,
        0.702f, 0.709f, 0.716f, 0.724f, 0.731f, 0.738f, 0.746f, 0.753f, 0.761f, 0.768f,
        0.776f, 0.784f, 0.791f, 0.799f, 0.807f, 0.815f, 0.824f, 0.832f, 0.84f,  0.849f,
        0.857f, 0.866f, 0.874f, 0.883f, 0.892f, 0.901f, 0.91f,  0.919f, 0.928f, 0.937f,
        0.965f, 0.994f, 1.024f, 1.055f, 1.087f, 1.119f, 1.153f, 1.187f, 1.223f, 1.26f,

        }; 

        /// <summary>
        /// Скорость телепортации (фактически анимации перед перемещением)
        /// </summary>
        private static readonly float[] TeleportationSpeed =
        {
        0,
        1f, 1.015f, 1.03f,  1.046f, 1.061f, 1.077f, 1.093f, 1.11f,  1.126f, 1.143f,
        1.161f, 1.178f, 1.196f, 1.214f, 1.232f, 1.25f,  1.269f, 1.288f, 1.307f, 1.327f,
        1.347f, 1.367f, 1.388f, 1.408f, 1.43f,  1.451f, 1.473f, 1.495f, 1.517f, 1.54f,
        1.563f, 1.587f, 1.61f,  1.634f, 1.659f, 1.684f, 1.709f, 1.735f, 1.761f, 1.787f,
        1.814f, 1.841f, 1.869f, 1.897f, 1.925f, 1.954f, 1.984f, 2.013f, 2.043f, 2.074f,
        2.105f, 2.137f, 2.169f, 2.201f, 2.234f, 2.268f, 2.302f, 2.336f, 2.372f, 2.407f,
        2.443f, 2.48f,  2.517f, 2.555f, 2.593f, 2.632f, 2.672f, 2.712f, 2.752f, 2.794f,
        2.835f, 2.878f, 2.921f, 2.965f, 3.009f, 3.055f, 3.1f,   3.147f, 3.194f, 3.242f,
        3.291f, 3.34f,  3.39f,  3.441f, 3.493f, 3.545f, 3.598f, 3.652f, 3.707f, 3.763f,
        3.819f, 3.876f, 3.934f, 3.993f, 4.053f, 4.114f, 4.176f, 4.238f, 4.302f, 4.367f,
        4.563f, 4.768f, 4.983f, 5.207f, 5.442f, 5.686f, 5.942f, 6.21f,  6.489f, 6.781f,

        };

        /// <summary>
        /// Максимальное количество зарядов, которое может накопить корабль в боевом режиме.
        /// </summary>
        private static readonly int[] TeleportationMaxCharges =
        {
        0,
        2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
2,  2,  2,  2,  2,  3,  3,  3,  3,  3,
3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
3,  3,  3,  3,  3,  3,  3,  3,  4,  4,
4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
4,  4,  4,  4,  4,  5,  5,  5,  5,  5,
5,  5,  5,  5,  5,  5,  5,  5,  6,  6,
6,  6,  6,  6,  6,  6,  6,  6,  6,  6,
7,  7,  7,  7,  7,  7,  7,  7,  7,  8,
8,  8,  8,  8,  8,  8,  8,  8,  9,  9,
9,  10, 10, 10, 11, 11, 12, 12, 13, 14,

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


        private void ShotDetectedHandler()
        {
            if (TeleportationCharges > 0 && 
                !IsTeleportating &&
                ship.State != Ship.States.DIGESTION && 
                ship.State != Ship.States.REPRODUCTION &&
                ship.State != Ship.States.STUNNED &&
                ship.State != Ship.States.IDLE &&
                ship.State != Ship.States.FLEE)
            {
                TeleportationCharges--;
                TeleportationMilitary();
            }
        }


        public IEnumerator Timer()
        {
            yield return new WaitForSeconds(ModuleData[levelOfModule] * Random.Range(1 - shift, 1 + shift));

            //В состоянии размножения, заморозки и убегания не прибавляет заряд или не телепортируется.
            while (ship.State == Ship.States.REPRODUCTION || ship.State == Ship.States.STUNNED || ship.State == Ship.States.FLEE)
            {
                yield return null;
            }

            timerEndAction.Invoke();
        }


        private void Teleportation()
        {
            StartCoroutine(TeleportationNormal());
        }

        private void AddOneCharge()
        {
            if (TeleportationCharges < TeleportationMaxCharges[levelOfModule])
            {
                TeleportationCharges++;
                ship.shipVisualController.TeleportationCharge();
            }

            StartCoroutine(Timer());
        }


        private IEnumerator TeleportationNormal()
        {
                //в состояниях переваривания, размножения, оглушения, отлета от ударной волны не портается
                while (ship.State == Ship.States.DIGESTION || 
                ship.State == Ship.States.REPRODUCTION || 
                ship.State == Ship.States.STUNNED || 
                ship.State == Ship.States.IDLE ||
                ship.State == Ship.States.FLEE)
                {
                    yield return null;
                }

                IsTeleportating = true;
                ship.shipVisualController.Teleportation(this, TeleportationSpeed[levelOfModule], TeleportationEndAction);
                StartCoroutine(Timer());
        }


        private void TeleportationMilitary()
        {
            IsTeleportating = true;
            ship.shipVisualController.Teleportation(this, TeleportationSpeed[levelOfModule], TeleportationEndAction);
        }


        private void TeleportationEndAction() => IsTeleportating = false;




        public Vector2 GetNewPosition()
        {
            Vector2 __newPosition = (Vector2)ship.transform.position + ship.moveDirection * DistanceOfTeleportation[levelOfModule];

            //если цель ближе, чем дистанция телепортации, то телепортируемся прямо в цель
            if (ship.State == Ship.States.TO_USP && ship.USPCurrent != null)
            {
                Vector2 USPPosition = ship.USPCurrent.transform.position;
                float sqrDistanceToUSP = (USPPosition - (Vector2)ship.transform.position).sqrMagnitude;
                if (sqrDistanceToUSP < DistanceOfTeleportation[levelOfModule] * DistanceOfTeleportation[levelOfModule])
                {
                    __newPosition = USPPosition;
                }
            }
            else if (ship.State == Ship.States.FIGHT && ship.enemyCurrent != null)
            {
                #region Метод пересечения окружностей (более точный, но более сложный и емкий и с ошибками)
                //Для начала найдем расстояние между центрами окружностей.d = || P1 - P0 ||.Если d > r0 + r1, тогда решений нет: круги лежат отдельно.Аналогично в случае d< || r0 - r1 || -тогда нет решений, так как одна окружность находится внутри другой.
                //Рассмотрим два треугольника P0P2P3 и P1P2P3. Имеем
                //a2 + h2 = r02 and b2 +h2 = r12
                //Используя равенство d = a + b, мы можем разрешить относительно a:
                //                a = (r02 - r12 + d2) / (2 d)
                //В случае соприкосновения окружностей, это, очевидно, превратится в r0, так как: d = r0 + r1
                //Решим относительно h, подставив в первое уравнение h2 = r02 - a2
                //Итак,
                //P2 = P0 + a(P1 - P0) / d
                //Таким образом, получаем координаты точек P3 = (x3, y3):
                //x3 = x2 + -h(y1 - y0) / d
                //y3 = y2 - +h(x1 - x0) / d



                //Vector2 P0 = transform.position;
                //Vector2 P1 = ship.enemyCurrent.transform.position;
                //float d = Vector2.Distance(P1, P0);

                //float a, r0, r1, h;
                //Vector2 P2;
                //r0 = DistanceOfTeleportation[levelOfModule];
                //r1 = ship.radiusSize + ship.attack_range;

                //if (d >= r0 + r1)
                //{
                //    Debug.LogWarning($"Окружности не пересекаются. Телепортируемся по moveDirection. d: {d}, r0: {r0}, r1: {r1}");
                //    return __newPosition;
                //}
                //else
                //{
                //    r1 = d;
                //}

                //a = (r0 * r0 - r1 * r1 + d * d) / (2 * d);
                //h = Mathf.Sqrt(r0 * r0 - a * a);

                //P2 = P0 + a * (P1 - P0) / d;

                //int point = Random.Range(0, 2);
                //Debug.Log($"point: {point}");
                //float x3, y3;

                //if (point == 0)
                //{
                //    x3 = P2.x + h * (P1.y - P0.y) / d;
                //    y3 = P2.y - h * (P1.x - P0.x) / d;
                //    return new Vector2(x3, y3);
                //}
                //else if (point == 1)
                //{
                //    x3 = P2.x - h * (P1.y - P0.y) / d;
                //    y3 = P2.y + h * (P1.x - P0.x) / d;
                //    return new Vector2(x3, y3);
                //}
                //else
                //{
                //    Debug.Log("Error of point");
                //    throw new System.Exception();
                //}
                #endregion

                if(!Attack_Module.CanReachEnemy)
                {
                    //Debug.Log($"Расстояние до цели больше дистанции атаки! Телепорт вперед.");
                    return __newPosition;
                }

                int point = Random.Range(0, 2);
                Vector2 vectorFromShipToEnemy = (ship.enemyCurrent.transform.position - ship.transform.position).normalized;
                Vector2 perpendicularVector;
                if (point == 0)
                {
                    perpendicularVector = new Vector2(-vectorFromShipToEnemy.y, vectorFromShipToEnemy.x);
                }
                else
                {
                    perpendicularVector = new Vector2(vectorFromShipToEnemy.y, -vectorFromShipToEnemy.x);
                }

                __newPosition = (Vector2)ship.transform.position + perpendicularVector * DistanceOfTeleportation[levelOfModule];
            }
            else if (ship.State == Ship.States.HEALING && ship.GetComponent<Medicus_Module>().injuredShip != null)
            {
                //Vector2 injuredShipPosition = ship.GetComponent<Medicus_Module>().injuredShip.transform.position;
                //float sqrDistanceToinjuredShip = (injuredShipPosition - (Vector2)ship.transform.position).sqrMagnitude;
                //if (sqrDistanceToinjuredShip < DistanceOfTeleportation[levelOfModule] * DistanceOfTeleportation[levelOfModule])
                //{
                //    __newPosition = injuredShipPosition;
                //}
            }

            return __newPosition;
        }





        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, DistanceOfTeleportation[levelOfModule]);
            

            if (ship.State == Ship.States.FIGHT && ship.enemyCurrent != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(ship.enemyCurrent.transform.position, ship.radiusSize + ship.attack_range);
            }
            
        }



    }
}