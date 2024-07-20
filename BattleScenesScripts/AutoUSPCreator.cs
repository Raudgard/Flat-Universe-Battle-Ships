using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUSPCreator : MonoBehaviour
{
    public float interval;
    public float radiusOfRandomCircle;

    public Coroutine USPCreatorCoroutine;
    [SerializeField] private BattleSceneController battle_Scene_Controller;
    [SerializeField] private Global_Controller global_Data;

    public USP USPPrefab;
    private LayerMask shipLayer;

    private void Start()
    {
        global_Data = Global_Controller.Instance;
        shipLayer = LayerMask.GetMask("ShipLayer");
    }

    public void StartCreate(Vector2 _position)
    {
        if (Player_Data.Instance.USPCountInBattle <= 0 || global_Data.isMenuOpened)
            return;

        if (USPCreatorCoroutine == null)
            USPCreatorCoroutine = StartCoroutine(USPCreator(_position));
        else
        {
            StopCoroutine(USPCreatorCoroutine);
            USPCreatorCoroutine = StartCoroutine(USPCreator(_position));
        }
    }

    public IEnumerator USPCreator(Vector2 _position)
    {
        CreateUSP(_position);

        yield return new WaitForSeconds(interval);

        while(Player_Data.Instance.USPCountInBattle > 0 && !global_Data.isMenuOpened)
        {
            Vector2 placeForUSP = Random.insideUnitCircle * radiusOfRandomCircle;
            CreateUSP(CheckNewUSPPosition(_position, placeForUSP + _position));

            yield return new WaitForSeconds(interval);

        }


    }

    private bool CheckForFreePlace(Vector2 place)
    {
        Ray ray = Camera.main.ViewportPointToRay(place);//посылаем луч "в экран"
        print("place = " + place);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayer))
            print("I'm looking at " + hit.transform.name);
        else
        {
            print("I'm looking at nothing!");
            return true;
        }
        //RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        //print("hit.name = " + hit.transform.name);

        return false;
    }//метод для проверки нахождения еды в том же месте. Но не работает((

    public void CreateUSP(Vector2 placeForNewUSP)
    {
        if (Player_Data.Instance.USPCountInBattle <= 0 || global_Data.isMenuOpened)
            return;

        USP newUSP = USP.Create(placeForNewUSP, 0);//создаем еду

        //Battle_Scene_Controller.Instance.freeUSPs.Add(newUSP);//создаем еду
        //Vector3 positionOfNewUSP = new Vector3(placeForNewUSP.x, placeForNewUSP.y, -2);
        //battle_Scene_Controller.freeUSPs[battle_Scene_Controller.freeUSPs.Count - 1].GetComponent<Transform>().position = positionOfNewUSP;
        //newUSP.transform.position = positionOfNewUSP;//позиция еды равна месту пересечения луча с "экраном"
        //newUSP.team = 1;
        Player_Data.Instance.USPCountInBattle--;

        //EventManager.Instance.PostNotification(EVENT_TYPE.USP_CREATED, Battle_Scene_Controller.Instance.freeUSPs[Battle_Scene_Controller.Instance.freeUSPs.Count - 1], positionOfNewUSP);
        //battle_Scene_Controller.scene_UI_Controller.CalculateApM(0);


        //print("freeUSPs.Count = " + freeUSPs.Count + "   freeUSPs.Capacity = " + freeUSPs.Capacity);//СЛЕДИТЬ ЗА ПАМЯТЬЮ, ВЫДЕЛЯЕМОЙ  ДЛЯ "ЕДЫ". ОСОБЕННО ПОСЛЕ СМЕНЫ СЦЕН!!!
    }

    /// <summary>
    /// Метод, проверяющий выход позиции новой еды за периметр поля боя
    /// </summary>
    /// <param name="center">центр круга автосоздания еды</param>
    /// <param name="positionOfNewUSP">конкретная предлагаемая позиция новой еды</param>
    /// <returns></returns>
    private Vector2 CheckNewUSPPosition(Vector2 center, Vector2 positionOfNewUSP)
    {
        //Vector2 res = position;
        //float overline;

        //if (positionOfNewUSP.y > battle_Scene_Controller.topBorderOfBattleField)
        //{
        //    overline = positionOfNewUSP.y - center.y;
        //    overline *= 2;
        //    positionOfNewUSP.y -= overline;
        //}

        //if (positionOfNewUSP.y < battle_Scene_Controller.bottomBorderOfBattleField)
        //{
        //    overline = center.y - positionOfNewUSP.y;
        //    overline *= 2;
        //    positionOfNewUSP.y += overline;
        //}

        //if (positionOfNewUSP.x > battle_Scene_Controller.rightBorderOfBattleField)
        //{
        //    overline = positionOfNewUSP.x - center.x;
        //    overline *= 2;
        //    positionOfNewUSP.x -= overline;
        //}

        //if (positionOfNewUSP.x < battle_Scene_Controller.leftBorderOfBattleField)
        //{
        //    overline = center.x - positionOfNewUSP.x;
        //    overline *= 2;
        //    positionOfNewUSP.x += overline;
        //}


        return positionOfNewUSP;

    }



}
