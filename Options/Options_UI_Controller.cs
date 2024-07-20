using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_UI_Controller : MonoBehaviour
{
    private void Start()
    {
        Player_Data.ResizeCamera();
    }

    public void OnBackButton()
    {
        //foreach (Ship _ship in Player_Data.Instance.shipsDontDestroy)
        //    _ship.gameObject.SetActive(true);

        StartCoroutine(LoadMainSceneAsync());
    }
    private IEnumerator LoadMainSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        //Ship[] ships = Player_Data.Instance.shipsDontDestroy;
        //Ship[] AIShips = null;
        //if (Player_Data.Instance.AIshipDontDestroy != null)
        //{ 
        //    AIShips = Player_Data.Instance.AIshipDontDestroy;
        //}

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //if(AIShips != null)
        //{
        //    for(int i = 0; i < AIShips.Length; i++)
        //    {
        //        AIShips[i].State = Ship.States.IDLE;
        //        SceneManager.MoveGameObjectToScene(AIShips[i].gameObject, SceneManager.GetSceneByName("MainScene"));
        //    }
        //}

        //for (int i = 0; i < ships.Length; i++)
        //{
        //    ships[i].State = Ship.States.IDLE;
        //    SceneManager.MoveGameObjectToScene(ships[i].gameObject, SceneManager.GetSceneByName("MainScene"));
        //}

        //battle_Scene_Controller.freeUSPs.Clear();
        SceneManager.UnloadSceneAsync(thisScene);
    }
}
