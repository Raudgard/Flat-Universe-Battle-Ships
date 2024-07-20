using UnityEngine;

namespace MODULES
{
    public class USP_Power_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.USP_POWER_MODULE;
        }

        protected override void Start()
        {
            base.Start();

            if (TryGetComponent(out Attack_Module attack_Module))
            {
                //attack_Module.beforeProjectileCreate += Attack_Module_beforeProjectileCreate;
                attack_Module.onProjectileCreated += Attack_Module_onProjectileCreated;

            }
        }

        private void Attack_Module_onProjectileCreated(Projectile—ontainer projectileContainer, Module attack_Module)
        {
            if (ship.USPTakens > 1 && UltimateImpactAction())
            {
                CreateAndAddUSPPowerProjectile(projectileContainer);
                CreateAndAddUSPPowerProjectile(projectileContainer);
                ship.USPTakens -= 2;
            }
            else if (ship.USPTakens > 0)
            {
                CreateAndAddUSPPowerProjectile(projectileContainer);
                ship.USPTakens--;
            }
        }


        private void CreateAndAddUSPPowerProjectile(Projectile—ontainer projectile—ontainer)
        {
            float coeff = ModuleData[levelOfModule] / 100;
            DamageProjectile _projectile = Instantiate(Prefabs.Instance.projectile_USP_Power);
            _projectile.damage = (int)(projectile—ontainer.damage * coeff);
            _projectile.projectileTransform.localScale = new Vector3(_projectile.projectileTransform.localScale.x * coeff, _projectile.projectileTransform.localScale.y * coeff, _projectile.projectileTransform.localScale.z);
            _projectile.trail.startWidth *= coeff;
            _projectile.trail.time *= coeff;
            _projectile.direction = projectile—ontainer.direction;
            _projectile.shipWhoFired = ship;
            projectile—ontainer.AddProjectile(_projectile);
        }



        public new static readonly float[] ModuleData =
        {
            0f,
100f,   100.5f, 101f,   101.51f,    102.02f,    102.53f,    103.04f,    103.55f,    104.07f,    104.59f,
105.11f,    105.64f,    106.17f,    106.7f, 107.23f,    107.77f,    108.31f,    108.85f,    109.39f,    109.94f,
110.49f,    111.04f,    111.6f, 112.16f,    112.72f,    113.28f,    113.85f,    114.42f,    114.99f,    115.56f,
116.14f,    116.72f,    117.3f, 117.89f,    118.48f,    119.07f,    119.67f,    120.27f,    120.87f,    121.47f,
122.08f,    122.69f,    123.3f, 123.92f,    124.54f,    125.16f,    125.79f,    126.42f,    127.05f,    127.68f,
128.32f,    128.96f,    129.61f,    130.26f,    130.91f,    131.56f,    132.22f,    132.88f,    133.55f,    134.21f,
134.89f,    135.56f,    136.24f,    136.92f,    137.6f, 138.29f,    138.98f,    139.68f,    140.38f,    141.08f,
141.78f,    142.49f,    143.2f, 143.92f,    144.64f,    145.36f,    146.09f,    146.82f,    147.55f,    148.29f,
149.03f,    149.78f,    150.53f,    151.28f,    152.04f,    152.8f, 153.56f,    154.33f,    155.1f, 155.88f,
156.66f,    157.44f,    158.23f,    159.02f,    159.81f,    160.61f,    161.41f,    162.22f,    163.03f,    163.85f,
166.31f,    168.8f, 171.33f,    173.9f, 176.51f,    179.16f,    181.85f,    184.57f,    187.34f,    190.15f,



        };  //‚ ÔÓˆÂÌÚ‡ı

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