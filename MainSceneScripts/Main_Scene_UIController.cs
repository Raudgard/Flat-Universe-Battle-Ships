using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Scene_UIController : MonoBehaviour
{
    public GameObject quitPanel;
    public GameObject mainMenu;
    public GameObject singlePlayerMenu;

    private void Start()
    {
        StartCoroutine(CheckForLoadSceneAndResizeCamera());
    }

    private IEnumerator CheckForLoadSceneAndResizeCamera()
    {
        Scene activeScene = SceneManager.GetActiveScene(); 
        while(activeScene != SceneManager.GetSceneByName("MainScene"))
        {
            activeScene = SceneManager.GetActiveScene();
            yield return null;
        }
        Player_Data.ResizeCamera();
    }


    #region Main menu

    public void OnSinglePlayerClick()
    {
        mainMenu.gameObject.SetActive(false);
        singlePlayerMenu.gameObject.SetActive(true);
    }

    public void OnMultiplayerClick()
    {

    }

    public void OnHangarClick()
    {
        Ship[] ships = FindObjectsOfType<Ship>();
        foreach (Ship ship in ships)
            DontDestroyOnLoad(ship.gameObject);

        SceneManager.LoadScene("Hangar", LoadSceneMode.Single);
    }


    public void OnBattleFormationClick()
    {
        StartCoroutine(LoadBattleFormationSceneAsync());
    }

    public void OnOptionsClick()
    {
        //Ship[] ships = FindObjectsOfType<Ship>();

        //foreach (Ship ship in ships)
        //{
        //    ship.gameObject.SetActive(false);
        //    DontDestroyOnLoad(ship.gameObject);
        //}
        //Player_Data.Instance.shipsDontDestroy = ships;
        SceneManager.LoadScene("Options", LoadSceneMode.Single);
    }

    private IEnumerator LoadBattleFormationSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleFormation", LoadSceneMode.Single);

        //Ship[] ships = FindObjectsOfType<Ship>();

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //for (int i = 0; i < ships.Length; i++)
        //    SceneManager.MoveGameObjectToScene(ships[i].gameObject, SceneManager.GetSceneByName("BattleFormation"));

        SceneManager.UnloadSceneAsync(thisScene);
    }


    #endregion



    #region SinglePlayer

    public void OnStoryClick()
    {

    }

    public void OnSkirmishClick()
    {
        //Ship[] ships = FindObjectsOfType<Ship>();
        //Player_Data.Instance.shipsDontDestroy = ships;
        ////print("Player_Data.Instance.dontDestroyShipsObjects.Length = " + Player_Data.Instance.shipsDontDestroy.Length);

        //foreach (Ship ship in ships)
        //    DontDestroyOnLoad(ship.gameObject);

        SceneManager.LoadScene("ScirmishScene", LoadSceneMode.Single);
    }

    public void OnInfinityClick()
    {

    }

    public void OnMazeClick()
    {
        //Ship[] ships = FindObjectsOfType<Ship>();
        //Player_Data.Instance.shipsDontDestroy = ships;
        ////print("Player_Data.Instance.dontDestroyShipsObjects.Length = " + Player_Data.Instance.shipsDontDestroy.Length);

        //foreach (Ship ship in ships)
        //    DontDestroyOnLoad(ship.gameObject);

        SceneManager.LoadScene("Maze", LoadSceneMode.Single);
    }

    public void OnBackClick()
    {
        singlePlayerMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    #endregion


    
    public void OnExitClick()
    {
        if (quitPanel.activeSelf)
            Application.Quit();
        else
            StartCoroutine(ShowQuitPanel());
    }

    private IEnumerator ShowQuitPanel()
    {
        quitPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        quitPanel.gameObject.SetActive(false);
    }

    
}
