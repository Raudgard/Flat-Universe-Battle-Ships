using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MazeSceneController : MonoBehaviour
{
    //[SerializeField] Ship shipPrefab;
    public USP USPPrefab;
    public PreUSP preUSP;//для CookShop_Module
    public SpriteRenderer backlight_enemy_circle;
    public SpriteRenderer backlight_enemy_fivePointedStar;
    public SpriteRenderer backlight_enemy_fourPointedStar;
    public SpriteRenderer backlight_enemy_heart;
    public SpriteRenderer backlight_enemy_pentagon;
    public SpriteRenderer backlight_enemy_square;
    public SpriteRenderer backlight_enemy_triangle;

    [SerializeField] public List<USP> freeUSPs = new List<USP>();

    //public HealProjectile healProjectile;
    public BackGround_Lights BackGround_Lights_prefab;

    private Global_Controller global_Data;
    public Camera mainCamera;

    //private MazeScene_UI_Controller scene_UI_Controller;
     
    private GameObject fogOfWar;
    public GameObject[] DeadShips;
    public ClickControllerForMaze clickController;

    public Ship shipMain;

    

    private void Awake()
    {
        global_Data = Global_Controller.Instance;
        //scene_UI_Controller = GetComponent<MazeScene_UI_Controller>();

        EventManager.Instance.AddListener(EVENT_TYPE.USP_TAKEN, RemoveUSPFromShip);

    }

    private void Start()
    {
        Player_Data.Instance.isAutoUSPCreationON = false;

        CreateCopyShipForMaze();
        StartCoroutine(GetListOfShips());
        //StartCoroutine(CreatingBackgroundLights());


        //убираем туман войны в случае, если в настройках игрока он убран (пока только для Develop Mode)
        fogOfWar = GameObject.FindGameObjectWithTag("FogOfWar");
        if (!Player_Data.Instance.fogOfWar)
            fogOfWar.SetActive(false);

        ApplyToClickControllerBoxColliderNewSize();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < global_Data.allShips.Count; i++)
            if (global_Data.allShips[i] != null)
                global_Data.allShips[i].FixUpdateMe();
    }

    private void Update()
    {
        Camera.main.transform.position = new Vector3(shipMain.transform.position.x, shipMain.transform.position.y, -10);

    }

    private void CreateCopyShipForMaze()
    {
        //ПЕРЕРАБОТАТЬ!!!

        //GameObject[] shipsForBattle = new GameObject[Player_Data.Instance.shipsDontDestroy.Length];
        //GameObject shipForMaze = Instantiate(Player_Data.Instance.shipForMaze.gameObject) as GameObject;
        //shipForMaze.GetComponentInChildren<Canvas>().enabled = true;
        //Vector3 _position = GetComponent<MazeCreating>().startZone.transform.position;
        //shipForMaze.transform.position = _position;

        ////Ship ship = shipsForBattle[i].GetComponent<Ship>();
        //StartCoroutine(ApplyNewStrengthOfTouch(shipForMaze.GetComponent<Ship>()));
        //shipMain = shipForMaze.GetComponent<Ship>();




        //foreach (Ship ship in Player_Data.Instance.shipsDontDestroy)
        //{
        //    ship.gameObject.SetActive(false);
        //}

        //Camera.main.transform.position = new Vector3(_position.x, _position.y, -10);
    }

    /// <summary>
    /// Получаем изначальный список микротов, находящихся в уровне
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetListOfShips()
    {
        yield return null;

        Ship[] shipsTMP = FindObjectsOfType<Ship>();
        foreach (Ship ship in shipsTMP)
        {
            global_Data.AddShip(ship);
        }
    }

    private IEnumerator CreatingBackgroundLights()
    {
        int _numberOfBackgroundLigths = 0;
        while (true)
        {
            BackGround_Lights backgoundLigth = Instantiate(BackGround_Lights_prefab) as BackGround_Lights;
            _numberOfBackgroundLigths++;

            if (_numberOfBackgroundLigths > 9)
                break;
            else
                yield return new WaitForSeconds(10);
        }
    }

    private IEnumerator ApplyNewStrengthOfTouch(Ship ship)
    {
        yield return null;

        ship.SensivityRadiusSqr = 1;
    }

    public void ApplyToClickControllerBoxColliderNewSize()
    {
        BoxCollider2D boxCollider= clickController.GetComponent<BoxCollider2D>();
        MazeCreating mazeCreating = GetComponent<MazeCreating>();
        boxCollider.size = new Vector2(mazeCreating.widthOfNewMaze, mazeCreating.heightOfNewMaze);
        boxCollider.offset = new Vector2((float)mazeCreating.widthOfNewMaze / 2, (float)mazeCreating.heightOfNewMaze / 2);


    }

    private void RemoveUSPFromShip(EVENT_TYPE eVENT_TYPE, Component component, object obj)
    {
        shipMain.USPTakens = 0;
        //shipMain.fillingWithUSPForm.fillAmount = 0;
    }

    

    
}
