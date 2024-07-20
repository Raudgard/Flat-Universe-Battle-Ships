using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SearchingForUSP : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private Rotation_Controller rotation_Control;
    Dictionary<float,USP> distanciesToUSPs = new Dictionary<float, USP>();
    public Vector2 directionToUSP;
    //private Battle_Scene_Controller battle_Scene_Controller;
    public float checkInterval;
    private Transform shipTransform;

    [Tooltip("Вторая по дистации USP от корабля. Обычно используется ультимейтом Telekinesis_Module.")]
    public USP secondDistanceUSP;

    private void Awake()
    {
        shipTransform = ship.transform;
        EventManager.Instance.AddListener(EVENT_TYPE.USP_CREATED, OnUSPCreated);
        EventManager.Instance.AddListener(EVENT_TYPE.USP_TAKEN, USPEaten);
    }

    private void Start()
    {
        StartCoroutine(CheckUSP());
    }


    private void OnUSPCreated(EVENT_TYPE event_type, Component component, object obj)
    {
        if (ship.IsStateFree && SearchingNearestUSP(out USP usp))
        {
            SendShipForUSP(usp);
        }
    }


    /// <summary>
    /// Ищет ближайший USP.
    /// </summary>
    /// <param name="usp"></param>
    /// <returns></returns>
    public bool SearchingNearestUSP(out USP usp)
    {
        usp = null;
        if (BattleSceneController.Instance == null || BattleSceneController.Instance.freeUSPs.Count == 0 /*|| enabled == false*/) 
        {
            return false;
        }

        Vector2 shipPosition = shipTransform.position;
        //float minDistance;

        foreach (USP _USP in BattleSceneController.Instance.freeUSPs)
        {
            Vector2 positionOfUSP = Vector2.zero;
            if (_USP != null)
                positionOfUSP = _USP.transform.position;
            float distanceSqr = (positionOfUSP - shipPosition).sqrMagnitude;
            if (!distanciesToUSPs.ContainsKey(distanceSqr))
            {
                distanciesToUSPs.Add(distanceSqr, _USP);
                //return; 
            }         //записываем в словарь при отсутствии такой же записи (одинакового расстояния до двух или более "еды")
            //minDistance = distanceSqr;
        }

        //foreach (KeyValuePair<float, USP> Item in distanciesToUSPs)
        //{
        //    if (Item.Key < minDistance)
        //        minDistance = Item.Key;
        //}

        //float minDistance = distanciesToUSPs.Keys.AsParallel().Min();
        var orderedDistancies = distanciesToUSPs.Keys.AsParallel().OrderBy(k => k).ToArray();
        //string log = "";
        //for (int i = 0; i < orderedDistancies.Length; i++)
        //{
        //    log += $"distance [{i}]: {orderedDistancies[i]}, ";
        //}

        //Debug.Log(log);

        float minDistance = orderedDistancies[0];
        secondDistanceUSP = orderedDistancies.Length > 1 ? distanciesToUSPs[orderedDistancies[1]] : null;
        //Debug.Log($"orderedDistancies[0]: {orderedDistancies[0]}, secondDistanceUSP: {secondDistanceUSP}");

        usp = distanciesToUSPs[minDistance];
        distanciesToUSPs.Clear();
        return true;
    }


    /// <summary>
    /// Отправляет корабль за USP.
    /// </summary>
    /// <param name="usp"></param>
    public void SendShipForUSP(USP usp)
    {
        ship.USPCurrent = usp;
        //print("ship " + ship.name + "report: my current USP is " + ship.USPCurrent);
        Vector2 USPPosition;

        //if (ship.USPCurrent != null)
        {
            ship.State = Ship.States.TO_USP;

            USPPosition = ship.USPCurrent.transform.position;
            Vector2 vectorToUSP = USPPosition - (Vector2)shipTransform.position;

            vectorToUSP = vectorToUSP.normalized;
            ship.moveDirection = vectorToUSP;
            rotation_Control.Rotate(vectorToUSP);

            //Debug.Log("CheckDistanceToUSP   in    SendShipForUSP");
            CheckDistanceToUSP();//проверка расстояния до еды на тот случай, если она уже внутри коллайдера
            //StartCoroutine(MeasuringSpeed());
        }
    }


    private void CheckDistanceToUSP()
    {
        float sqrDistance;
        float sqrSizeOfShipCollider = ship.radiusSize;
        sqrSizeOfShipCollider *= sqrSizeOfShipCollider;
        
        sqrDistance = ((Vector2)ship.transform.position - (Vector2)ship.USPCurrent.transform.position).sqrMagnitude;

        //print("sqrDistance = " + sqrDistance + "    sqrSize = " + sqrSizeOfShipCollider + "    distance = "+((Vector2)ship.transform.position - (Vector2)ship.USPCurrent.transform.position).magnitude);
        if (sqrDistance < sqrSizeOfShipCollider)
        {
            //Debug.Log("USP into ship collider!");
            ship.USPCurrent.USPTaken(ship); 
        }
    }

    void USPEaten(EVENT_TYPE event_type, Component component, object obj)
    {
        if (ship.USPCurrent != null && ship.USPCurrent != (USP)component)
        {
            return;
        }//если съедена не еда-цель этого микрота

        if (ship.USPCurrent != null)
        {
            ship.USPCurrent = null;
            if (ship.State != Ship.States.REPRODUCTION && ship.State != Ship.States.FIGHT && ship.State != Ship.States.STUNNED && ship.State != Ship.States.DIGESTION)
            {
                ship.Idle();
            }

        }
    }


    /// <summary>
    /// проверка раз в некоторое время правильности направления движения к еде
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckUSP()
    {
        while (true)
        {
            if (ship.State != Ship.States.TO_USP)
            {
                yield return new WaitForSeconds(checkInterval);
                continue;
            }
           
            if (ship.USPCurrent == null)
            {
                yield return new WaitForSeconds(checkInterval);
                continue;
            }

            Vector2 USPPosition = ship.USPCurrent.transform.position;
            Vector2 vectorToUSP = USPPosition - (Vector2)ship.transform.position;

            //print("angle = " + Vector2.Angle(vectorToUSP, ship.moveDirection));
            if(Vector2.Angle(vectorToUSP, ship.moveDirection) > 10 && ship.State == Ship.States.TO_USP)
            {
                //Debug.DrawRay(ship.transform.position, vectorToUSP, Color.blue, 3);
                //print("топаю мимо еды! ship: " + ship.name);
                vectorToUSP = vectorToUSP.normalized;
                ship.moveDirection = vectorToUSP;
                rotation_Control.Rotate(vectorToUSP);
            }

            //Debug.Log("CheckDistanceToUSP   in    CheckUSP");
            CheckDistanceToUSP();//заодно проверим не находится ли еда уже внутри коллайдера.

            yield return new WaitForSeconds(checkInterval);
        }
    }


    //IEnumerator MeasuringSpeed()
    //{
    //    Vector2 start = transform.position;
    //    yield return new WaitForSeconds(1);
    //    Vector2 finish = transform.position;
    //    float distance = Vector2.Distance(start, finish);
    //    print("distance = " + distance);
    //}
}
