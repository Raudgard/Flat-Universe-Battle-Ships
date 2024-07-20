using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ClickControllerForMaze : MonoBehaviour
{
    private Global_Controller global_Data;

    private bool isDoubleClick = false;
    public USP USPPrefab;


    private void Awake()
    {
        global_Data = Global_Controller.Instance;
    }

    private void Start()
    {

    }



    private void OnMouseDown()
    {
        if (!Player_Data.Instance.isAutoUSPCreationON)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//�������� ��� "� �����"

            CreateUSP(ray.origin);
            Click();
            return;
        }

        CheckForDoubleClick(WhatWasClicked.ClickController);
    }

    public void Click()
    {
        StartCoroutine(Clicked());
    }

    private IEnumerator Clicked()
    {
        isDoubleClick = true;
        yield return new WaitForSeconds(Options.doubleClickTime);
        isDoubleClick = false;
    }

    /// <summary>
    /// �������� �� ������� �������
    /// </summary>
    /// <param name="whoCalled">�� ��� ��������� �������</param>
    public void CheckForDoubleClick(WhatWasClicked whatWasClicked)
    {
        if (isDoubleClick)
        {
            //print("double click");
        }
        else if (whatWasClicked == WhatWasClicked.ClickController)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//�������� ��� "� �����"
        }

        Click();
    }


    public void CreateUSP(Vector2 placeForNewUSP)
    {
        //if (Player_Data.Instance.USPCountInBattle <= 0 || global_Data.isMenuOpened)
        //    return;

        if (global_Data.isMenuOpened)
            return;

        USP newUSP = USP.Create(placeForNewUSP, 1);//������� ���

        //global_Data.freeUSPs.Add(newUSP);//������� ���
        //Vector3 positionOfNewUSP = new Vector3(placeForNewUSP.x, placeForNewUSP.y, -2);
        //newUSP.transform.position = positionOfNewUSP;//������� ��� ����� ����� ����������� ���� � "�������"
        //newUSP.team = 1;
        Player_Data.Instance.USPCountInBattle--;

        //EventManager.Instance.PostNotification(EVENT_TYPE.USP_CREATED, global_Data.freeUSPs[global_Data.freeUSPs.Count - 1], positionOfNewUSP);

        //print("freeUSPs.Count = " + freeUSPs.Count + "   freeUSPs.Capacity = " + freeUSPs.Capacity);//������� �� �������, ����������  ��� "���". �������� ����� ����� ����!!!
    }



}
