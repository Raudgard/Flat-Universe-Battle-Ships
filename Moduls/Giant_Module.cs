using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Giant_Module : Module
    {
        [SerializeField] private readonly float secondNeedForDigestion = 0.75f; // 12/16  время, необходимое для переваривания
        public Vector3 originalSize = Vector3.zero;
        private int originalAttackDamage;
        [SerializeField] private int originalHealthMax;
        public float originalMoveSpeed = -1;
        private float originalVisionRadius;
        [SerializeField] private float originalMass = -1;
        //public bool reachMaxSize = false;
        [SerializeField] private float timePassedFromStartDigestion = 0;
        public Vector3 nextScale;   //размер, который должен быть у микрота, при поедании очередной еды (ввел из-за Teleportation_Module)

        private float attackAdditive = 0;
        private float healthAdditive = 0;
        private float moveSpeedReduction = 0;
        private float minRotationSpeed = 0.694f; // = 5 / 7,2. Потому что 720 / 100 = 7,2. Та же разница, что между максимальным размером корабля и обычным.
        private float maxRotationSpeed = 5;

        private float originalExtents;

        public Coroutine digestionCoroutine;
        private Attack_Module attack_Module;
        private Rotation_Controller rotation_Control;
        private Rigidbody2D rb;
        private RectTransform[] strips = new RectTransform[3];
        private ShipVisualControllerForGiant visualEffectsController;
        [SerializeField] private float stripsOriginalLocalScaleHeight;
        [SerializeField] private float stripsOriginalPositionY;

        private void Awake()
        {
            ship = GetComponent<Ship>();
            rb = GetComponent<Rigidbody2D>();

            if (originalSize != Vector3.zero) transform.localScale = originalSize;
            if(originalMoveSpeed != -1) ship.move_speed = originalMoveSpeed;
            if(originalMass != -1) rb.mass = originalMass;

            originalExtents = ship.mainCollider.bounds.extents.x;
            moduleType = Moduls.GIANT_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            //print("GM  ship name: " + ship.name);
            attack_Module = GetComponent<Attack_Module>();
            rotation_Control = GetComponent<Rotation_Controller>();
            ship.USP_taken = USPTaken;  //меняем метод, выполняющийся при получении USP.
            
            //меняем компонент, отвечающий за визуализацию эффектов.
            if (!ship.TryGetComponent(out visualEffectsController))
            {
                Destroy(ship.GetComponent<ShipVisualController>());
                ship.shipVisualController = visualEffectsController = ship.gameObject.AddComponent<ShipVisualControllerForGiant>();
            }

            StartCoroutine(UpdateRadiusSizeVisionRadiusAndSpriteMask());//чтобы после размножения присваивался правильный ("маленький") radiusSize, правильный VisionRasius и обновлялся размер SpriteMask
            var originalStrip = Prefabs.Instance.ship.gameObject.GetComponentInChildren<Canvas>().GetComponentsInChildren<RectTransform>()[3];
            stripsOriginalLocalScaleHeight = originalStrip.localScale.y;
            stripsOriginalPositionY = originalStrip.localPosition.y;
            var transforms = ship.GetComponentInChildren<Canvas>().GetComponentsInChildren<RectTransform>();
            strips[0] = transforms[1];
            strips[1] = transforms[2];
            strips[2] = transforms[3];
            strips[0].localScale = strips[1].localScale = strips[2].localScale = originalStrip.localScale;
            strips[2].localPosition = originalStrip.localPosition;

            StartCoroutine(GetOriginalParameters());
            nextScale = originalSize = ship.transform.localScale;
            originalMass = rb.mass;

            ship.changedSize = ship.transform.localScale.x / originalSize.x;
            ApplyRotationSpeed();
            //reachMaxSize = false;
        }

        private IEnumerator UpdateRadiusSizeVisionRadiusAndSpriteMask()
        {
            yield return null;
            yield return null;

            ship.radiusSize = GetComponent<CircleCollider2D>().bounds.extents.x;//чтобы после размножения присваивался правильный ("маленький") radiusSize
            ship.VisionRadius = originalVisionRadius;//правильный VisionRadius

            //if (spriteMask != null)
            //    spriteMask.transform.localScale = new Vector3(ship.VisionRadius, ship.VisionRadius); // и правильный spriteMask

            //print("radiusSize = " + ship.radiusSize + "    extents.x = " + GetComponent<CircleCollider2D>().bounds.extents.x);
        }

        /// <summary>
        /// Сохраняем начальные показатели с задержкой в 1 кадр, чтобы другие модули могли применить свои изменения.
        /// Иначе рассчет будет некорректным, т.к. если Giant_Module добавляется раньше какого-либо из изменяющих основные параметры модулей,
        /// то его метод Start() срабатывает раньше их и в original(Parameter) сохраняется неверное значение.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetOriginalParameters()
        {
            yield return null;
            originalAttackDamage = ship.attack_damage;
            originalHealthMax = ship.healthMax;
            originalMoveSpeed = ship.move_speed;
            originalVisionRadius = ship.VisionRadius - ship.radiusSize;
        }

        private new static readonly float[] ModuleData =
        {
        0,
        6.67f,  6.7f,   6.73f,  6.77f,  6.8f,   6.84f,  6.87f,  6.9f,   6.94f,  6.97f,
        7.01f,  7.04f,  7.08f,  7.11f,  7.14f,  7.18f,  7.21f,  7.25f,  7.29f,  7.32f,
        7.36f,  7.4f,   7.43f,  7.47f,  7.51f,  7.55f,  7.58f,  7.62f,  7.66f,  7.69f,
        7.73f,  7.77f,  7.81f,  7.85f,  7.89f,  7.93f,  7.97f,  8.01f,  8.05f,  8.09f,
        8.13f,  8.17f,  8.21f,  8.25f,  8.29f,  8.33f,  8.38f,  8.42f,  8.46f,  8.5f,
        8.54f,  8.59f,  8.63f,  8.67f,  8.72f,  8.76f,  8.8f,   8.85f,  8.89f,  8.94f,
        8.98f,  9.03f,  9.09f,  9.14f,  9.18f,  9.23f,  9.27f,  9.32f,  9.37f,  9.41f,
        9.46f,  9.51f,  9.56f,  9.6f,   9.65f,  9.7f,   9.75f,  9.8f,   9.85f,  9.9f,
        9.94f,  10f,    10.05f, 10.1f,  10.15f, 10.2f,  10.25f, 10.3f,  10.36f, 10.41f,
        10.46f, 10.51f, 10.56f, 10.62f, 10.67f, 10.72f, 10.78f, 10.83f, 10.88f, 10.94f,
        11.11f, 11.28f, 11.45f, 11.62f, 11.79f, 11.97f, 12.15f, 12.33f, 12.5f,  12.69f

    };  //в процентах. Увеличение HP и атаки

        private static readonly int[] MaximumSize =
        {
        100,
        200,    202,    204,    206,    208,    210,    212,    214,    217,    219,
        221,    223,    225,    228,    230,    232,    235,    237,    239,    242,
        244,    246,    249,    251,    254,    256,    259,    262,    264,    267,
        270,    272,    275,    278,    281,    283,    286,    289,    292,    295,
        298,    301,    304,    307,    310,    313,    316,    319,    322,    326,
        329,    332,    336,    339,    342,    346,    349,    353,    356,    360,
        363,    367,    371,    374,    378,    382,    386,    390,    393,    397,
        401,    405,    409,    414,    418,    422,    426,    430,    435,    439,
        443,    448,    452,    457,    461,    466,    471,    475,    480,    485,
        490,    495,    500,    505,    510,    515,    520,    525,    530,    536,
        552,    568,    585,    603,    621,    640,    659,    678,    699,    720

    };//максимальный размер микрота в % от оригинального

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



        public void USPTaken()
        {
            if (ship.transform.localScale.x >= originalSize.x * MaximumSize[LevelOfModule] / 100)
            {
                //print("Maximum Size!");
                ship.USPTakens++;
                ship.shipVisualController.USP_Taken();

                if (ship.USPTakens >= ship.USPNeedToReproduction)
                    ship.Reproduction();

                return;
            }

            nextScale = ship.transform.localScale + originalSize * ModuleData[levelOfModule] / 100;
            Digestion();
        }

        public void Digestion()
        {
            ship.State = Ship.States.DIGESTION;

            //ship.movingFromForceCoroutine = ship.StartCoroutine(ship.MovingFromForce(Vector2.zero));//нужно что бы микрот медленно остановился

            visualEffectsController.DigestionStart();
            //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            //renderer.color = ship.colors.USP_color;

            digestionCoroutine = StartCoroutine(DigestionGo());
            StartCoroutine(Deseleration());
        }

        private IEnumerator DigestionGo()
        {
            Vector3 currentSize = ship.transform.localScale;

            if (timePassedFromStartDigestion < 0.01f)
                timePassedFromStartDigestion += secondNeedForDigestion;

            while (timePassedFromStartDigestion > 0)
            {
                //print("localScale = " + ship.transform.localScale);
                ship.transform.localScale += originalSize * (ModuleData[LevelOfModule] / 100) * Time.deltaTime * (1 / secondNeedForDigestion);

                yield return null;
                timePassedFromStartDigestion -= Time.deltaTime;
            }


            ship.transform.localScale = currentSize + originalSize * (ModuleData[LevelOfModule] / 100);
            ship.changedSize = ship.transform.localScale.x / originalSize.x;
            //исключаем случай отсутствия коллайдера (например, при использовании Evasion_Module)
            if(ship.mainCollider.bounds.extents.x != 0)
            {
                ship.radiusSize = ship.mainCollider.bounds.extents.x;
            }
            else
            {
                ship.radiusSize = originalExtents * ship.changedSize;
            }

            //print("radius = " + GetComponent<CircleCollider2D>().radius + "     offset.x = " + GetComponent<CircleCollider2D>().bounds.extents.x);
            DecreaseSizeOfStrips();
            ApplyRotationSpeed();

            attackAdditive += originalAttackDamage * ModuleData[LevelOfModule] / 100; //прибавка к атаке, рассчитаная на основе оригинальной атаки
            healthAdditive += originalHealthMax * ModuleData[LevelOfModule] / 100; // то же самое со здоровьем
            moveSpeedReduction += ship.move_speed * ModuleData[LevelOfModule] / 100 / 8; // и со скоростью

            ship.VisionRadius = originalVisionRadius;//обновляем радиус обзора


            ship.attack_damage = (int)(originalAttackDamage + attackAdditive);
            ship.healthMax = (int)(originalHealthMax + healthAdditive);
            ship.HealthCurrent += Mathf.CeilToInt(originalHealthMax * ModuleData[LevelOfModule] / 100);
            ship.move_speed = originalMoveSpeed - moveSpeedReduction;
            rb.mass = originalMass * ship.changedSize;

            //print("originalAttack = " + originalAttackDamage + "\noriginalHealthMax = " + originalHealthMax + "      originalMoveSpeed = " + originalMoveSpeed);
            //print("attackAdditive = " + attackAdditive + "\nhealthAdditive = " + healthAdditive + "     moveSpeedReduction = " + moveSpeedReduction);

            //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            //renderer.color = new Color(1, 1, 1);
            visualEffectsController.DigestionEnd();

            //присоединяем маску обратно и задаем ей новый уменьшенный localScale, т.к. localScale микрота увеличился.
            //if (spriteMask != null)
            //{
            //    spriteMask.transform.parent = ship.transform;
            //    spriteMask.transform.localScale = new Vector3(ship.VisionRadius, ship.VisionRadius) * originalSize.x / (currentSize + originalSize * (ModuleData[LevelOfModule] / 100)).x;
            //}

            digestionCoroutine = null;

            EventManager.Instance.PostNotification(EVENT_TYPE.SHIPS_SIZE_CHANGED, this);

            //для того, чтобы отвлечься от поглощения USP, если начали бить. (Работает неровно, нужно тестить.)
            if (ship.shipLastToDoDamage != null && attack_Module != null)
            {
                StartCoroutine(attack_Module.ToFight(ship.shipLastToDoDamage));
                yield break;
            }

            if(ship.State != Ship.States.FLEE)
                ship.Idle();


        }

        /// <summary>
        /// Снижение скорости во время "переваривания"
        /// </summary>
        /// <returns></returns>
        private IEnumerator Deseleration()
        {
            while (digestionCoroutine != null && rb.velocity.sqrMagnitude > ship.deseleration_treshold && ship.State != Ship.States.FLEE) // while (_body.velocity.magnitude > 0.03f)// <- скорость, при которой микрот перестает "убегать"
            {
                ship.move_speed *= ship.deseleration_coeff;
                //print("moveDirection.sqrMagnitude = " + moveDirection.sqrMagnitude);
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Уменьшает толщину линеек жизни и заполнения USP
        /// </summary>
        private void DecreaseSizeOfStrips()
        {
            var decreaser = 1 / ship.changedSize;

            for(int i = 0; i < strips.Length; i++)
            {
                strips[i].localScale = new Vector3(strips[i].localScale.x, stripsOriginalLocalScaleHeight * decreaser, strips[i].localScale.z);
            }

            var position = strips[strips.Length - 1].localPosition;
            strips[strips.Length - 1].localPosition = new Vector3(position.x, stripsOriginalPositionY - strips[strips.Length - 1].rect.height * (1 - decreaser), position.z);
        }

        /// <summary>
        /// Уменьшаем скорость поворота корабля при увеличени размера.
        /// </summary>
        private void ApplyRotationSpeed()
        {
            rotation_Control.rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, Mathf.InverseLerp(MaximumSize[110], MaximumSize[0], ship.changedSize * 100));
        }

    }
}