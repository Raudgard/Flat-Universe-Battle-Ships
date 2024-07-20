namespace MODULES
{
    public class Asceticism_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.ASCETICISM_MODULE;
        }

        protected override void Start()
        {
            base.Start();
            ship.USPNeedToReproduction = ModuleData[LevelOfModule];
            ship.USP_taken += USPTaken;
        }

        private new static readonly int[] ModuleData =
        {
            16,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 12, 12, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 12, 11, 11,
            11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            10, 10, 10, 10, 10, 10, 10, 10, 10, 9,

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


        private void USPTaken()
        {
            if (UltimateImpactAction())
            {
                ship.USPTakens++;
            }
        }


    }
}