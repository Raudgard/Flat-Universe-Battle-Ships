using UnityEngine;
namespace MODULES
{
    public class Bash_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.BASH_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.chanceToStun = ModuleData[LevelOfModule];
            if (TryGetComponent(out Attack_Module attack_Module))
            {
                attack_Module.onProjectileCreated += Attack_Module_onProjectileCreated;
            }
        }

        private void Attack_Module_onProjectileCreated(ProjectileСontainer projectileContainer, Module attack_Module)
        {
            if (GameEngineAssistant.GetProbability(ModuleData[LevelOfModule]))
            {
                StunProjectile projectileWithStun = Instantiate(Prefabs.Instance.stunProjectile);
                projectileWithStun.shipWhoFired = ship;
                projectileWithStun.direction = projectileContainer.direction;
                if (UltimateImpactAction())
                {
                    projectileWithStun.UltimateEffect();
                }

                projectileContainer.AddProjectile(projectileWithStun);
            }
        }

        

        public new static readonly float[] ModuleData =
        {
            0,
            8f, 8.08f,  8.16f,  8.24f,  8.32f,  8.41f,  8.49f,  8.58f,  8.66f,  8.75f,
            8.84f,  8.93f,  9.01f,  9.1f,   9.2f,   9.29f,  9.38f,  9.47f,  9.57f,  9.66f,
            9.76f,  9.86f,  9.96f,  10.06f, 10.16f, 10.26f, 10.36f, 10.47f, 10.57f, 10.68f,
            10.78f, 10.89f, 11f,    11.11f, 11.22f, 11.33f, 11.45f, 11.56f, 11.68f, 11.79f,
            11.91f, 12.03f, 12.15f, 12.27f, 12.39f, 12.52f, 12.64f, 12.77f, 12.9f,  13.03f,
            13.16f, 13.29f, 13.42f, 13.56f, 13.69f, 13.83f, 13.97f, 14.11f, 14.25f, 14.39f,
            14.53f, 14.68f, 14.83f, 14.97f, 15.12f, 15.27f, 15.43f, 15.58f, 15.74f, 15.9f,
            16.05f, 16.21f, 16.38f, 16.54f, 16.71f, 16.87f, 17.04f, 17.21f, 17.38f, 17.56f,
            17.73f, 17.91f, 18.09f, 18.27f, 18.45f, 18.64f, 18.82f, 19.01f, 19.2f,  19.4f,
            19.59f, 19.78f, 19.98f, 20.18f, 20.38f, 20.59f, 20.79f, 21f,    21.21f, 21.42f,
            22.07f, 22.73f, 23.41f, 24.11f, 24.84f, 25.58f, 26.35f, 27.14f, 27.95f, 28.79f,

        };  //в процентах

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

    }
}