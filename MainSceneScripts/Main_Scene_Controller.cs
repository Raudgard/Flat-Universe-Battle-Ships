using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using SaveUtility;



public class Main_Scene_Controller : MonoBehaviour
{
    public GameObject quitPanel;
    [SerializeField] private TextMeshProUGUI USPCount;

    public Ship[] playersShipsInScene;
    public Ship[] AIShipsInScene = new Ship[0];

    private Global_Controller global_Data;

    private void Awake()
    {
        global_Data = Global_Controller.Instance;
    }


    private void Start()
    {
        quitPanel.gameObject.SetActive(false);
        //Battle_Scene_Controller.Instance.freeUSPs.Clear();  //пришлось ввести, потому что не всегда полностью очищает список еды по окончанию уровня

        playersShipsInScene = global_Data.CreateShips(SaveUtil.dataOfPlayer);
        if(SaveUtil.dataOfAI != null)
        {
            AIShipsInScene = global_Data.CreateShips(SaveUtil.dataOfAI);
            Player_Data.Instance.enemyShips = AIShipsInScene;
        }
        Player_Data.Instance.playerShips = playersShipsInScene;

        for (int i = 0; i < playersShipsInScene.Length; i++)
        {
            playersShipsInScene[i].shipBars.SetActive(false);
            playersShipsInScene[i].Idle();
        }
        for (int i = 0; i < AIShipsInScene.Length; i++)
        {
            AIShipsInScene[i].shipBars.SetActive(false);
            AIShipsInScene[i].Idle();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < playersShipsInScene.Length; i++)
        {
            if (playersShipsInScene[i] != null)
                playersShipsInScene[i].FixUpdateMe();
        }
        for (int i = 0; i < AIShipsInScene.Length; i++)
        {
            if (AIShipsInScene[i] != null)
                AIShipsInScene[i].FixUpdateMe();
        }
    }
    private void Update()
    {
        USPCount.text = "" + Player_Data.Instance.USPCount;
    }





    




}
