using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Класс для тестирования гена Cookshop
/// </summary>
public class Testing_time : MonoBehaviour
{
    public float FLEE_time = 0;
    public float IDLE_time = 0;
    public float TO_USP_time = 0;
    public float FIGHT_time = 0;
    public float REPRODUCTION_time = 0;
    public float STUNNED_time = 0;
    public float all_time = 0;
    public float summ_of_times = 0;
    public float percent_of_time_enable_cookshop = 0;
    public float percent_of_time_disable_cookshop = 0;

    private Ship ship;
    private BattleSceneController battle_Scene_Controller;

    private void Awake()
    {
        ship = GetComponent<Ship>();
    }
    void Start()
    {
        battle_Scene_Controller = BattleSceneController.Instance;
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        if (battle_Scene_Controller == null)
            yield break;
        
        while(true)
        {
            switch(ship.State)
            {
                case Ship.States.FIGHT:
                    FIGHT_time += Time.deltaTime;
                    break;
                case Ship.States.FLEE:
                    FLEE_time += Time.deltaTime;
                    break;
                case Ship.States.IDLE:
                    IDLE_time += Time.deltaTime;
                    break;
                case Ship.States.REPRODUCTION:
                    REPRODUCTION_time += Time.deltaTime;
                    break;
                case Ship.States.STUNNED:
                    STUNNED_time += Time.deltaTime;
                    break;
                case Ship.States.TO_USP:
                    TO_USP_time += Time.deltaTime;
                    break;
                default:
                    throw new System.Exception();
            }

            all_time += Time.deltaTime;
            summ_of_times = FIGHT_time + FLEE_time + IDLE_time + REPRODUCTION_time + STUNNED_time + TO_USP_time;

            percent_of_time_enable_cookshop = (FLEE_time + IDLE_time + TO_USP_time) / all_time;
            percent_of_time_disable_cookshop = (FIGHT_time + REPRODUCTION_time + STUNNED_time) / all_time;

            yield return null;
        }
    }
}
