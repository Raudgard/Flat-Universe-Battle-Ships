namespace MODULES
{
    public class Attack_Speed_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.ATTACK_SPEED_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.reload_time = ModuleData[LevelOfModule];

            if (TryGetComponent(out Attack_Module attack_Module))
            {
                attack_Module.onProjectileCreated += ProjectileCreated;
            }

            if (TryGetComponent(out Medicus_Module medicus_Module))
            {
                medicus_Module.onProjectileCreated += ProjectileCreated;
            }
        }

        private new static readonly float[] ModuleData =
        {
        2.0f,
        1.98f,  1.97f,  1.95f,  1.94f,  1.92f,  1.91f,  1.89f,  1.88f,  1.86f,  1.85f,
        1.83f,  1.82f,  1.8f,   1.79f,  1.77f,  1.76f,  1.75f,  1.73f,  1.72f,  1.71f,
        1.69f,  1.68f,  1.67f,  1.65f,  1.64f,  1.63f,  1.61f,  1.6f,   1.59f,  1.57f,
        1.56f,  1.55f,  1.54f,  1.53f,  1.51f,  1.5f,   1.49f,  1.48f,  1.47f,  1.45f,
        1.44f,  1.43f,  1.42f,  1.41f,  1.4f,   1.39f,  1.38f,  1.36f,  1.35f,  1.34f,
        1.33f,  1.32f,  1.31f,  1.3f,   1.29f,  1.28f,  1.27f,  1.26f,  1.25f,  1.24f,
        1.23f,  1.22f,  1.21f,  1.2f,   1.19f,  1.18f,  1.17f,  1.16f,  1.15f,  1.14f,
        1.14f,  1.13f,  1.12f,  1.11f,  1.1f,   1.09f,  1.08f,  1.07f,  1.07f,  1.06f,
        1.05f,  1.04f,  1.03f,  1.02f,  1.02f,  1.01f,  1f, 0.99f,  0.98f,  0.98f,
        0.97f,  0.96f,  0.95f,  0.95f,  0.94f,  0.93f,  0.92f,  0.92f,  0.91f,  0.9f,
        0.88f,  0.86f,  0.84f,  0.82f,  0.8f,   0.78f,  0.76f,  0.75f,  0.73f,  0.71f,

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




        private void ProjectileCreated(ProjectileСontainer projectileСontainer, Module module)
        {
            if (UltimateImpactAction())
            {
                ship.reload_time = 0.05f;
                //Минимум 2 кадра до возвращения нормального значения!!!
                Tools.UnityTools.ExecuteWithDelay(() => { ship.reload_time = ModuleData[LevelOfModule]; }, 2);
            }
        }


    }
}