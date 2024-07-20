using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;

public enum DifficultyAI
{
    VERY_EASY,
    EASY,
    NORMAL,
    HARD,
    INSANE
}
public class AI : MonoBehaviour
{
    [SerializeField] private Ship shipPrefab;
    [SerializeField] private USP USPPrefab;
    [SerializeField] private BattleSceneController Battle_Scene_Controller;
    [SerializeField] private Global_Controller global_Controller;
    private Colors colors;
    [SerializeField] private BattleSceneUIController Scene_UI_Controller;

    //public static DifficultyAI difficulty = DifficultyAI.HARD;

    //public float strengthOfTouch = 3;
    private float rateOfCreationUSP = 3.0f;
    public int level = 3;
    public float distanceSqrFromBorderForNewUSP = 0.5f;
    //public List<Ship> shipsAI = new List<Ship>();

    public int USPCountAI = 500;
    public int USPCap;
    public int startingQuantityUSPInBattle;
    public float USPEnlargingSpeed; // USP per 1 second
    public float enlargingUSPspeedCoeff;
    private USPEnlarger USPEnlarger;
    private EdgeCollider2D bordersCollider;
    private PolygonCollider2D clickableAreaCollider;

    public static float sensivityRadiusSqr = 5;

    private void Start()
    {
        global_Controller = Global_Controller.Instance;
        colors = global_Controller.GetComponent<Colors>();
        USPEnlarger = GameObject.FindGameObjectWithTag("AI_USPEnlarger").GetComponent<USPEnlarger>();
        bordersCollider = GameObject.FindGameObjectWithTag("Borders").GetComponent<EdgeCollider2D>();
        clickableAreaCollider = GameObject.FindGameObjectWithTag("ClickableArea").GetComponent<PolygonCollider2D>();
        //difficulty = Player_Data.Instance.chosenDifficulty;
        USPCountAI = Player_Data.Instance.USPCountInBattle;
        if (Player_Data.Instance.isDevelopMode)
            USPCountAI = Player_Data.Instance.USPCountAI;

        SetRateOfCreateUSP();

        if(!Player_Data.Instance.isDevelopMode) 
            CreateShips();

        //FindShipsAI();

        if (Player_Data.Instance.canAICreateUSP)
            StartCoroutine(CreatingUSP());
    }


    //void FindShipsAI()
    //{
    //    Ship[] allShipsInScene = FindObjectsOfType<Ship>();
    //    List<Ship> shipsAItmp = new List<Ship>();
    //    foreach (Ship ship in allShipsInScene)
    //        if (ship.team % 2 == 0)
    //            shipsAItmp.Add(ship);
    //    shipsAI = shipsAItmp;
    //}

    private void SetRateOfCreateUSP()
    {
        rateOfCreationUSP = Player_Data.Instance.rateOfCreationUSPAI;
    }


    private void CreateShips()
    {
        int CountOfShipsForCreate()
        {
            if (level == 1)
                return 0;
            if (level == 2)
                return 4;
            if (level == 3)
                return 5;
            else return 5;
        }

        int countOfShipsAI = CountOfShipsForCreate();
        Vector3[] StartingPositions()
        {
            if (countOfShipsAI == 1)
                return new Vector3[] { new Vector3(0, 9, -1) };

            if (countOfShipsAI == 2)
            {
                Vector3[] res = new Vector3[2];
                res[0] = new Vector3(-4, 9, -1);
                res[1] = new Vector3(4, 9, -1);
                return res;
            }

            if (countOfShipsAI == 3)
                return new Vector3[]
                {
                    new Vector3(-5, 9, -1),
                    new Vector3(0, 9, -1),
                    new Vector3(5, 9, -1)
                };

            else
            {
                Vector3[] res = new Vector3[countOfShipsAI];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(5.0f, 9.0f), -1);

                return res;

            }
        }

