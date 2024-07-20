
namespace MODULES
{
    public class Health_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.HEALTH_MODULE;
            ship = GetComponent<Ship>();

        }


        protected override void Start()
        {
            base.Start();

            //Если у корабля-родителя максимальное количество ХП больше, чем значение модуля (например, было прибавлено другими модулями),
            //то присваиваем бОльшее значение.

            // !!!  Думаю, это неверный подход. Каждый модуль баффает только свой корабль. Иначе такая наследственность принесет неконтролируемую сверхмощь !!!

            //ship.healthMax = ship.healthMax > ModuleData[LevelOfModule] ? ship.healthMax : ModuleData[levelOfModule];

            //бафф от ультимейта запрашивается только у копий.
            int buff = ship.IsOriginal ? 0 : GetUltimateBuff();

            ship.healthMax = ModuleData[LevelOfModule] + buff;
            ship.HealthCurrent = ship.healthMax;

            //ship.healthBar.fillAmount = (float)ship.HealthCurrent / ship.healthMax;

        }

        public static new int[] ModuleData =
        {
        100,
        101,    102,    103,    104,    105,    106,    107,    108,    109,    110,
        112,    113,    114,    115,    116,    117,    118,    120,    121,    122,
        123,    124,    126,    127,    128,    130,    131,    132,    133,    135,
        136,    137,    139,    140,    142,    143,    145,    146,    147,    149,
        150,    152,    153,    155,    156,    158,    160,    161,    163,    164,
        166,    168,    169,    171,    173,    175,    176,    178,    180,    182,
        183,    185,    187,    189,    191,    193,    195,    197,    199,    201,
        203,    205,    207,    209,    211,    213,    215,    217,    219,    222,
        224,    226,    228,    231,    233,    235,    238,    240,    242,    245,
        247,    250,    252,    255,    257,    260,    263,    265,    268,    270,
        279,    287,    296,    304,    314,    323,    333,    343,    353,    364
        };

        public static int[] UltimateBuff =
        {
        50,
        51, 51, 52, 52, 53, 53, 54, 54, 55, 55,
        56, 56, 57, 57, 58, 59, 59, 60, 60, 61,
        62, 62, 63, 63, 64, 65, 65, 66, 67, 67,
        68, 69, 69, 70, 71, 72, 72, 73, 74, 74,
        75, 76, 77, 77, 78, 79, 80, 81, 81, 82,
        83, 84, 85, 86, 86, 87, 88, 89, 90, 91,
        92, 93, 94, 95, 95, 96, 97, 98, 99, 100,
        101,    102,    103,    104,    105,    107,    108,    109,    110,    111,
        112,    113,    114,    115,    116,    118,    119,    120,    121,    122,
        124,    125,    126,    127,    129,    130,    131,    133,    134,    135,
        139,    143,    148,    152,    157,    161,    166,    171,    176,    182,
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


        private int GetUltimateBuff()
        {
            if (UltimateImpactAction())
            {
                UnityEngine.Debug.Log($"Health_Module Ultimate!!! ship: {ship.name}");
                return UltimateBuff[levelOfModule];
            }

            return 0;
        }

    }
}