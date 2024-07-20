using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Выполняет функции, необходимые непосредственно для ScirmishScene
/// </summary>
public class ScirmishSceneController : MonoBehaviour
{
    [SerializeField] private BattleSceneUIController battleSceneUIController;
    
    private Battlefield battlefield;
    private GameObject enlargerUSPForPlayer;
    private GameObject enlargerUSPForAI;



    private void Awake()
    {
        CreateUSPEnlarger("player");
        CreateUSPEnlarger("AI");
        CreateBattlefield();
        Global_Controller.Instance.SetShipsMaxCapacityFromSettings();
    }

    private void Start()
    {
        CreateShipsForBattle();
        CreatePowerBars();


    }

    private void CreateShipsForBattle()
    {
        //GameObject[] shipsForBattle = new GameObject[Player_Data.Instance.shipsDontDestroy.Length];
        var shape = Player_Data.Instance.shapeOfBattlefield;
        var shipsForBattle = new List<Ship>();
        var playerShips = Global_Controller.Instance.CreateShips(SaveUtility.SaveUtil.dataOfPlayer);
        var AIShips = Global_Controller.Instance.CreateShips(SaveUtility.SaveUtil.dataOfAI);
        shipsForBattle.AddRange(playerShips);
        shipsForBattle.AddRange(AIShips);


        for (int i = 0; i < shipsForBattle.Count; i++)
        {
            Global_Controller.Instance.AddShip(shipsForBattle[i]);

            //временно
            if (shipsForBattle[i].GetComponent<Ship>().playerNumber == 1)
            {
                if (shape == ShapeOfBattlefield.Triangle)
                {
                    shipsForBattle[i].transform.position = shipsForBattle[i].startingPosition + (Vector3)battlefield.SpawnPointsFor3And6Players[0];
                }
                else
                {
                    shipsForBattle[i].transform.position = shipsForBattle[i].startingPosition + (Vector3)battlefield.SpawnPointsFor2And4And8Players[0];
                }
            }
            else
            {
                if (shape == ShapeOfBattlefield.Triangle)
                {
                    shipsForBattle[i].transform.position = shipsForBattle[i].startingPosition + (Vector3)battlefield.SpawnPointsFor3And6Players[1];
                }
                else
                {
                    shipsForBattle[i].transform.position = shipsForBattle[i].startingPosition + (Vector3)battlefield.SpawnPointsFor2And4And8Players[1];
                }
            }

        }

        Camera.main.transform.position = Global_Controller.Instance.ships[1].Count > 0 ?
            new Vector3(Global_Controller.Instance.ships[1][0].transform.position.x, Global_Controller.Instance.ships[1][0].transform.position.y, -10) :
            new Vector3(0, 0, -10);
    }

    private void CreatePowerBars()
    {
        var powerBar1 = Instantiate(Prefabs.Instance.powerBar);
        powerBar1.rectTransform.SetParent(battleSceneUIController.mainCanvas.transform);
        powerBar1.Initialize(1);
        var powerBar2 = Instantiate(Prefabs.Instance.powerBar);
        powerBar2.rectTransform.SetParent(battleSceneUIController.mainCanvas.transform);
        powerBar2.Initialize(2);

    }


    private void CreateBattlefield()
    {
        //min length for Cross shape is 6!

        battlefield = new Battlefield(Player_Data.Instance.shapeOfBattlefield, Player_Data.Instance.sizeOfBattleField, Player_Data.Instance.CountOfPointForBorders, Player_Data.Instance.capsuleLength);
        var borders = Instantiate(Prefabs.Instance.bordersPrefab);
        //switch(shapeOfBattlefield)
        {
            //case ShapeOfBattlefield.Circle:

            var points = battlefield.Points;

            var pointsForLineRenderer = new Vector3[points.Length * 2];
            points.CopyTo(pointsForLineRenderer, 0);
            for (int i = points.Length; i < pointsForLineRenderer.Length; i++)
            {
                pointsForLineRenderer[i] = points[i - points.Length];
            }
            var lineRenderer = borders.GetComponent<LineRenderer>();
            lineRenderer.positionCount = pointsForLineRenderer.Length;
            lineRenderer.SetPositions(pointsForLineRenderer);
            if (Player_Data.Instance.shapeOfBattlefield == ShapeOfBattlefield.Cross)
            {
                lineRenderer.numCornerVertices = 8;
            }

            var pointsForCollider = new Vector2[points.Length + 1];
            for (int i = 0; i < pointsForCollider.Length - 1; i++)
            {
                pointsForCollider[i] = points[i];
            }
            pointsForCollider[pointsForCollider.Length - 1] = pointsForCollider[0];
            //pointsForCollider[pointsForCollider.Length - 1] = pointsForCollider[0];
            var collider = borders.GetComponent<EdgeCollider2D>();
            collider.SetPoints(pointsForCollider.ToList());

            CreateColliderForClickController(points);

            //break;
        }
    }

    private void CreateColliderForClickController(Vector3[] points)
    {
        var pointsForCollider = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            pointsForCollider[i] = points[i];
        }

        var clickController = FindObjectOfType<ClickController>();
        var poligonCollider = clickController.GetComponent<PolygonCollider2D>();
        poligonCollider.SetPath(0, pointsForCollider);
    }


    private void CreateUSPEnlarger(string target)
    {
        if (target == "player")
        {
            enlargerUSPForPlayer = new GameObject();
            enlargerUSPForPlayer.name = "Player USP Enlarger";
            enlargerUSPForPlayer.tag = "PlayerUSPEnlarger";
            var enlarger = enlargerUSPForPlayer.AddComponent<USPEnlarger>();
            enlarger.Cap = Player_Data.Instance.USPCap;
            enlarger.CurrentQuantity = Player_Data.Instance.startingQuantityUSPInBattle;
            enlarger.EnlargingSpeed = Player_Data.Instance.USPEnlargingSpeed;
            enlarger.enlargingSpeedCoeff = Player_Data.Instance.enlargingUSPspeedCoeff;
        }
        else if (target == "AI")
        {
            enlargerUSPForAI = new GameObject();
            enlargerUSPForAI.name = "AI USP Enlarger";
            enlargerUSPForAI.tag = "AI_USPEnlarger";
            var enlarger = enlargerUSPForAI.AddComponent<USPEnlarger>();
            var AI = GetComponent<AI>();
            enlarger.Cap = AI.USPCap;
            enlarger.CurrentQuantity = AI.startingQuantityUSPInBattle;
            enlarger.EnlargingSpeed = AI.USPEnlargingSpeed;
            enlarger.enlargingSpeedCoeff = AI.enlargingUSPspeedCoeff;

        }
    }




    /// <summary>
    /// Метод создания еды в центре поля, если вся еда закончилась
    /// </summary>
    //public IEnumerator EmergencyCreatingUSP()
    //{
    //    while (gameObject != null)
    //    {
    //        if (Player_Data.Instance.USPCountInBattle <= 0 && freeUSPs.Count == 0)
    //        {
    //            print("еды нет, создаю");
    //            USP.Create(Random.insideUnitCircle * 4, 0);//создаем еду
    //        }
    //        else
    //        {
    //            //print("еда еще где-то есть...");
    //        }

    //        yield return new WaitForSeconds(2);
    //    }
    //}





}
