using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace BattleFormation
{
    public class BattleFormationSceneController : MonoBehaviour
    {
        private Global_Controller global_Controller;
        private Ship[] ships;
        public float sizeForShipCollider = 3.0f;
        public float sizeOfBorders;
        public float radiusForSpawnZone;

        private void Start()
        {
            global_Controller = Global_Controller.Instance;
            StartCoroutine(CheckForLoadSceneAndResizeCamera());
            CreateBorders();
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + radiusForSpawnZone, -10);

            ships = global_Controller.CreateShips(SaveUtility.SaveUtil.dataOfPlayer);
            //for (int i = 0; i < ships.Length; i++)
            //    ships[i].State = Ship.States.IN_HANGAR;

            //initialSizesOfColliders = new float[ships.Length];

            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i] != null)
                {
                    ships[i].gameObject.AddComponent<Dragable>();

                    CircleCollider2D shipCollider = ships[i].mainCollider;
                    if (shipCollider.radius < sizeForShipCollider)
                        shipCollider.radius = sizeForShipCollider;

                }
            }

        }


        private void CreateBorders()
        {
            var battlefield = new Battlefield(ShapeOfBattlefield.Circle, sizeOfBorders, 90);
            var points = new Vector3[battlefield.Points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector3(battlefield.Points[i].x, battlefield.Points[i].y /*- radiusForSpawnZone*/, -1);
            }

            var borders = GameObject.Find("Borders");
            var edgeRadius = borders.GetComponent<EdgeCollider2D>().edgeRadius = sizeOfBorders - radiusForSpawnZone;

            var pointsForCollider = new Vector2[points.Length + 1];
            for (int i = 0; i < pointsForCollider.Length - 1; i++)
            {
                pointsForCollider[i] = points[i];
            }

            pointsForCollider[pointsForCollider.Length - 1] = pointsForCollider[0];
            //pointsForCollider[pointsForCollider.Length - 1] = pointsForCollider[0];
            var collider = borders.GetComponent<EdgeCollider2D>();
            collider.SetPoints(pointsForCollider.ToList());

            battlefield = new Battlefield(ShapeOfBattlefield.Circle, sizeOfBorders - edgeRadius + borders.GetComponent<LineRenderer>().widthMultiplier / 2, 90);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector3(battlefield.Points[i].x, battlefield.Points[i].y /*- radiusForSpawnZone*/, -1);
            }

            var pointsForLineRenderer = new Vector3[points.Length * 2];
            points.CopyTo(pointsForLineRenderer, 0);
            for (int i = points.Length; i < pointsForLineRenderer.Length; i++)
            {
                pointsForLineRenderer[i] = points[i - points.Length];
            }

            var lineRenderer = borders.GetComponent<LineRenderer>();
            lineRenderer.positionCount = pointsForLineRenderer.Length;
            lineRenderer.SetPositions(pointsForLineRenderer);



        }




        private IEnumerator CheckForLoadSceneAndResizeCamera()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            while (activeScene != SceneManager.GetSceneByName("BattleFormation"))
            {
                activeScene = SceneManager.GetActiveScene();
                yield return null;
            }
            //Player_Data.ResizeCamera();
        }

        public void OnBackButtonClick()
        {
            for (int i = 0; i < ships.Length; i++)
            {
                ships[i].startingPosition = ships[i].transform.position;
            }

            //SaveUtility.SaveUtil.playerShips = ships.ToList();
            Player_Data.Instance.playerShips = ships;
            SaveUtility.SaveUtil.Save();

            StartCoroutine(LoadNewSceneAsync());
        }

        private IEnumerator LoadNewSceneAsync()
        {
            Scene thisScene = SceneManager.GetActiveScene();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            SceneManager.UnloadSceneAsync(thisScene);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(LoadNewSceneAsync());
            }
        }



    }
}