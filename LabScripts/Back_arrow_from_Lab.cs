using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_arrow_from_Lab : MonoBehaviour
{
    private void OnMouseDown()
    {
        StartCoroutine(LoadNewSceneAsync());
    }
    private IEnumerator LoadNewSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        //Ship[] ships = FindObjectsOfType<Ship>();

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //for (int i = 0; i < ships.Length; i++)
        //    SceneManager.MoveGameObjectToScene(ships[i].gameObject, SceneManager.GetSceneByName("MainScene"));

        SceneManager.UnloadSceneAsync(thisScene);
    }
}
