using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class CookShop_Module : Module
    {
        private BattleSceneController battleSceneController;
        public PreUSP preUSP;
        [SerializeField] private float timer;
        [SerializeField] private int quantity = -1;
        private void Awake()
        {
            moduleType = Moduls.COOKSHOP_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            battleSceneController = BattleSceneController.Instance;

            if (battleSceneController != null)
                preUSP = Prefabs.Instance.preUSP;

            timer = 0;//при размножении у нового микрота таймер обнуляется

            //это позволяет новому микроту иметь столько же PreUSP, сколько было у "родителя", а не Quantity[levelOfModule]
            //иначе производство еды будет бесконечным
            if (quantity == -1)
                quantity = Quantity[LevelOfModule];
            //StartCoroutine(CreatePreUSP());
            StartCoroutine(Timer());
        }

        private new static readonly float[] ModuleData =
        {
        0,
        20f,    19.8f,  19.6f,  19.41f, 19.21f, 19.02f, 18.83f, 18.64f, 18.45f, 18.27f,
        18.09f, 17.91f, 17.73f, 17.55f, 17.37f, 17.2f,  17.03f, 16.86f, 16.69f, 16.52f,
        16.36f, 16.19f, 16.03f, 15.87f, 15.71f, 15.56f, 15.4f,  15.25f, 15.09f, 14.94f,
        14.79f, 14.65f, 14.5f,  14.35f, 14.21f, 14.07f, 13.93f, 13.79f, 13.65f, 13.51f,
        13.38f, 13.25f, 13.11f, 12.98f, 12.85f, 12.72f, 12.6f,  12.47f, 12.35f, 12.22f,
        12.1f,  11.98f, 11.86f, 11.74f, 11.62f, 11.51f, 11.39f, 11.28f, 11.17f, 11.05f,
        10.94f, 10.83f, 10.73f, 10.62f, 10.51f, 10.41f, 10.3f,  10.2f,  10.1f,  10f,
        9.9f,   9.8f,   9.7f,   9.6f,   9.51f,  9.41f,  9.32f,  9.22f,  9.13f,  9.04f,
        8.95f,  8.86f,  8.77f,  8.68f,  8.6f,   8.51f,  8.43f,  8.34f,  8.26f,  8.18f,
        8.09f,  8.01f,  7.93f,  7.85f,  7.78f,  7.7f,   7.62f,  7.54f,  7.47f,  7.39f,
        7.17f,  6.96f,  6.75f,  6.55f,  6.35f,  6.16f,  5.97f,  5.8f,   5.62f,  5.45f


    };

        private static readonly int[] Quantity =
        {
        0,
        8,  8,  8,  8,  8,  8,  8,  9,  9,  9,
        9,  9,  9,  9,  9,  9,  9,  9,  10, 10,
        10, 10, 10, 10, 10, 10, 10, 10, 11, 11,
        11, 11, 11, 11, 11, 11, 11, 12, 12, 12,
        12, 12, 12, 12, 12, 13, 13, 13, 13, 13,
        13, 13, 13, 14, 14, 14, 14, 14, 14, 14,
        15, 15, 15, 15, 15, 15, 15, 16, 16, 16,
        16, 16, 16, 17, 17, 17, 17, 17, 17, 18,
        18, 18, 18, 18, 18, 19, 19, 19, 19, 19,
        20, 20, 20, 20, 20, 21, 21, 21, 21, 21,
        22, 23, 23, 24, 25, 26, 26, 27, 28, 29
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


        //отсчет времени до создания новой еды идет только в состояниях FLEE, IDLE, TO_USP.
        private IEnumerator Timer()
        {
            if (battleSceneController == null)
                yield break;

            while (quantity > 0)
            {
                yield return null;

                if (ship.State == Ship.States.FLEE || ship.State == Ship.States.IDLE || ship.State == Ship.States.TO_USP)
                {
                    timer += Time.deltaTime;
                }

                if (timer > ModuleData[LevelOfModule])
                {
                    CreatePreUSP();
                    if (UltimateImpactAction())
                    {
                        CreatePreUSP(false);
                    }

                    timer = 0;
                }
            }
        }

        /// <summary>
        /// Создает preUSP.
        /// </summary>
        /// <param name="withDecreasing">С уменьшением количества запаса preUSP.</param>
        private void CreatePreUSP(bool withDecreasing = true)
        {
            if (withDecreasing)
            {
                quantity--;
            }

            //print("created PreUSP");
            PreUSP _preUSP = Instantiate(preUSP);

            float sizeOfPreUSP = _preUSP.GetComponent<CircleCollider2D>().bounds.extents.x;
            Vector2 randomPositionOnRadiusShip = Random.insideUnitCircle.normalized * (ship.radiusSize + sizeOfPreUSP);

            _preUSP.transform.position = randomPositionOnRadiusShip + (Vector2)ship.transform.position;
            //print("origin position of preUSP = " + _preUSP.transform.position);

            _preUSP.StartCoroutine(_preUSP.GoingOut(ship.transform.position));
        }
    }
}