using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class BattleSceneUIController : MonoBehaviour
{
    public Canvas mainCanvas;
    public TextMeshProUGUI USPCountAI;
    public TextMeshProUGUI USPCountPlayer;

    [SerializeField] private Image WinOrLosePanel;
    [SerializeField] private Image MenuPanel;
    [SerializeField] private Image ConfirmationPanel;

    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI FPS;
    [SerializeField] private float FPS_ShowDelay;
    [SerializeField] private TextMeshProUGUI numberOfAllShips;


    //private AI AIComponent;
    //private Battle_Scene_Controller battle_Scene_Controller;
    private Global_Controller global_Controller;
    private USPEnlarger enlargerUSPForPlayer;
    private USPEnlarger enlargerUSPForAI;

    public Button backToShips_Button;
    private Image topMenuPanel;// то меню, которое в данный момент находится "выше" остальных.

    private void Awake()
    {
        //AIComponent = GetComponent<AI>();
        //battle_Scene_Controller = GetComponent<Battle_Scene_Controller>();
        global_Controller = Global_Controller.Instance;

        
    }
    void Start()
    {
        Player_Data.ResizeCamera();

        MenuPanel.gameObject.SetActive(false);
        ConfirmationPanel.gameObject.SetActive(false);
        FPS.gameObject.SetActive(Player_Data.Instance.showFPS);
        if (Player_Data.Instance.showFPS) StartCoroutine(ShowFPS());

        enlargerUSPForPlayer = GameObject.FindGameObjectWithTag("PlayerUSPEnlarger").GetComponent<USPEnlarger>();
        enlargerUSPForAI = GameObject.FindGameObjectWithTag("AI_USPEnlarger").GetComponent<USPEnlarger>();

        USPForPlayer();

        //player_create_USP = Time.time;
        //AI_create_USP = Time.time;

        numberOfAllShips.text = "" + global_Controller.allShips.Count;

        //USPCountChanged(EVENT_TYPE.SHIPS_COUNT_CHANGED, null, null);
        USPCountChanged(EVENT_TYPE.USP_ENLARGED, null, null);


        EventManager.Instance.AddListener(EVENT_TYPE.SHIP_DESTROYED, ShipsCountChanged);
        //EventManager.Instance.AddListener(EVENT_TYPE.SHIPS_COUNT_CHANGED, ShipsCountChanged);
        EventManager.Instance.AddListener(EVENT_TYPE.SHIP_CREATED, ShipsCountChanged);

        EventManager.Instance.AddListener(EVENT_TYPE.USP_CREATED, USPCountChanged);
        EventManager.Instance.AddListener(EVENT_TYPE.USP_ENLARGED, USPCountChanged);

        StartCoroutine(CheckCameraPosition());
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (MenuPanel.gameObject.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        timer.text = Timer(Time.timeSinceLevelLoad);
    }

    private IEnumerator ShowFPS()
    {
        while(true)
        {
            if (FPS.isActiveAndEnabled) FPS.text = "" + (int)(1 / Time.deltaTime);
            yield return new WaitForSeconds(FPS_ShowDelay);
        }
    }

    private void ShipsCountChanged(EVENT_TYPE type, Component component, object obj)
    {
        numberOfAllShips.text = "" + global_Controller.allShips.Count;

    }

    private void USPCountChanged(EVENT_TYPE type, Component component, object obj)
    {
        USPCountAI.text = "" + enlargerUSPForAI.CurrentQuantity;
        USPCountPlayer.text = "" + enlargerUSPForPlayer.CurrentQuantity;
    }

    private string Timer(float timeSinceLevelLoad)
    {
        int minutes = (int)(timeSinceLevelLoad / 60), seconds = (int)(timeSinceLevelLoad % 60);
        if (seconds < 10)
            return $"{minutes}:0{seconds} ";
        else
            return $"{minutes}:{seconds} ";
    }

    public void ShowWinOrLosePanel(int arg) // 1 - player win, 2 - player lose.
    {
        if(WinOrLosePanel.gameObject != null)
            WinOrLosePanel.gameObject.SetActive(true);

        global_Controller.isMenuOpened = true;
        TextMeshProUGUI textObj = WinOrLosePanel.GetComponentInChildren<TextMeshProUGUI>();

        if (arg == 1)
        {
            textObj.text = "VICTORY!";
            textObj.color = new Color(0, 0.7f, 0.1f);
        }

        if (arg == 2)
        {
            textObj.text = "DEFEAT!";
            textObj.color = new Color(0.8f, 0f, 0f);
        }
    }

    //public void CalculateApM(int whoCreate) // 0 - player, 1 - AI
    //{

    //    if (whoCreate == 0)
    //    {
    //        float Apm = 60 / (Time.time - player_create_USP);
    //        ApM_player_text = "" + Apm;
    //        player_create_USP = Time.time;
    //    }
    //    else
    //    {
    //        float Apm = 60 / (Time.time - AI_create_USP);
    //        ApM_AI_text = "" + Apm;
    //        AI_create_USP = Time.time;
    //    }
    //}

    private void USPForPlayer()
    {
        Player_Data.Instance.USPCountInBattle = Player_Data.Instance.howMuchUSPPlayerCouldTakeIntoBattle;
        if (Player_Data.Instance.USPCount < Player_Data.Instance.USPCountInBattle)
        {
            Player_Data.Instance.USPCountInBattle = Player_Data.Instance.USPCount;
            Player_Data.Instance.USPCount = 0;
        }
        else
        {
            Player_Data.Instance.USPCount -= Player_Data.Instance.USPCountInBattle;
        }
    }

    public void ContinuePlaying()
    {
        WinOrLosePanel.gameObject.SetActive(false);
        Global_Controller.Instance.isMenuOpened = false;
        //CheckForOpenedMenu();
    }

    public void QuitLevel()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.QUIT_LEVEL, this);
        Global_Controller.Instance.IsGamePaused = false;

        global_Controller.Clear();
        StartCoroutine(LoadMainSceneAsync());
    }

    private IEnumerator LoadMainSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        //Ship[] ships = Player_Data.Instance.shipsDontDestroy;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //battle_Scene_Controller.freeUSPs.Clear();
        SceneManager.UnloadSceneAsync(thisScene);
    }

    public void OpenMenu()
    {
        MenuPanel.gameObject.SetActive(true);
        topMenuPanel = MenuPanel;
        Global_Controller.Instance.IsGamePaused = true;
        global_Controller.isMenuOpened = true;
    }

    public void CloseMenu()
    {
        topMenuPanel.gameObject.SetActive(false);
        topMenuPanel = MenuPanel;
        if (!MenuPanel.gameObject.activeSelf && !ConfirmationPanel.gameObject.activeSelf)
        {
            StartCoroutine(DelayBeforeUnpausedGame());
            Global_Controller.Instance.isMenuOpened = false;
        }

        //CheckForOpenedMenu();
    }

    public void ClickOnQuitButton()
    {
        ConfirmationPanel.gameObject.SetActive(true);
        topMenuPanel = ConfirmationPanel;
        global_Controller.isMenuOpened = true;
    }

    public void ClickOnBackButtonInConfirmationPanel()
    {
        ConfirmationPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Необходима, чтобы при отжатии клавиши Back в меню не создавался USP
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayBeforeUnpausedGame()
    {
        yield return null;
        Global_Controller.Instance.IsGamePaused = false;
    }

    /// <summary>
    /// Проверяет есть ли в поле зрения основной камеры хоть один из своих кораблей. 
    /// Если нет, то активирует кнопку перемещения камеры к первому в списке кораблю.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckCameraPosition()
    {
        yield return null;
        yield return null;

        while(true)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            var ships = global_Controller.ships[1];
            Collider2D[] colliders = new Collider2D[ships.Count];
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = ships[i].GetComponent<Collider2D>();
            }

            bool isSomeShipVisible = true;
            for(int i = 0; i < colliders.Length; i++)
            {
                if (GeometryUtility.TestPlanesAABB(planes, colliders[i].bounds))
                {
                    //print("Some ship is visible");
                    backToShips_Button.gameObject.SetActive(false); 
                    break;
                }

                if (i == colliders.Length - 1) isSomeShipVisible = false;

            }

            if(!isSomeShipVisible)
            {
                backToShips_Button.gameObject.SetActive(true);
            }

            //if (((Vector2)Camera.main.transform.position - Vector2.zero).sqrMagnitude > distanceSqrCameraFromCenterToAppearButton)
            //{
            //    backToShips_Button.gameObject.SetActive(true);
            //}

            yield return new WaitForSeconds(3);
        }
    }

    public void BackToShips_Click()
    {
        if (global_Controller.ships[1].Count > 0)
            Camera.main.transform.position = new Vector3(global_Controller.ships[1][0].transform.position.x, global_Controller.ships[1][0].transform.position.y, -10);
        else
            Camera.main.transform.position = new Vector3(0, 0, -10);

        backToShips_Button.gameObject.SetActive(false);

        Global_Controller.Instance.isUIElementPressed = true;
        //Global_Controller.Instance.DelayBefore(() => Global_Controller.Instance.isUIElementPressed = false, 1);
        Tools.UnityTools.ExecuteWithDelay(() => Global_Controller.Instance.isUIElementPressed = false, 1);

    }


}
