namespace MODULES
{
    public class Rabbit_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.RABBIT_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            SetTimeForReproduction();

            ship.beforeStartReproduction += CheckForUltimateImpact;
            ship.afterEndTimeReproduction += SetTimeForReproduction;
        }

        private new static readonly float[] ModuleData =
        {

        16.0f,
        15.84f, 15.68f, 15.52f, 15.37f, 15.22f, 15.06f, 14.91f, 14.76f, 14.62f, 14.47f,
        14.33f, 14.18f, 14.04f, 13.9f,  13.76f, 13.62f, 13.49f, 13.35f, 13.22f, 13.09f,
        12.96f, 12.83f, 12.7f,  12.57f, 12.45f, 12.32f, 12.2f,  12.08f, 11.95f, 11.84f,
        11.72f, 11.6f,  11.48f, 11.37f, 11.26f, 11.14f, 11.03f, 10.92f, 10.81f, 10.7f,
        10.6f,  10.49f, 10.39f, 10.28f, 10.18f, 10.08f, 9.98f,  9.88f,  9.78f,  9.68f,
        9.58f,  9.49f,  9.39f,  9.3f,   9.21f,  9.11f,  9.02f,  8.93f,  8.84f,  8.75f,
        8.67f,  8.58f,  8.49f,  8.41f,  8.33f,  8.24f,  8.16f,  8.08f,  8f,     7.92f,
        7.84f,  7.76f,  7.68f,  7.61f,  7.53f,  7.45f,  7.38f,  7.31f,  7.23f,  7.16f,
        7.09f,  7.02f,  6.95f,  6.88f,  6.81f,  6.74f,  6.67f,  6.61f,  6.54f,  6.48f,
        6.41f,  6.35f,  6.28f,  6.22f,  6.16f,  6.1f,   6.04f,  5.98f,  5.92f,  5.86f,
        5.56f,  5.29f,  5.02f,  4.77f,  4.53f,  4.31f,  4.09f,  3.89f,  3.69f,  3.51f

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


        private void CheckForUltimateImpact()
        {
            if (UltimateImpactAction())
            {
                ship.secondsNeedToReproduction = 0;
            }
        }

        private void SetTimeForReproduction() => ship.secondsNeedToReproduction = ModuleData[LevelOfModule];

    }
}