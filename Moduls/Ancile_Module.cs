using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Ancile_Module : Module
    {
        public Ancile ancile;

        private void Awake()
        {
            moduleType = Moduls.ANCILE_MODULE;
        }

        protected override void Start()
        {
            base.Start();

            if (ancile != null)
            {
                Destroy(ancile.gameObject);
            }


            ancile = Instantiate(Prefabs.Instance.ancile, ship.transform);
            ancile.Initialize(ship, this, ModuleData[levelOfModule], coverDegrees[levelOfModule], recoveryTime[levelOfModule]);
            //ancile = ancileGO.GetComponentInChildren<Ancile>();
            ancile.transform.localPosition = Prefabs.Instance.ancile.transform.localPosition;
            ancile.transform.localScale = Vector3.one;
            var rotationZ = new System.Random().Next(0, 360);
            ancile.transform.eulerAngles = new Vector3(0, 0, rotationZ);
            ancile.shipRotationController = ship.rotation_Controller;
        }

        private new static readonly int[] ModuleData =
        {
            0,
            3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
            3,  3,  3,  3,  3,  3,  4,  4,  4,  4,
            4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
            4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
            4,  5,  5,  5,  5,  5,  5,  5,  5,  5,
            5,  5,  5,  5,  5,  5,  5,  5,  5,  5,
            5,  6,  6,  6,  6,  6,  6,  6,  6,  6,
            6,  6,  6,  6,  6,  6,  6,  6,  7,  7,
            7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
            7,  7,  7,  8,  8,  8,  8,  8,  8,  8,
            8,  9,  9,  9,  9,  10, 10, 10, 10, 11,
        };

        private static readonly int[] coverDegrees =
        {
            0,
            70, 71, 71, 72, 73, 74, 74, 75, 76, 77,
            77, 78, 79, 80, 80, 81, 82, 83, 84, 85,
            85, 86, 87, 88, 89, 90, 91, 92, 92, 93,
            94, 95, 96, 97, 98, 99, 100,    101,    102,    103,
            104,    105,    106,    107,    108,    110,    111,    112,    113,    114,
            115,    116,    117,    119,    120,    121,    122,    123,    125,    126,
            127,    128,    130,    131,    132,    134,    135,    136,    138,    139,
            140,    142,    143,    145,    146,    148,    149,    151,    152,    154,
            155,    157,    158,    160,    161,    163,    165,    166,    168,    170,
            171,    173,    175,    177,    178,    180,    182,    184,    186,    187,
            193,    199,    205,    211,    217,    224,    231,    237,    245,    252,
        };

        private static readonly float[] recoveryTime =
        {
            0f,
            40f,    39.6f,  39.2f,  38.81f, 38.42f, 38.04f, 37.66f, 37.28f, 36.91f, 36.54f,
            36.18f, 35.81f, 35.46f, 35.1f,  34.75f, 34.4f,  34.06f, 33.72f, 33.38f, 33.05f,
            32.72f, 32.39f, 32.07f, 31.74f, 31.43f, 31.11f, 30.8f,  30.49f, 30.19f, 29.89f,
            29.59f, 29.29f, 29f,    28.71f, 28.42f, 28.14f, 27.86f, 27.58f, 27.3f,  27.03f,
            26.76f, 26.49f, 26.23f, 25.96f, 25.7f,  25.45f, 25.19f, 24.94f, 24.69f, 24.44f,
            24.2f,  23.96f, 23.72f, 23.48f, 23.25f, 23.01f, 22.78f, 22.56f, 22.33f, 22.11f,
            21.89f, 21.67f, 21.45f, 21.24f, 21.02f, 20.81f, 20.61f, 20.4f,  20.2f,  19.99f,
            19.79f, 19.6f,  19.4f,  19.21f, 19.01f, 18.82f, 18.64f, 18.45f, 18.26f, 18.08f,
            17.9f,  17.72f, 17.54f, 17.37f, 17.2f,  17.02f, 16.85f, 16.68f, 16.52f, 16.35f,
            16.19f, 16.03f, 15.87f, 15.71f, 15.55f, 15.4f,  15.24f, 15.09f, 14.94f, 14.79f,
            14.35f, 13.92f, 13.5f,  13.09f, 12.7f,  12.32f, 11.95f, 11.59f, 11.24f, 10.91f,
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


    }



}