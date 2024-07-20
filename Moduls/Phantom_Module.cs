//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//namespace MODULES
//{
//    public class Phantom_Module : Module
//    {
//        public float timeOfLife; //сколько времени живет фантом
//        //private readonly float timeForNewPhantom = 10.0f; //как часто появляется новый фантом
//        //public int numberOfHitsForDeath; //сколько ударов выдерживает перед исчезновением
//                                         //private Battle_Scene_Controller battleSceneController;
//        private Global_Controller global_Data;
//        private Scene activeScene;


//        private void Awake()
//        {
//            moduleType = Moduls.PHANTOM_MODULE;

//            timeOfLife = ModuleData[LevelOfModule];
//            //numberOfHitsForDeath = NumberOfHitsForDeath[LevelOfModule];
//        }

//        void Start()
//        {
//            ship = GetComponent<Ship>();
//            global_Data = Global_Controller.Instance;

//            activeScene = SceneManager.GetActiveScene();
//            StartCoroutine(Phantoming());

//        }

//        private new static readonly float[] ModuleData =
//        {
//            0,
//            5f, 5.05f,  5.1f,   5.15f,  5.2f,   5.26f,  5.31f,  5.36f,  5.41f,  5.47f,
//            5.52f,  5.58f,  5.63f,  5.69f,  5.75f,  5.8f,   5.86f,  5.92f,  5.98f,  6.04f,
//            6.1f,   6.16f,  6.22f,  6.29f,  6.35f,  6.41f,  6.48f,  6.54f,  6.61f,  6.67f,
//            6.74f,  6.81f,  6.87f,  6.94f,  7.01f,  7.08f,  7.15f,  7.23f,  7.3f,   7.37f,
//            7.44f,  7.52f,  7.59f,  7.67f,  7.75f,  7.82f,  7.9f,   7.98f,  8.06f,  8.14f,
//            8.22f,  8.31f,  8.39f,  8.47f,  8.56f,  8.64f,  8.73f,  8.82f,  8.9f,   8.99f,
//            9.08f,  9.17f,  9.27f,  9.36f,  9.45f,  9.55f,  9.64f,  9.74f,  9.84f,  9.93f,
//            10.03f, 10.13f, 10.24f, 10.34f, 10.44f, 10.55f, 10.65f, 10.76f, 10.87f, 10.97f,
//            11.08f, 11.19f, 11.31f, 11.42f, 11.53f, 11.65f, 11.77f, 11.88f, 12f,    12.12f,
//            12.24f, 12.37f, 12.49f, 12.61f, 12.74f, 12.87f, 13f,    13.13f, 13.26f, 13.39f,
//            13.79f, 14.21f, 14.63f, 15.07f, 15.52f, 15.99f, 16.47f, 16.96f, 17.47f, 18f
//        };

//        private static readonly int[] NumberOfHitsForDeath =
//        {
//            0,
//            3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
//            3,  3,  3,  3,  3,  3,  4,  4,  4,  4,
//            4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
//            4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
//            4,  5,  5,  5,  5,  5,  5,  5,  5,  5,
//            5,  5,  5,  5,  5,  5,  5,  5,  5,  5,
//            5,  6,  6,  6,  6,  6,  6,  6,  6,  6,
//            6,  6,  6,  6,  6,  6,  6,  6,  7,  7,
//            7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
//            7,  7,  7,  8,  8,  8,  8,  8,  8,  8,
//            8,  9,  9,  9,  9,  10, 10, 10, 10, 11
//        };

//        public override int LevelOfModule
//        {
//            get
//            {
//                return levelOfModule;
//            }
//            set
//            {
//                if (value < 1)
//                    levelOfModule = 1;
//                else if (value > ModuleData.Length - 1)
//                    levelOfModule = ModuleData.Length - 1;
//                else
//                    levelOfModule = value;
//            }
//        }
//        public static int GetMaxLevel() => ModuleData.Length - 1;

//        private IEnumerator Phantoming()
//        {
//            if (activeScene.name != "ScirmishScene" && activeScene.name != "Maze")
//                yield break;

//            var lerp = Mathf.Lerp(2, 1, (float)levelOfModule / GetMaxLevel());          // по балансу нужно будет прикинуть еще. Пока (если от 2 до 1) получается, что фантом живет
//                                                                                        // на 1 уровне в два раза меньше от своего времени появления, на последнем уровне - ровно 
//                                                                                        // столько, чтоб сразу после его смерти появился новый фантом. Возможно, нужно будет усилить.
//                                                                                        // Например, от 1,5 до 0,8. 
//            float spawnTime = ModuleData[levelOfModule] * lerp;

//            while (gameObject != null)
//            {
//                yield return new WaitForSeconds(spawnTime + Random.Range(-0.1f, 0.1f));

//                GameObject newGameObject = Instantiate(gameObject) as GameObject;
//                //фантом появляется рядом с микротом, а не внутри, как при размножении

//                newGameObject.name = $"Ship({ship.shipName} Phantom)";
//                Ship _newShip = newGameObject.GetComponent<Ship>();

//                if (_newShip.TryGetComponent(out Giant_Module _))
//                {
//                    _newShip.radiusSize = ship.mainCollider.bounds.extents.x * GetComponent<Giant_Module>().originalSize.x / _newShip.transform.localScale.x;
//                    _newShip.transform.localScale = GetComponent<Giant_Module>().originalSize;
//                    _newShip.move_speed = GetComponent<Giant_Module>().originalMoveSpeed;
//                }
//                _newShip.VisionRadius = 3.8f;

//                newGameObject.transform.position = Random.insideUnitCircle.normalized * (ship.radiusSize + _newShip.radiusSize) + (Vector2)ship.transform.position;

//                global_Data.AddShip(_newShip);

//                //yield return null;//ждем 1 кадр для применения всех методов внутри нового микрота во избежание глюков
//                _newShip.USPTakens = 0;
//                //_newShip.healthBar.fillAmount = (float)_newShip.HealthCurrent / _newShip.healthMax;
//                _newShip.healthMax = NumberOfHitsForDeath[levelOfModule];
//                _newShip.HealthCurrent = _newShip.healthMax;

//                newGameObject.AddComponent<Phantom>().numberOfHitsForDeath = NumberOfHitsForDeath[LevelOfModule];


                

//            }
//        }


//    }
//}