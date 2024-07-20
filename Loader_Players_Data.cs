using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MODULES;
using SaveUtility;

public class Loader_Players_Data : MonoBehaviour
{
    //[SerializeField] Ship shipPrefab;
    //private Colors colors;
    //List<Ship> ships = new List<Ship>();
    private Global_Controller global_Controller;

    void Start()
    {
        global_Controller = Global_Controller.Instance;
        //colors = global_Controller.GetComponent<Colors>();
        Player_Data.ResizeCamera();
        RU_Texts.Instance.GetText("");
        Application.targetFrameRate = 60;

        if (!SaveUtil.Load())
        {
            Debug.LogError("Can't Load Data!");
            return;
        }


        //var playerShipsLoaded = global_Controller.CreateShips(SaveUtil.dataOfPlayer);
        //var AIshipsLoaded = global_Controller.CreateShips(SaveUtil.dataOfAI);


        //for (int i = 0; i < playerShipsLoaded.Length; i++)
        //{
        //    playerShipsLoaded[i].GetComponentInChildren<Canvas>().enabled = false;//убираем канвас с полосками жизней у загруженных микротов
        //    playerShipsLoaded[i].transform.position = new Vector3(playerShipsLoaded[i].startingPosition.x, playerShipsLoaded[i].startingPosition.y, -1);
        //    //ships.Add(playerShipsLoaded[i]);
        //}

        //for (int i = 0; i < AIshipsLoaded.Length; i++)
        //{
        //    AIshipsLoaded[i].GetComponentInChildren<Canvas>().enabled = false;//убираем канвас с полосками жизней у загруженных микротов
        //    AIshipsLoaded[i].transform.position = new Vector3(AIshipsLoaded[i].startingPosition.x, AIshipsLoaded[i].startingPosition.y, -1);
        //    //ships.Add(AIshipsLoaded[i]);
        //}

        //Player_Data.Instance.shipForMaze = ships[0];
        StartCoroutine(Waiting(0.5f));
    }

    private IEnumerator Waiting(float time)
    {
        yield return new WaitForSeconds(time);

        StartCoroutine(LoadNewSceneAsync());
    }
    private IEnumerator LoadNewSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }

        //for (int i = 0; i < ships.Count; i++)
        //{
        //    SceneManager.MoveGameObjectToScene(ships[i].gameObject, SceneManager.GetSceneByName("MainScene"));
        //    //SceneManager.MoveGameObjectToScene(ships[i].transform.parent.gameObject, SceneManager.GetSceneByName("MainScene"));
        //}

        SceneManager.UnloadSceneAsync(thisScene);
    }



   

    
}
