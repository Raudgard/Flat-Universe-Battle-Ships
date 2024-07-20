using UnityEngine;
using System;

namespace MODULES
{
    public class Armor_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.ARMOR_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.armor = ModuleData[LevelOfModule];
            ship.takeHitComponent.impactOnDamageValue += ImpactOnDamage;
        }

        private new static readonly float[] ModuleData =
        {
            0,
            10f,    10.1f,  10.2f,  10.3f,  10.41f, 10.51f, 10.62f, 10.72f, 10.83f, 10.94f,
            11.05f, 11.16f, 11.27f, 11.38f, 11.49f, 11.61f, 11.73f, 11.84f, 11.96f, 12.08f,
            12.2f,  12.32f, 12.45f, 12.57f, 12.7f,  12.82f, 12.95f, 13.08f, 13.21f, 13.35f,
            13.48f, 13.61f, 13.75f, 13.89f, 14.03f, 14.17f, 14.31f, 14.45f, 14.6f,  14.74f,
            14.89f, 15.04f, 15.19f, 15.34f, 15.49f, 15.65f, 15.8f,  15.96f, 16.12f, 16.28f,
            16.45f, 16.61f, 16.78f, 16.94f, 17.11f, 17.29f, 17.46f, 17.63f, 17.81f, 17.99f,
            18.17f, 18.35f, 18.53f, 18.72f, 18.9f,  19.09f, 19.28f, 19.48f, 19.67f, 19.87f,
            20.07f, 20.27f, 20.47f, 20.68f, 20.88f, 21.09f, 21.3f,  21.52f, 21.73f, 21.95f,
            22.17f, 22.39f, 22.61f, 22.84f, 23.07f, 23.3f,  23.53f, 23.77f, 24f,    24.24f,
            24.49f, 24.73f, 24.98f, 25.23f, 25.48f, 25.74f, 25.99f, 26.25f, 26.52f, 26.78f,
            27.58f, 28.41f, 29.26f, 30.14f, 31.05f, 31.98f, 32.94f, 33.92f, 34.94f, 35.99f,

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



        private (int damage, Action visualEffect) ImpactOnDamage(int damage, Vector2 direction, Vector3 impactPoint)
        {
            if (UltimateImpactAction())
            {
                return (0, delegate { ship.shipVisualController.ArmorUltimateDamageTaken(direction, impactPoint); });
            }
            else
            {
                int damageRevised = (int)(damage * (100f - ModuleData[LevelOfModule]) / 100f);
                return (damageRevised, delegate { ship.shipVisualController.ExplosionAndDebrisWhenDamageTaken(damageRevised, direction, impactPoint); });
            }

        }


    }
}