using System.Linq;
namespace MODULES
{
    public class Supercargo_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.SUPERCARGO_MODULE;
            //ship = GetComponent<Ship>();

        }


        protected override void Start()
        {
            base.Start();
            if(ship.IsOriginal)
            {
                Global_Controller.Instance.shipsMaxCapacity[ship.playerNumber] += ModuleData[levelOfModule];
            }
            else
            {
                if (UltimateImpactAction())
                {
                    UnityEngine.Debug.Log($"Supercargo ULTIMATE! playerNumber: {ship.playerNumber}");
                    Global_Controller.Instance.shipsMaxCapacity[ship.playerNumber] += 1;
                    //пересчитываем полоску мощности.
                    //Global_Controller.Instance.powerBars.Single(v => v.key == ship.playerNumber).value.CalculateAndShowPower(EVENT_TYPE.SHIPS_COUNT_CHANGED, null, null);
                    Global_Controller.Instance.powerBars.Single(v => v.key == ship.playerNumber).value.CalculateAndShowPower(EVENT_TYPE.SHIP_CREATED, null, null);

                }
            }
        }

        public static new int[] ModuleData =
        {
        0,
        1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
        1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
        1,  1,  1,  1,  1,  1,  1,  1,  2,  2,
        2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
        2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
        2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
        2,  2,  3,  3,  3,  3,  3,  3,  3,  3,
        3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
        3,  3,  3,  3,  3,  4,  4,  4,  4,  4,
        4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
        5,  5,  5,  5,  5,  6,  6,  6,  6,  7,

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