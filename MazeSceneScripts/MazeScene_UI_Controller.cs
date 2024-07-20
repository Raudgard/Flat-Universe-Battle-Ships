using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MazeScene_UI_Controller : MonoBehaviour
{
    public TextMeshProUGUI USPCountPlayer;


    [SerializeField] private Image WinOrLosePanel;
    [SerializeField] private Image MenuPanel;
    [SerializeField] private Image ConfirmationPanel;

    private Global_Controller global_Data;

    private void Awake()
    {
        global_Data = Global_Controller.Instance;
    }


    void Start()
    {
        Player_Data.ResizeCamera();

        MenuPanel.gameObject.SetActive(false);
        ConfirmationPanel.gameObject.SetActive(false);
        CheckForOpenedMenu();

        USPForPlayer();
    }


    private void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (MenuPanel.gameObject.activeSelf)
            {
                if (ConfirmationPanel.gameObject.activeSelf == true)
                {
                    ConfirmationPanel.gameObject.SetActive(false);
                    return;
                }

                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        USPCountPlayer.text = "" + Player_Data.Instance.USPCountInBattle;

    }


    public void ShowWinOrLosePanel(int arg) // 1 - player win, 2 - player lose.
    {
        if (WinOrLosePanel.gameObject != null)
            WinOrLosePanel.gameObject.SetActive(true);

        global_Data.isMenuOpened = true;
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
        CheckForOpenedMenu();
    }


    public void QuitLevel()
    {
        //foreach (Ship _ship in Player_Data.Instance.shipsDontDestroy)
        //    _ship.gameObject.SetActive(true);

        global_Data.Clear();

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

        //for (int i = 0; i < ships.Length; i++)
        //{
        //    ships[i].State = Ship.States.IDLE;
        //    SceneManager.MoveGameObjectToScene(ships[i].gameObject, SceneManager.GetSceneByName("MainScene"));
        //}

        //battle_Scene_Controller.freeUSPs.Clear();
        SceneManager.UnloadSceneAsync(thisScene);
    }


    public void OpenMenu()
    {
        MenuPanel.gameObject.SetActive(true);
        global_Data.isMenuOpened = true;
    }


    public void CloseMenu()
    {
        MenuPanel.gameObject.SetActive(false);
        CheckForOpenedMenu();
    }


    public void ClickOnQuitButton()
    {
        ConfirmationPanel.gameObject.SetActive(true);
        global_Data.isMenuOpened = true;
    }


    public void ClickOnBackButtonInConfirmationPanel()
    {
        ConfirmationPanel.gameObject.SetActive(false);
    }


    private void CheckForOpenedMenu()
    {
        if (ConfirmationPanel.gameObject.activeSelf)
            global_Data.isMenuOpened = true;
        else
            global_Data.isMenuOpened = false;

        if (MenuPanel.gameObject.activeSelf)
            global_Data.isMenuOpened = true;
        else
            global_Data.isMenuOpened = false;

        if (WinOrLosePanel.gameObject.activeSelf)
            global_Data.isMenuOpened = true;
        else
            global_Data.isMenuOpened = false;
    }
}
