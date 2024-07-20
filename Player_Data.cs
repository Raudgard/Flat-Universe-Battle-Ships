using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class Player_Data : MonoBehaviour
{
    #region singleton

    public static Player_Data Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }
    #endregion

    public Sprite[] ships_Skins;

    //public RuntimeAnimatorController ship1_animator_controller;

    public DifficultyAI chosenDifficulty = DifficultyAI.HARD;

    public Ship[] playerShips;//here saves all player's ships
    public Ship[] enemyShips;

    public int NumberOfShips { get; set; }
    public string[] NamesOfShips;

    //public float StrengthOfTouch = 20.0f;
    //public float partOfDeadShipSpeed = 20f; //скорость разлетания частей убитого микрота
    //public float partOfDeadShipSpeedReduction = 0.75f; // коэффициент снижения скорости разлетающихся частей

    public int USPCount = 700;
    public int USPCountInBattle = 300;
    //public int USPCountInMaze;
    public int howMuchUSPPlayerCouldTakeIntoBattle = 300;

    public int USPCap;
    public float USPEnlargingSpeed; // USP per 1 second
    public float enlargingUSPspeedCoeff;
    public int startingQuantityUSPInBattle;

    public int productionUSPPerSecond = 1;//количество еды произведенной в секунду

    //public Ship[] shipsDontDestroy; //копии микротов для битвы
    //public Ship shipForMaze; // копия микрота для лабиринта
    public HealProjectile healProjectile;


    public static float cameraOrthographicSize = 9.6f; // стандартное значение для размера "портрет 16:9"
    public bool isAutoUSPCreationON = false;
    public bool showFPS;


    public int levelOfMaze = 1;

    [Header("If Cross, min capsule Length = 6 and count of points = 90")]
    public ShapeOfBattlefield shapeOfBattlefield = ShapeOfBattlefield.Circle;
    public float sizeOfBattleField;

    [Tooltip("If Cross, min capsule Length = 6 and count of points = 90")]
    public float capsuleLength;
    public int CountOfPointForBorders;

    #region DevelopMode
    public bool canAICreateUSP = true;
    public bool fogOfWar = true;
    public float rateOfCreationUSPAI = 0.4f;
    public bool isDevelopMode = false;
    //public Ship[] AIshipDontDestroy;
    public int USPCountAI;

    //public static int screenHeight;
    //public static int screenWidth;
    //public static float ratio;
    //public static float needHeight;
    //public static float ortSize;
    //public static float currentOrtSize;
    #endregion

    

    private void Start()
    {
        USPProduction(productionUSPPerSecond);
        //ResizeCamera();
    }

    /// <summary>
    /// Асинхронный метод, пассивно прибавляющий игроку еду при включенной игре.
    /// </summary>
    /// <param name="productionUSPPerSecond"></param>
    private async void USPProduction(int productionUSPPerSecond)
    {
        while(this != null && gameObject != null)
        {
            USPCount += productionUSPPerSecond;
            //print("+1 to USP!");
            await Task.Delay(1000);
        }
    }

    

    public static void ResizeCamera()
    {
        //screenHeight = Screen.height;
        //screenWidth = Screen.width;
        //print("worked");
        float ratio = (float)Screen.height / Screen.width;
        //print("ratio" + ratio);

        float needHeight = 5.4f * ratio;
        //ortSize = needHeight / 200f;
        //print("needHeight = " + needHeight);
        cameraOrthographicSize = needHeight;

        Camera.main.orthographicSize = needHeight;
        //currentOrtSize = Camera.main.orthographicSize;
        //print("currentOrtSIze = " + currentOrtSize);

    }
}
