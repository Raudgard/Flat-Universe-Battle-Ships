using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum WhatWasClicked
{
    ClickController,
    USP,
    Obstacle,
    
}

public class ClickController : MonoBehaviour
{
    public BattleSceneController battle_Scene_Controller;
    private Global_Controller global_Controller;
    private EdgeCollider2D bordersCollider;
    private PolygonCollider2D clickableAreaCollider;
    public USP USPPrefab;
    private USPEnlarger USPEnlarger;
    public float minimumDistanceSqrFromBorder;

    private Transform cameraTransform;
    private float mouseX;
    private float mouseY;
    private float mouseWheel;
    public float minOrthographicSize;
    public float maxOrthographicSize;
    //public float touchMovePenalty;
    public float zoomCoeff;
    public float swipeCoeff;
    public float zoomCoeffForWin;

    public Slider zoomCoeffSlider;
    public Slider swipeCoeffSlider;
    public TextMeshProUGUI swipeCoeffValue_Label;
    public TextMeshProUGUI zoomCoeffValue_Label;
    public TextMeshProUGUI debugPanel;


    private bool isPreviousMoved;//если перед отпусканием была фаза Move или масштабирование двумя пальцами, тогда - true.

    private Coroutine holdingClickHandlerCoroutine = null;
    [SerializeField] private Image holdingIndicatorImage;
    private float holdingPreTime;
    private float holdingIndicatorTime;
    private Vector3 previousMousePosition = new Vector3();


    private void Awake()
    {
        global_Controller = Global_Controller.Instance;
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        bordersCollider = GameObject.FindGameObjectWithTag("Borders").GetComponent<EdgeCollider2D>();
        clickableAreaCollider = GameObject.FindGameObjectWithTag("ClickableArea").GetComponent<PolygonCollider2D>();
        var go = GameObject.FindGameObjectWithTag("PlayerUSPEnlarger");
        if (go != null)
            USPEnlarger = go.GetComponent<USPEnlarger>();
    }



    private void Update()
    {
        //if (Global_Controller.Instance.isMenuOpened) return;
        if (global_Controller.IsGamePaused) return;


#if UNITY_ANDROID

        float touchCount = Input.touchCount;
        if (touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            debugPanel.text = "touch count 1, touch phase: " + touch.phase + ", sqrMag:" + touch.deltaPosition.sqrMagnitude + " fingerId: " + touch.fingerId;
            if (touch.phase == TouchPhase.Moved)
            {
                var moveCamera = -touch.deltaPosition * swipeCoeff * Camera.main.orthographicSize / 10;
                cameraTransform.position += (Vector3)moveCamera;
                isPreviousMoved = true;

                holdingPreTime = 0;
                holdingIndicatorTime = 0;
                holdingIndicatorImage.gameObject.SetActive(false);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isPreviousMoved)
                {
                    isPreviousMoved = false;
                }
                else
                {
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    if (IsPositionOfNewUSPAllowed(ray.origin))
                        CreateUSP(ray.origin);
                }

                holdingPreTime = 0;
                holdingIndicatorTime = 0;
                holdingIndicatorImage.gameObject.SetActive(false);
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                HoldingClickHandler(touch.position);
            };



        }
        else if (touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDelta = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDelta = (touch0.position - touch1.position).magnitude;

            float zoomDelta = prevTouchDelta - touchDelta;

            if (zoomDelta > 0.01f || zoomDelta < -0.01f)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize += zoomDelta * zoomCoeff, minOrthographicSize, maxOrthographicSize);
            }
            debugPanel.text = "touch count 2";
            isPreviousMoved = true;
        }

#endif


#if UNITY_EDITOR_WIN

        if (Input.GetMouseButtonUp(0))
        {
            holdingPreTime = 0;
            holdingIndicatorTime = 0;
            holdingIndicatorImage.gameObject.SetActive(false);

            if (Input.GetMouseButton(1))
            {
                return;
            }
            var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (IsPositionOfNewUSPAllowed(ray1.origin))
                CreateUSP(ray1.origin);
        }


        if (Input.GetMouseButton(1))
        {
            //print("Mouse button 1 was pressed!");

            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            Vector3 moveCamera = new Vector3(-mouseX, -mouseY, 0) * Camera.main.orthographicSize / 10;
            cameraTransform.position += moveCamera;


        }

        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel < -0.01f || mouseWheel > 0.01f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= mouseWheel * zoomCoeffForWin, minOrthographicSize, maxOrthographicSize);
        }

        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            //print($"Mouse button 0 was pressed! mousePosition: {mousePosition}");
            if (mousePosition == previousMousePosition)
            {
                HoldingClickHandler(mousePosition);
            }
            else
            {
                holdingPreTime = 0;
                holdingIndicatorTime = 0; 
                holdingIndicatorImage.gameObject.SetActive(false);
            }
            previousMousePosition = mousePosition;
        }



