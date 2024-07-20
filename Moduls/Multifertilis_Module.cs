namespace MODULES
{
    public class Multifertilis_Module : Module
    {
        private void Awake()
        {
            moduleType = Moduls.MULTIFERTILIS_MODULE;
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Общая вероятность срабатывания модуля.
        /// </summary>
        public static new float[] ModuleData =
        {
            0,
            10f,    10.12f, 10.24f, 10.36f, 10.49f, 10.62f, 10.74f, 10.87f, 11.01f, 11.14f,
11.27f, 11.41f, 11.55f, 11.69f, 11.83f, 11.98f, 12.13f, 12.27f, 12.43f, 12.58f,
12.73f, 12.89f, 13.05f, 13.21f, 13.37f, 13.54f, 13.71f, 13.88f, 14.05f, 14.23f,
14.41f, 14.59f, 14.77f, 14.95f, 15.14f, 15.33f, 15.53f, 15.72f, 15.92f, 16.12f,
16.33f, 16.53f, 16.74f, 16.96f, 17.17f, 17.39f, 17.62f, 17.84f, 18.07f, 18.3f,
18.54f, 18.78f, 19.02f, 19.27f, 19.52f, 19.77f, 20.03f, 20.29f, 20.55f, 20.82f,
21.1f,  21.37f, 21.65f, 21.94f, 22.23f, 22.52f, 22.82f, 23.12f, 23.43f, 23.74f,
24.05f, 24.37f, 24.7f,  25.03f, 25.36f, 25.7f,  26.05f, 26.4f,  26.76f, 27.12f,
27.48f, 27.86f, 28.23f, 28.62f, 29.01f, 29.4f,  29.81f, 30.21f, 30.63f, 31.05f,
31.48f, 31.91f, 32.35f, 32.8f,  33.25f, 33.71f, 34.18f, 34.66f, 35.14f, 35.63f,
36.77f, 37.95f, 39.16f, 40.41f, 41.71f, 43.04f, 44.42f, 45.85f, 47.31f, 48.83f,


        };


        /// <summary>
        /// Вероятность появления 2х кораблей.
        /// </summary>
        private static readonly float[] ProbabilityOfTwoNewShips =
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


        };



        /// <summary>
        /// Вероятность появления 3х кораблей вместо 2х.
        /// </summary>
        private static readonly float[] ProbabilityOfThreeNewShips =
        {
            0,
            2f, 2.04f,  2.08f,  2.12f,  2.16f,  2.21f,  2.25f,  2.3f,   2.34f,  2.39f,
2.44f,  2.49f,  2.54f,  2.59f,  2.64f,  2.69f,  2.75f,  2.8f,   2.86f,  2.91f,
2.97f,  3.03f,  3.09f,  3.15f,  3.22f,  3.28f,  3.35f,  3.41f,  3.48f,  3.55f,
3.62f,  3.7f,   3.77f,  3.84f,  3.92f,  4f, 4.08f,  4.16f,  4.24f,  4.33f,
4.42f,  4.5f,   4.59f,  4.69f,  4.78f,  4.88f,  4.97f,  5.07f,  5.17f,  5.28f,
5.38f,  5.49f,  5.6f,   5.71f,  5.83f,  5.94f,  6.06f,  6.18f,  6.31f,  6.43f,
6.56f,  6.69f,  6.83f,  6.96f,  7.1f,   7.25f,  7.39f,  7.54f,  7.69f,  7.84f,
8f, 8.16f,  8.32f,  8.49f,  8.66f,  8.83f,  9.01f,  9.19f,  9.37f,  9.56f,
9.75f,  9.95f,  10.14f, 10.35f, 10.55f, 10.77f, 10.98f, 11.2f,  11.42f, 11.65f,
11.89f, 12.12f, 12.37f, 12.61f, 12.87f, 13.12f, 13.39f, 13.65f, 13.93f, 14.21f,
14.7f,  15.22f, 15.75f, 16.3f,  16.87f, 17.46f, 18.07f, 18.71f, 19.36f, 20.04f,


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


        public int Multifertilis()
        {
            //сначала проверяем сработает ли вообще модуль, подставляя в метод GetProbability общую вероятность.
            //Усли модуль сработал, проверяем сработает ли та вероятность, по которой появится 3 новых корабля вместо 2.
            //Если сработала, то новых кораблей 3, если нет, то 2.
            int newShipsNumber = 1;
            if (UltimateImpactAction())
                newShipsNumber++;

            if (GameEngineAssistant.GetProbability(ProbabilityOfTwoNewShips[LevelOfModule]))
            {
                print("Multifertilis! 2 new ships");
                newShipsNumber++;
                
            }

            if (GameEngineAssistant.GetProbability(ProbabilityOfThreeNewShips[LevelOfModule]))
            {
                print("Multifertilis! 3 new ships");
                newShipsNumber++;
                newShipsNumber++;

            }

            return newShipsNumber;
        }


        

        










        
    }
}