        for (int i = 0; i < countOfShipsAI; i++)
        {
            //Ship shipAI = Instantiate(shipPrefab) as Ship;
            var shipAI = global_Controller.CreateShip(2, 2, (Ship.Forms)i,  StartingPositions()[i],
                new SaveUtility.ModuleSaved() { typeOfModule = typeof(Attack_Module),level = 110 }, 
                new SaveUtility.ModuleSaved() { typeOfModule = typeof(Health_Module), level = 110 },
                new SaveUtility.ModuleSaved() { typeOfModule = typeof(Movement_Speed_Module), level = 110 },
                new SaveUtility.ModuleSaved() { typeOfModule = typeof(Armor_Module), level = 110 }       
                );
            //shipAI.transform.position = StartingPositions()[i];
        }
    }

    private IEnumerator CreatingUSP()
    {
        while (true)
        {
            yield return new WaitForSeconds(rateOfCreationUSP);

            //if (USPCountAI == 0)
            //{
            //    Battle_Scene_Controller.emergencyCreatingUSP = Battle_Scene_Controller.StartCoroutine(Battle_Scene_Controller.EmergencyCreatingUSP());
            //    yield break;
            //}
            if(global_Controller.ships[2].Count == 0)
            {
                yield break;
            }

            if(USPEnlarger.CurrentQuantity == 0)
            {
                //yield return new WaitForSeconds(rateOfCreationUSP);
                continue;
            }

            //print("USPEnlarger.CurrentQuantity = " + USPEnlarger.CurrentQuantity);
            //FindShipsAI();//обновляем список shipsAI

            Vector2 positionOfNewUSP = GetPositionForNewUSP();

            //проверка на совпадение позиций нового USP и уже созданного, и на попадание нового USP в коллайдер границы. Если совпадают
            //(или попадает в границы), то алгоритм i раз пытается получить новое место, затем возвращает false  и пропускает создание USP
            bool CheckPlaceForUSP()
            {
                bool positionAllowed = true;
                //print("freeUSPs.Count = " + Battle_Scene_Controller.freeUSPs.Count);

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < BattleSceneController.Instance.freeUSPs.Count; j++)
                    {
                        if (BattleSceneController.Instance.freeUSPs[j] != null)
                        {
                            float positionOfUSP_X = BattleSceneController.Instance.freeUSPs[j].transform.position.x;
                            float positionOfUSP_Y = BattleSceneController.Instance.freeUSPs[j].transform.position.y;
                            if (Mathf.Abs(positionOfNewUSP.x - positionOfUSP_X) < 0.05 &&
                                Mathf.Abs(positionOfNewUSP.y - positionOfUSP_Y) < 0.05)
                            {
                                positionAllowed = false;
                                break;
                            }
                        }
                    }

                    if(!positionAllowed)
                    {
                        positionOfNewUSP = GetPositionForNewUSP();
                        continue;
                    }

                    var closestPoint = bordersCollider.ClosestPoint(positionOfNewUSP);
                    var distanceSqrFromBorder = (closestPoint - (Vector2)positionOfNewUSP).sqrMagnitude;
                    if (distanceSqrFromBorder < distanceSqrFromBorderForNewUSP)
                    {
                        //print("distanceSQRFromBorder = " + distanceSqrFromBorder);
                        positionAllowed = false;
                    }

                    if(!clickableAreaCollider.OverlapPoint(positionOfNewUSP))
                    {
                        //Debug.Log("Попытка ИИ создать USP вне разрешенной зоны! Координаты создаваемого USP: " + positionOfNewUSP);
                        positionAllowed = false;
                    }

                    if (!positionAllowed)
                    {
                        positionOfNewUSP = GetPositionForNewUSP();
                    }
                    else break;
                }
                //print("res = " + res);
                return positionAllowed;
            }


            if (CheckPlaceForUSP())
            {
                //USP newUSP = Instantiate(USPPrefab) as USP;//создаем еду
                USP newUSP = USP.Create(positionOfNewUSP, 2);//создаем еду

                //global_Controller.freeUSPs.Add(newUSP);
                //newUSP.transform.position = positionOfNewUSP;
                //newUSP.transform.parent = Battle_Scene_Controller.USPs;
                //newUSP.team = 2;
                //EventManager.Instance.PostNotification(EVENT_TYPE.USP_CREATED, newUSP, positionOfNewUSP);
                //USPCountAI--;//уменьшаем количество еды, "взятой" AI в уровень
                //print("NewUSPFromAI = " + positionOfNewUSP);
                USPEnlarger.CurrentQuantity--;
                //Scene_UI_Controller.CalculateApM(1);
            }
        }
    }


    private Vector2 GetPositionForNewUSP()
    {
        Vector2 coordinatesOfNewUSP;
        var shipsAI = global_Controller.ships[2];

        bool willNewUSPDontDisturbOtherShipsAI;
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < shipsAI.Count; i++)
            {
                willNewUSPDontDisturbOtherShipsAI = true;
                Vector2 _randomAdd = Random.insideUnitCircle.normalized;
                //print("randomAddstart = " + _randomAdd.magnitude);
                _randomAdd *= shipsAI[i].SensivityRadiusSqr;
                //print("randomAddChanged = " + _randomAdd.magnitude);
                coordinatesOfNewUSP = (Vector2)shipsAI[i].transform.position + _randomAdd;
                //coordinatesOfNewUSP = new Vector3(coordinatesOfNewUSP.x, coordinatesOfNewUSP.y, -2);
                //print("shipPosition = " + shipsAI[i].transform.position + "   randomAdd = " + _randomAdd + "    shipAI.StrengthOfTouch / 7 = " + shipsAI[i].StrengthOfTouch / 7 + "    _randomAdd.length = " + _randomAdd.magnitude);

                for (int k = 0; k < shipsAI.Count; k++)
                {
                    if (shipsAI[i] == shipsAI[k])
                        continue;
                    if (Vector2.SqrMagnitude(coordinatesOfNewUSP - (Vector2)shipsAI[k].transform.position) < shipsAI[k].SensivityRadiusSqr)
                    {
                        willNewUSPDontDisturbOtherShipsAI = false;
                        //print("Distance < norm   Distance = "+ Vector3.Distance(coordinatesOfNewUSP, shipsAI[k].transform.position));
                        break;
                    }
                }

                if (willNewUSPDontDisturbOtherShipsAI)
                    return coordinatesOfNewUSP;
            }
        }

        return Vector2.zero;
    }
    



}