#endif
    }


    private void HoldingClickHandler(Vector2 touchPosition)
    {
        if (holdingClickHandlerCoroutine != null)
            return;

        var settings = References.Instance.settings;
        holdingPreTime += Time.deltaTime;
        if (holdingPreTime > settings.touchTimeBeforeReaction)
        {
            holdingIndicatorTime += Time.deltaTime;
            holdingIndicatorImage.gameObject.SetActive(true);
            holdingIndicatorImage.transform.position = touchPosition;
            holdingIndicatorImage.fillAmount = holdingIndicatorTime / settings.touchTimeForReaction;
            if (holdingIndicatorTime > settings.touchTimeForReaction &&
                IsPositionOfNewUSPAllowed(Camera.main.ScreenPointToRay(touchPosition).origin))
            {
                holdingClickHandlerCoroutine = StartCoroutine(HoldingTouchCreatingUSP());
                holdingIndicatorImage.gameObject.SetActive(false);

            }
        }
    }




    private IEnumerator HoldingTouchCreatingUSP()
    {
        var settings = References.Instance.settings;
        var timeDelay = new WaitForSeconds(settings.timeBetweenUSPCreationWhileHolding);
        //Debug.Log($"timeDelay: {timeDelay}");
        Vector2 position;
        Vector2 place;

#if UNITY_ANDROID

        while (Input.touchCount == 1)
        {
            position = Camera.main.ScreenPointToRay(Input.GetTouch(0).position).origin;
            place = position + Random.insideUnitCircle * settings.holdingTouchCreationRadius;
            if (IsPositionOfNewUSPAllowed(place))
            {
                CreateUSP(place);
            }
            yield return timeDelay;
        }

#endif

#if UNITY_EDITOR_WIN

        while (Input.GetMouseButton(0))
        {
            position = Camera.main.ScreenPointToRay((Vector2)Input.mousePosition).origin;
            place = position + Random.insideUnitCircle * settings.holdingTouchCreationRadius;
            if (IsPositionOfNewUSPAllowed(place))
            {
                CreateUSP(place);
            }
            yield return timeDelay;
        }

#endif
        holdingClickHandlerCoroutine = null;
    }






    public void CreateUSP(Vector2 placeForNewUSP)
    {
        if (/*global_Data.isMenuOpened || */USPEnlarger.CurrentQuantity <= 0)
            return;

        USP.Create(placeForNewUSP, 1);//создаем USP
        USPEnlarger.CurrentQuantity--;
    }


    /// <summary>
    /// Проверяет на близость нового USP к границе
    /// </summary>
    /// <param name="placeForNewUSP"></param>
    /// <returns></returns>
    private bool IsPositionOfNewUSPAllowed(Vector2 placeForNewUSP)
    {
        if (!clickableAreaCollider.OverlapPoint(placeForNewUSP))
            return false;

        if (Global_Controller.Instance.isUIElementPressed) return false;


        for (int i = 0; i < Global_Controller.Instance.notClickableGameObjects.Count; i++)
        {
            if(Global_Controller.Instance.notClickableGameObjects[i].OverlapPoint(placeForNewUSP))
            {
                Debug.Log($"Click on notClickable object: {Global_Controller.Instance.notClickableGameObjects[i].gameObject.name}");
                return false;
            }
        }


        var closestPoint = bordersCollider.ClosestPoint(placeForNewUSP);
        var distanceSqrFromBorder = (closestPoint - placeForNewUSP).sqrMagnitude;
        if (distanceSqrFromBorder < minimumDistanceSqrFromBorder)
        {
            //print("distanceSQRFromBorder = " + distanceSqrFromBorder);
            return false;
        }
        
        return true;
    }





    public void SetZoomCoeff()
    {
        zoomCoeff = zoomCoeffSlider.value;
        zoomCoeffValue_Label.text = zoomCoeffSlider.value.ToString();

    }

    public void SetSwipeCoeff()
    {
        swipeCoeff = swipeCoeffSlider.value;
        swipeCoeffValue_Label.text = swipeCoeffSlider.value.ToString();
    }












    //Система даблкликов


    //private void OnMouseDown()
    //{
    //if(!Player_Data.Instance.isAutoUSPCreationON)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//посылаем луч "в экран"

    //    CreateUSP(ray.origin);
    //    Click();
    //    return;
    //}

    //CheckForDoubleClick(WhatWasClicked.ClickController);
    //}

    //public void Click()
    //{
    //    StartCoroutine(Clicked());
    //}

    //private IEnumerator Clicked()
    //{
    //    isDoubleClick = true;
    //    yield return new WaitForSeconds(Options.doubleClickTime);
    //    isDoubleClick = false;
    //}

    /// <summary>
    /// Проверка на двойное нажатие
    /// </summary>
    /// <param name="whoCalled">На чем произошло нажатие</param>
    //public void CheckForDoubleClick(WhatWasClicked whatWasClicked)
    //{
    //    if (isDoubleClick)
    //    {
    //        //print("double click");
    //        if(autoUSPCreator != null && autoUSPCreator.USPCreatorCoroutine != null)
    //            autoUSPCreator.StopCoroutine(autoUSPCreator.USPCreatorCoroutine);
    //    }
    //    else if (whatWasClicked == WhatWasClicked.ClickController)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//посылаем луч "в экран"

    //        //if (!battle_Scene_Controller.isBorderVanished && ray.origin.y > dividingLine.transform.position.y)
    //        //{
    //        //    print("Выше разделительной линии");
    //        //    Click();
    //        //    return;
    //        //}
    //        autoUSPCreator.StartCreate(ray.origin);
    //    }

    //    Click();
    //}













}
