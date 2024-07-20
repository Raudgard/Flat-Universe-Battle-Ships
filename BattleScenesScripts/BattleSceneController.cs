using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Выполняет все общие функции для сцен с кораблями и боем.
/// </summary>
public class BattleSceneController : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Singleton
    private static BattleSceneController instance;
    public static BattleSceneController Instance => instance;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public BattleSceneUIController scene_UI_Controller;

    public GameObject[] DeadShips;
    public List<USP> freeUSPs = new List<USP>();

    public List<Transform> ShipsPlayer_number_Transforms;
    public Transform USPs_Transform;
    public Transform Projectiles_Transform;
    public Transform DepartingDigits_Transform;
    public Transform Debris_Transform;
    public Transform BackgroundLights_Transform;
    public bool isNeedTrailForUSP;


    private void Start()
    {
        isNeedTrailForUSP = false;
        DontDestroyOnLoad(DepartingDigits_Transform.gameObject);
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        StartCoroutine(CreatingBackgroundLights());





        //убираем туман войны в случае, если в настройках игрока он убран (пока только для Develop Mode)
        //fogOfWar = GameObject.FindGameObjectWithTag("FogOfWar");
        //if (!Player_Data.Instance.fogOfWar)
        //    fogOfWar.SetActive(false);

    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        //if (/*scene.name == "BattleScene" &&*/ DepartingDigits_Transform != null) Global_Controller.Instance.DestroyGO_WithDelay(DepartingDigits_Transform.gameObject, 5);

        if(DepartingDigits_Transform != null)
        {
            Tools.UnityTools.ExecuteWithDelay(() =>
            {
                if (DepartingDigits_Transform != null)
                    Destroy(DepartingDigits_Transform.gameObject);
            }, 5f);
        }
    }

    private void PlayerWin()
    {
        Player_Data.Instance.USPCount += Player_Data.Instance.USPCountInBattle;

        AI AIcomponent = FindObjectOfType<AI>();
        if (AIcomponent != null)
        {
            Player_Data.Instance.USPCount += AIcomponent.USPCountAI;
        }

        scene_UI_Controller.ShowWinOrLosePanel(1);
    }


    private void PlayerLose()
    {
        scene_UI_Controller.ShowWinOrLosePanel(2);
        return;
    }

    

        
    private IEnumerator CreatingBackgroundLights()
    {
        int _numberOfBackgroundLigths = 0;
        while(_numberOfBackgroundLigths < 10)
        {
            BackGround_Lights backgoundLigth = Instantiate(Prefabs.Instance.BackGround_Lights_prefab);
            //backgoundLigth.transform.parent = BackgroundLights;
            backgoundLigth.transform.parent = BackgroundLights_Transform;

            _numberOfBackgroundLigths++;
            yield return new WaitForSeconds(10);
        }
    }



    


    
    



    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //for (int i = 0; i < allShips.Count; i++)
        //{
        //    Gizmos.DrawWireSphere(allShips[i].transform.position, allShips[i].GetComponent<CircleCollider2D>().bounds.extents.x);
        //}

        //Gizmos.color = Color.red;
        //for (int i = 0; i < allShips.Count; i++)
        //{
        //    Gizmos.DrawWireSphere(allShips[i].transform.position, allShips[i].visionRadius);
        //}

        //Gizmos.color = Color.white;
        //for (int i = 0; i < allShips.Count; i++)
        //{
        //    Gizmos.DrawWireSphere(allShips[i].transform.position, allShips[i].GetComponent<CircleCollider2D>().bounds.extents.x + allShips[i].attack_range);
        //}


        //Gizmos.color = Color.green;
        //for (int i = 0; i < battlefield.SpawnPointsFor2And4And8Players.Length; i++)
        //{
        //    if (battlefield.SpawnPointsFor2And4And8Players[i] == null) break;
        //    Gizmos.DrawWireSphere(battlefield.SpawnPointsFor2And4And8Players[i], 5);
        //}

        //Gizmos.color = Color.cyan;
        //for (int i = 0; i < battlefield.SpawnPointsFor3And6Players.Length; i++)
        //{
        //    if (battlefield.SpawnPointsFor3And6Players[i] == null) break;

        //    Gizmos.DrawWireSphere(battlefield.SpawnPointsFor3And6Players[i], 5);
        //}

        //Gizmos.color = Color.cyan;
        //for (int i = 0; i < Battlefield.SpawnPointsFor3And6Players.Length; i++)
        //{
        //    Gizmos.DrawWireSphere(Battlefield.SpawnPointsFor3And6Players[i], 5);
        //}

    }
}
