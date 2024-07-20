using UnityEngine;
using System;
using System.Linq;


namespace MODULES
{
    public enum Moduls
    {
        ANCILE_MODULE,              //Создает щит Ancile, вращающийся вокруг корабля.
        ARMOR_MODULE,
        ASCETICISM_MODULE,          //уменьшение необходимого количества еды, необходимого для размножения
        ATTACK_MODULE,
        ATTACK_RANGE_MODULE,
        ATTACK_SPEED_MODULE,
        BASH_MODULE,                //вероятность оглушить врага при атаке
        COOKSHOP_MODULE,            //"ХАРЧЕВНЯ" - время от времени производит USP
        DEATHEATING_MODULE,         //пожирающий смерть - хилится, если его удар был фатальным для врага
        EVASION_MODULE,             //модуль уклонения  
        GIANT_MODULE,
        HEALTH_MODULE,
        MEDICUS_MODULE,             //лечит своих. С некоторого уровня имеет шанс снять дебафф (болезнь и пр.)
        MOVEMENT_SPEED_MODULE,
        MULTIFERTILIS_MODULE,       //вероятность появления более одного микрота при размножении
        PATRIOT_MODULE,             //увеличивает attack_damage, если в пределах видимости погиб "свой".
        PURPOSEFULNESS_MODULE,      //целеустреммленность - ускоряется каждую единицу времени, если движется в том же направлении
        //PHANTOM_MODULE,             //создает фантомов
        RABBIT_MODULE,              //уменьшение времени размножения
        REGENERATION_MODULE,
        STEALTH_MODULE,             //делает корабль невидимым
        SUPERCARGO_MODULE,          //увеличивает максимально возможное для игрока количество кораблей
        TELEKINESIS_MODULE,         //притягивает еду, к которой сейчас двигается
        TELEPORTATION_MODULE,       //телепортируется время от времени по направлению движения
        USP_POWER_MODULE,           //заряжает снаряды силой USP (увеличивает урон), но отнимает USP.
        VISION_MODULE,              //зоркость, дальность обзора

    }

    public class Module : MonoBehaviour
    {
        protected Ship ship;
        public Moduls moduleType;
        [SerializeField] protected int levelOfModule;

        public int energy = 0;

        protected static int[] ModuleData;// = new int[111]; //максимальное количество уровней генов (проследить за правильностью установки уровней) 

        //в классах модулей, наследующих этот класс, поле LevelOfModule должно быть override, а не new, иначе все ссылки будут 
        //будут идти на это ПУСТОЕ поле, так как классы-наследники будут иметь свои поля с тем же названием.
        public virtual int LevelOfModule { get; set; }
        //public static int GetMaxLevel { get; }

        protected float ChanceToUltimate { get; set; }

        /// <summary>
        /// Возвращает тип модуля по названию.
        /// </summary>
        /// <param name="nameOfModule">Название модуля без namespace MODULS.</param>
        /// <returns></returns>
        public static Type GetTypeOfModule(string nameOfModule)
        {
            return Type.GetType("MODULES." + nameOfModule, true, true);
        }


        /// <summary>
        /// Мощность данного модуля.
        /// </summary>
        public int Power => ModulePowers[LevelOfModule];


        /// <summary>
        /// Мощность модуля по уровням.
        /// </summary>
        public static readonly int[] ModulePowers =
        {
            0,
            1000000,    1010000,    1020100,    1030301,    1040604,    1051010,    1061520,    1072135,    1082857,    1093685,
            1104622,    1115668,    1126825,    1138093,    1149474,    1160969,    1172579,    1184304,    1196147,    1208109,
            1220190,    1232392,    1244716,    1257163,    1269735,    1282432,    1295256,    1308209,    1321291,    1334504,
            1347849,    1361327,    1374941,    1388690,    1402577,    1416603,    1430769,    1445076,    1459527,    1474123,
            1488864,    1503752,    1518790,    1533978,    1549318,    1564811,    1580459,    1596263,    1612226,    1628348,
            1644632,    1661078,    1677689,    1694466,    1711410,    1728525,    1745810,    1763268,    1780901,    1798710,
            1816697,    1834864,    1853212,    1871744,    1890462,    1909366,    1928460,    1947745,    1967222,    1986894,
            2006763,    2026831,    2047099,    2067570,    2088246,    2109128,    2130220,    2151522,    2173037,    2194768,
            2216715,    2238882,    2261271,    2283884,    2306723,    2329790,    2353088,    2376619,    2400385,    2424389,
            2448633,    2473119,    2497850,    2522829,    2548057,    2573538,    2599273,    2625266,    2651518,    2678033,
            2758374,    2841126,    2926360,    3014150,    3104575,    3197712,    3293643,    3392453,    3494226,    3599053,

        };


        /// <summary>
        /// Энергия, необходимая для запитки модуля, по уровням.
        /// </summary>
        public static readonly int[] ModuleNeededEnergy =
        {
            0,
            100,    105,    110,    116,    122,    128,    134,    141,    148,    155,
            163,    171,    180,    189,    198,    208,    218,    229,    241,    253,
            265,    279,    293,    307,    323,    339,    356,    373,    392,    412,
            432,    454,    476,    500,    525,    552,    579,    608,    639,    670,
            704,    739,    776,    815,    856,    899,    943,    991,    1040,   1092,
            1147,   1204,   1264,   1327,   1394,   1464,   1537,   1614,   1694,   1779,
            1868,   1961,   2059,   2162,   2270,   2384,   2503,   2628,   2760,   2898,
            3043,   3195,   3355,   3522,   3698,   3883,   4077,   4281,   4495,   4720,
            4956,   5204,   5464,   5737,   6024,   6325,   6642,   6974,   7322,   7689,
            8073,   8477,   8901,   9346,   9813,   10303,  10819,  11360,  11928,  12524,
            14403,  16563,  19047,  21904,  25190,  28969,  33314,  38311,  44058,  50665
        };


        public delegate bool UltimateImpact();

        /// <summary>
        /// Делегат проверки на срабатывание ультимейта.
        /// </summary>
        public UltimateImpact UltimateImpactAction { get; private set; }


        protected virtual void Start()
        {
            ship = GetComponent<Ship>();

            //Сделал через делегат, чтобы каждый раз не проверять уровень модуля. Иначе слишком много проверок получается.
            if (LevelOfModule >= References.Instance.settings.moduleUltimateLevel)
            {
                ChanceToUltimate = References.Instance.settings.ultimateChances.Single(uc => uc.module == moduleType).chance;
                //Debug.Log($"moduleType: {moduleType}, ChanceToUltimate new: {ChanceToUltimate}");
                UltimateImpactAction = UltimatePercentage;
            }
            else
            {
                UltimateImpactAction = delegate { return false; };
            }

        }



        private bool UltimatePercentage()
        {
            //Debug.Log($"ChanceToUltimate: {ChanceToUltimate}");

            if (GameEngineAssistant.GetProbability(ChanceToUltimate))
            {
                //Debug.Log($"ULTIMATE! Module: {moduleType}");
                //Global_Controller.Instance.StartCoroutine(Global_Controller.Instance.TextVisualization("Ultimate!", ship.transform.position, new Vector2(0, 0.5f), color, 0.0f));
                return true;
            }

            return false;
        }




    }
}