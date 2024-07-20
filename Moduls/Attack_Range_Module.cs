using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Attack_Range_Module : Module
    {

        private void Awake()
        {
            moduleType = Moduls.ATTACK_RANGE_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.attack_range = ModuleData[LevelOfModule];

            if (ship.TryGetComponent(out Attack_Module attack_Module))
            {
                attack_Module.onProjectileCreated += OnProjectileCreated;
            }

            if (ship.TryGetComponent(out Medicus_Module medicus_Module))
            {
                medicus_Module.onProjectileCreated += OnProjectileCreated;
            }
        }

        

        private new static readonly float[] ModuleData =
        {
            5f,
            5.03f,  5.05f,  5.08f,  5.1f,   5.13f,  5.15f,  5.18f,  5.2f,   5.23f,  5.26f,
            5.28f,  5.31f,  5.33f,  5.36f,  5.39f,  5.42f,  5.44f,  5.47f,  5.5f,   5.52f,
            5.55f,  5.58f,  5.61f,  5.64f,  5.66f,  5.69f,  5.72f,  5.75f,  5.78f,  5.81f,
            5.84f,  5.87f,  5.89f,  5.92f,  5.95f,  5.98f,  6.01f,  6.04f,  6.07f,  6.1f,
            6.13f,  6.17f,  6.2f,   6.23f,  6.26f,  6.29f,  6.32f,  6.35f,  6.38f,  6.42f,
            6.45f,  6.48f,  6.51f,  6.55f,  6.58f,  6.61f,  6.64f,  6.68f,  6.71f,  6.74f,
            6.78f,  6.81f,  6.85f,  6.88f,  6.91f,  6.95f,  6.98f,  7.02f,  7.05f,  7.09f,
            7.12f,  7.16f,  7.2f,   7.23f,  7.27f,  7.3f,   7.34f,  7.38f,  7.41f,  7.45f,
            7.49f,  7.53f,  7.56f,  7.6f,   7.64f,  7.68f,  7.72f,  7.76f,  7.79f,  7.83f,
            7.87f,  7.91f,  7.95f,  7.99f,  8.03f,  8.07f,  8.11f,  8.15f,  8.19f,  8.23f,
            8.36f,  8.48f,  8.61f,  8.74f,  8.87f,  9f, 9.14f,  9.27f,  9.41f,  9.56f,

        };

        private static readonly float[] UltimateAddition =
        {
            0.5f,
            0.51f,  0.51f,  0.52f,  0.52f,  0.53f,  0.53f,  0.54f,  0.54f,  0.55f,  0.55f,
            0.56f,  0.56f,  0.57f,  0.57f,  0.58f,  0.59f,  0.59f,  0.6f,   0.6f,   0.61f,
            0.62f,  0.62f,  0.63f,  0.63f,  0.64f,  0.65f,  0.65f,  0.66f,  0.67f,  0.67f,
            0.68f,  0.69f,  0.69f,  0.7f,   0.71f,  0.72f,  0.72f,  0.73f,  0.74f,  0.74f,
            0.75f,  0.76f,  0.77f,  0.77f,  0.78f,  0.79f,  0.8f,   0.81f,  0.81f,  0.82f,
            0.83f,  0.84f,  0.85f,  0.86f,  0.86f,  0.87f,  0.88f,  0.89f,  0.9f,   0.91f,
            0.92f,  0.93f,  0.94f,  0.95f,  0.95f,  0.96f,  0.97f,  0.98f,  0.99f,  1f,
            1.01f,  1.02f,  1.03f,  1.04f,  1.05f,  1.07f,  1.08f,  1.09f,  1.1f,   1.11f,
            1.12f,  1.13f,  1.14f,  1.15f,  1.16f,  1.18f,  1.19f,  1.2f,   1.21f,  1.22f,
            1.24f,  1.25f,  1.26f,  1.27f,  1.29f,  1.3f,   1.31f,  1.33f,  1.34f,  1.35f,
            1.39f,  1.43f,  1.48f,  1.52f,  1.57f,  1.61f,  1.66f,  1.71f,  1.76f,  1.82f,
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


        private void OnProjectileCreated(ProjectileСontainer arg1, Module arg2)
        {
            if (UltimateImpactAction())
            {
                ship.attack_range += UltimateAddition[levelOfModule];
                //Debug.Log($"ship: {ship.name}, attack_range: {ship.attack_range}");

            }


        }


    }
}