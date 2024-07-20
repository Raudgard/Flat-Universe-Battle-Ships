using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Purposefulness_Module : Module
    {
        public Vector2 _direction = Vector2.zero;
        public Vector2 attackDirection = Vector2.zero;
        public float interval = 0.2f; //интервал между проверками на ускорение
        private float originalMoveSpeed;
        private Rigidbody2D rb;
        private WaitForFixedUpdate fixUpdate;

        private void Awake()
        {
            moduleType = Moduls.PURPOSEFULNESS_MODULE;
            fixUpdate = new WaitForFixedUpdate();
        }

        protected override void Start()
        {
            base.Start();
            rb = ship.GetComponent<Rigidbody2D>();
            //ship.StateChanged += SetMoveSpeedToDefault;
            StartCoroutine(SaveOriginalMoveSpeed());//задержка в 1 кадр нужна, чтобы модуль Movement_speed успел применить свой бафф.

            StartCoroutine(Accelaration());

        }

        private new static readonly float[] ModuleData =
        {
            0,
            0.003f, 0.00303f,   0.0030603f, 0.0030909f, 0.0031218f, 0.003153f,  0.0031846f, 0.0032164f, 0.0032486f, 0.0032811f,
            0.0033139f, 0.003347f,  0.0033805f, 0.0034143f, 0.0034484f, 0.0034829f, 0.0035177f, 0.0035529f, 0.0035884f, 0.0036243f,
            0.0036606f, 0.0036972f, 0.0037341f, 0.0037715f, 0.0038092f, 0.0038473f, 0.0038858f, 0.0039246f, 0.0039639f, 0.0040035f,
            0.0040435f, 0.004084f,  0.0041248f, 0.0041661f, 0.0042077f, 0.0042498f, 0.0042923f, 0.0043352f, 0.0043786f, 0.0044224f,
            0.0044666f, 0.0045113f, 0.0045564f, 0.0046019f, 0.004648f,  0.0046944f, 0.0047414f, 0.0047888f, 0.0048367f, 0.004885f,
            0.0049339f, 0.0049832f, 0.0050331f, 0.0050834f, 0.0051342f, 0.0051856f, 0.0052374f, 0.0052898f, 0.0053427f, 0.0053961f,
            0.0054501f, 0.0055046f, 0.0055596f, 0.0056152f, 0.0056714f, 0.0057281f, 0.0057854f, 0.0058432f, 0.0059017f, 0.0059607f,
            0.0060203f, 0.0060805f, 0.0061413f, 0.0062027f, 0.0062647f, 0.0063274f, 0.0063907f, 0.0064546f, 0.0065191f, 0.0065843f,
            0.0066501f, 0.0067166f, 0.0067838f, 0.0068517f, 0.0069202f, 0.0069894f, 0.0070593f, 0.0071299f, 0.0072012f, 0.0072732f,
            0.0073459f, 0.0074194f, 0.0074936f, 0.0075685f, 0.0076442f, 0.0077206f, 0.0077978f, 0.0078758f, 0.0079546f, 0.0080341f,
            0.0082751f, 0.0085234f, 0.0087791f, 0.0090425f, 0.0093137f, 0.0095931f, 0.0098809f, 0.0101774f, 0.0104827f, 0.0107972f

        };

        private static readonly int[] max_speed =
        {
            20,
            100,    101,    102,    103,    104,    105,    106,    107,    108,    109,
            110,    112,    113,    114,    115,    116,    117,    118,    120,    121,
            122,    123,    124,    126,    127,    128,    130,    131,    132,    133,
            135,    136,    137,    139,    140,    142,    143,    145,    146,    147,
            149,    150,    152,    153,    155,    156,    158,    160,    161,    163,
            164,    166,    168,    169,    171,    173,    175,    176,    178,    180,
            182,    183,    185,    187,    189,    191,    193,    195,    197,    199,
            201,    203,    205,    207,    209,    211,    213,    215,    217,    219,
            222,    224,    226,    228,    231,    233,    235,    238,    240,    242,
            245,    247,    250,    252,    255,    257,    260,    263,    265,    268,
            276,    284,    293,    301,    310,    320,    329,    339,    349,    360
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


        private IEnumerator SaveOriginalMoveSpeed()
        {
            yield return null;
            originalMoveSpeed = ship.move_speed;
        }

        private IEnumerator DecreaseMoveSpeedToDefault()
        {
            while(ship.move_speed > originalMoveSpeed)
            {
                ship.move_speed *= ship.deseleration_coeff;
                yield return fixUpdate;
            }
            ship.move_speed = originalMoveSpeed;
        }


        private IEnumerator Accelaration()
        {
            yield return null;//нужна задержка в 1 кадр, иначе сразу же срабатывает else -> назначается move_speed = 0 и тут же он же сохраняется в originalMoveSpeed
            while (true)
            {
                if (ship.State != Ship.States.IDLE && !ship.movingFromForce && Vector2.Angle(_direction, rb.velocity) < 5 && rb.velocity.sqrMagnitude > 0.001f)
                {
                    //print("_direction = " + _direction + "   ship.moveDirection = " + ship.moveDirection);
                    if (ship.move_speed < max_speed[LevelOfModule])
                    {
                        ship.move_speed += originalMoveSpeed * ModuleData[LevelOfModule];
                    }
                }
                else
                {
                   yield return StartCoroutine(DecreaseMoveSpeedToDefault());
                }

                _direction = rb.velocity;
                yield return fixUpdate;
            }
        }

    }
}