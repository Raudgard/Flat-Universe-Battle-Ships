using UnityEngine;
using System.Collections;


namespace MODULES
{
    public class Movement_Speed_Module : Module
    {
        private bool IsBuffApplied { get; set; } = false;

        private System.Action CheckUltimate;
        [SerializeField] private int framesDelay = 1;

        private void Awake()
        {
            moduleType = Moduls.MOVEMENT_SPEED_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.move_speed = ModuleData[LevelOfModule];

            int nextFramesDelay = framesDelay + 1;
            framesDelay = nextFramesDelay > 5 ? 1 : nextFramesDelay;
            //Debug.Log($"framesDelay: {framesDelay}");

            //При изменении статуса будет проверять срабатывание ультимейта с задержкой от 1 до 5 кадров,
            //чтобы не получилась одновременная проверка большого количества кораблей.
            CheckUltimate = framesDelay switch
            {
                1 => CheckWith1FrameDelay,
                2 => CheckWith2FrameDelay,
                3 => CheckWith3FrameDelay,
                4 => CheckWith4FrameDelay,
                5 => CheckWith5FrameDelay,
                _ => throw new System.NotImplementedException()
            };

            ship.StateChanged += CheckUltimate;
            ship.shipVisualController.AddEngineFlameParticleSystem();
            //ship.shipVisualController.AddEngineFlameTrailRenderer();
            ship.shipVisualController.EngineFlameOff();

            ship.MoveDirectionZeroEvent += delegate { ship.shipVisualController.EngineFlameOff(); };

        }


        private new static readonly float[] ModuleData =
        {
            40f,
            40.12f, 40.24f, 40.36f, 40.48f, 40.6f,  40.73f, 40.85f, 40.97f, 41.09f, 41.22f,
            41.34f, 41.46f, 41.59f, 41.71f, 41.84f, 41.96f, 42.09f, 42.22f, 42.34f, 42.47f,
            42.6f,  42.72f, 42.85f, 42.98f, 43.11f, 43.24f, 43.37f, 43.5f,  43.63f, 43.76f,
            43.89f, 44.02f, 44.16f, 44.29f, 44.42f, 44.55f, 44.69f, 44.82f, 44.96f, 45.09f,
            45.23f, 45.36f, 45.5f,  45.64f, 45.77f, 45.91f, 46.05f, 46.19f, 46.32f, 46.46f,
            46.6f,  46.74f, 46.88f, 47.02f, 47.16f, 47.31f, 47.45f, 47.59f, 47.73f, 47.88f,
            48.02f, 48.16f, 48.31f, 48.45f, 48.6f,  48.74f, 48.89f, 49.04f, 49.18f, 49.33f,
            49.48f, 49.63f, 49.78f, 49.93f, 50.08f, 50.23f, 50.38f, 50.53f, 50.68f, 50.83f,
            50.98f, 51.14f, 51.29f, 51.44f, 51.6f,  51.75f, 51.91f, 52.06f, 52.22f, 52.38f,
            52.53f, 52.69f, 52.85f, 53.01f, 53.17f, 53.33f, 53.49f, 53.65f, 53.81f, 53.97f,
            54.51f, 55.05f, 55.61f, 56.16f, 56.72f, 57.29f, 57.86f, 58.44f, 59.03f, 59.62f,

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


        private void CheckWith1FrameDelay()
        {
            StartCoroutine(CheckWith1FrameDelayCoroutine());
        }
        private void CheckWith2FrameDelay()
        {
            StartCoroutine(CheckWith2FrameDelayCoroutine());
        }
        private void CheckWith3FrameDelay()
        {
            StartCoroutine(CheckWith3FrameDelayCoroutine());
        }
        private void CheckWith4FrameDelay()
        {
            StartCoroutine(CheckWith4FrameDelayCoroutine());
        }
        private void CheckWith5FrameDelay()
        {
            StartCoroutine(CheckWith5FrameDelayCoroutine());
        }


        private IEnumerator CheckWith1FrameDelayCoroutine()
        {
            yield return null;
            CheckForUltimateImpact();
        }

        private IEnumerator CheckWith2FrameDelayCoroutine()
        {
            yield return null;
            yield return null;
            CheckForUltimateImpact();
        }

        private IEnumerator CheckWith3FrameDelayCoroutine()
        {
            yield return null;
            yield return null;
            yield return null;
            CheckForUltimateImpact();
        }

        private IEnumerator CheckWith4FrameDelayCoroutine()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            CheckForUltimateImpact();
        }

        private IEnumerator CheckWith5FrameDelayCoroutine()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            CheckForUltimateImpact();
        }



        private void CheckForUltimateImpact()
        {
            Debug.Log($"State changed to: {ship.State}");

            if(ship.State != Ship.States.FIGHT &&
                ship.State != Ship.States.SEARCHING_FOR_ENEMY &&
                ship.State != Ship.States.TO_USP)
            {
                if (IsBuffApplied)
                {
                    //Debug.Log($"buff already applyed. Debuff!  ship.State: {ship.State}");
                    ship.move_speed -= ModuleData[LevelOfModule];
                    IsBuffApplied = false;
                    ship.shipVisualController.EngineFlameOff();
                }
                return; 
            }

            if (UltimateImpactAction())
            {
                //Debug.Log($"ULTIMATE IMPACT!    ship.State: {ship.State}");
                if (!IsBuffApplied)
                {
                    //Debug.Log($"buff NOT APPLIED. apply buff   ship.State: {ship.State}");
                    ship.move_speed += ModuleData[LevelOfModule];
                    IsBuffApplied = true;
                    ship.shipVisualController.EngineFlameOn();
                }
            }
            else
            {
                //Debug.Log($"Ultimate NOT Impact.  ship.State: {ship.State}");
                if (IsBuffApplied)
                {
                    //Debug.Log($"buff already applyed. Debuff!  ship.State: {ship.State}");
                    ship.move_speed -= ModuleData[LevelOfModule];
                    IsBuffApplied = false;
                    ship.shipVisualController.EngineFlameOff();
                }
            }
        }

    }
}