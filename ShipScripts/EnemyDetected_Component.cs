using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Burst;
using System.Linq;
using UnityEngine.SceneManagement;




/// <summary>
/// Отвечает за получение и отправку информации об обнаруженных врагах, а также за список обнаруженных врагов.
/// </summary>
public class EnemyDetected_Component : MonoBehaviour
{
    //public Action<Ship> Enemy_detected;
    //public List<Ship> enemysDetected = new();
    private Ship ship;
    private Global_Controller global_Controller;
    private GameEngineAssistant gameEngineAssistant;
    private Coroutine transmittingInfoCoroutine = null;
    [SerializeField] private float delayBeforeNextTransmitInfoInSeconds;
    //private static LayerMask shipsLayer;

    void Start()
    {
        //shipsLayer = LayerMask.GetMask("ShipLayer");
        ship = GetComponent<Ship>();
        global_Controller = Global_Controller.Instance;
        gameEngineAssistant = References.Instance.gameEngineAssistant;
        ship.Enemys_detected += EnemyDetected;
        var sceneName = SceneManager.GetActiveScene().name;
        //if (sceneName == "BattleScene" || sceneName == "Maze")
            //StartCoroutine(UpdateListsOfEnemys());

    }




    /// <summary>
    /// Метод, ответственный за реакцию на получение информации о расположении врага, и дальнейшем ее распространении.
    /// </summary>
    /// <param name="enemyShips">Вражеский корабль</param>
    /// <param name="shipsWhoKnows">Союзники, которые уже знают об этом корабле</param>
    private void EnemyDetected(List<Ship> enemyShips, List<Ship> shipsThatHaveBeenInformed)
    {
        //if (enemyShips == null) return;
        var enemies = enemyShips.Except(enemyShips.Where(ship => ship == null && ship.IsInvisible)).ToList();
        ship.discoveredEnemies = ship.discoveredEnemies.Union(enemies).ToList();

        //if (!ship.alreadyDiscoveredEnemies.Contains(enemyShips))
        //{
        //    ship.alreadyDiscoveredEnemies.Add(enemyShips);
        //}



        //int ID = enemyShip.GetInstanceID();
        //if (!ship.enemysDetectedIDs.Contains(ID))
        //{
        //    ship.enemysDetectedIDs.Add(ID);
        //}

        if (transmittingInfoCoroutine == null)
        {
            //print("3. transmit info about enemy. ship: " + ship.GetInstanceID());
            //transmittingInfoCoroutine = StartCoroutine(TransmittingInfoAboutEnemy(enemyShip));


            shipsThatHaveBeenInformed.Add(ship);
            transmittingInfoCoroutine = StartCoroutine(TransmittingInfoAboutEnemyParallel(enemies, shipsThatHaveBeenInformed));
        }
    }


    ///// <summary>
    ///// Находит все союзные корабли в радиусе обзора, и передает им информацию об обнаруженном враге. Те делают то же самое, поэтому получается по цепочке.
    ///// </summary>
    ///// <param name="enemyShip"></param>
    //private IEnumerator TransmittingInfoAboutEnemy(Ship enemyShip)
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    if (enemyShip == null)
    //    {
    //        transmittingInfoCoroutine = null;
    //        yield break;
    //    }
    //    //print("ship " + ship.GetInstanceID() + ". enemyShip: " + enemyShip.GetInstanceID());

    //    List<Ship> _teamShips = global_Controller.ships[ship.playerNumber];
    //    for (int i = 0; i < _teamShips.Count; i++)
    //    {
    //        //if (_teamShips[i] == this) continue;
    //        if ((_teamShips[i].State == Ship.States.IDLE || _teamShips[i].State == Ship.States.TO_USP) && Global_Controller.DoISeeThisShip(ship, _teamShips[i]))
    //        {
    //            if (!_teamShips[i].enemysDetected.Contains(enemyShip))
    //                _teamShips[i].enemysDetected.Add(enemyShip);

    //            _teamShips[i].Enemy_detected(enemyShip);
    //        }
    //        else if (!_teamShips[i].enemysDetected.Contains(enemyShip) && Global_Controller.DoISeeThisShip(ship, _teamShips[i]))
    //        {
    //            _teamShips[i].enemysDetected.Add(enemyShip);
    //            _teamShips[i].Enemy_detected(enemyShip);
    //        }
    //    }

    //    transmittingInfoCoroutine = null;
    //}


    /// <summary>
    /// Находит все союзные корабли в радиусе обзора, и передает им информацию об обнаруженном враге. Те делают то же самое, поэтому получается по цепочке.
    /// </summary>
    /// <param name="enemyShips"></param>
    [BurstCompile]
    private IEnumerator TransmittingInfoAboutEnemyParallel(List<Ship> enemyShips, List<Ship> shipsThatHaveBeenInformed)
    {
        //if (enemyShip == null) yield break;
        //print("ship " + ship.GetInstanceID() + ". enemyShip: " + enemyShip.GetInstanceID());
        //print($"count of transmitting: {global_Controller.countOfTransmittingInfo}, {global_Controller.canTransmit}");

        //print($"shipsThatHaveBeenInformed: {shipsThatHaveBeenInformed.Count} ");
        //List<Ship> _teamShips = global_Controller.ships[ship.playerNumber].Where(ship => !ship.enemysDetected.Contains(enemyShip)).ToList();
        List<Ship> _teamShips = global_Controller.ships[ship.playerNumber].Except(shipsThatHaveBeenInformed).ToList();
        //print($"_teamShips (who don't know: {_teamShips.Count} ");



        //1. Выясняем, у каких кораблей нет информации об обнаруженном враге.
        //uint jobNumber = gameEngineAssistant.JobsCounter;
        //yield return gameEngineAssistant.StartCoroutine(gameEngineAssistant.ContainsEnemyShipInDetectedEnemys(enemyShip, _teamShips, jobNumber));
        //var resultOfJob = gameEngineAssistant.ContainsEnemyShipInDetectedEnemys(enemyShip, _teamShips);

        //2. Выясняем, находятся ли такие корабли (дружественные, у которых нет информации о враге) в радиусе видимости.
        uint jobNumber = gameEngineAssistant.JobsCounter;
        yield return gameEngineAssistant.StartCoroutine(gameEngineAssistant.CheckShipsInVisionRadius(ship, _teamShips, jobNumber));
        var resultOfJob2 = gameEngineAssistant.CheckShipsInVisionRadius_Results[jobNumber];
        WaitForFixedUpdate fixedUpdate = new();

        for (int i = 0; i < resultOfJob2.Count; i++)
        {
            if(resultOfJob2[i] != null) resultOfJob2[i].Enemys_detected(enemyShips, shipsThatHaveBeenInformed);
            yield return fixedUpdate;
            ////if (_teamShips[i] == this) continue;
            //if ((_teamShips[i].State == Ship.States.IDLE || _teamShips[i].State == Ship.States.TO_USP) && Global_Controller.DoISeeThisShip(ship, _teamShips[i]))
            //{
            //    if (!_teamShips[i].enemysDetected.Contains(enemyShip))
            //        _teamShips[i].enemysDetected.Add(enemyShip);

            //    _teamShips[i].Enemy_detected(enemyShip);
            //}
            //else if (!_teamShips[i].enemysDetected.Contains(enemyShip) && Global_Controller.DoISeeThisShip(ship, _teamShips[i]))
            //{
            //    _teamShips[i].enemysDetected.Add(enemyShip);
            //    _teamShips[i].Enemy_detected(enemyShip);
            //}
        }
        //gameEngineAssistant.ContainsEnemyShipInDetectedEnemys_Results.Remove(jobNumber);
        gameEngineAssistant.CheckShipsInVisionRadius_Results.Remove(jobNumber);
        yield return new WaitForSeconds(delayBeforeNextTransmitInfoInSeconds);
        transmittingInfoCoroutine = null;
    }



   











    private IEnumerator UpdateListsOfEnemys()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            for (int i = 0; i < ship.discoveredEnemies.Count; i++)
            {
                if (ship.discoveredEnemies[i] == null)
                {
                    print("проверка сработала! ship = null");
                    ship.discoveredEnemies.RemoveAt(i);
                }
            }

            uint jobNumber = gameEngineAssistant.JobsCounter;
            yield return gameEngineAssistant.StartCoroutine(gameEngineAssistant.CheckShipsInVisionRadius(ship, ship.discoveredEnemies, jobNumber));
            var res = gameEngineAssistant.CheckShipsInVisionRadius_Results[jobNumber];
            Ship[] results = new Ship[res.Count];
            gameEngineAssistant.CheckShipsInVisionRadius_Results[jobNumber].CopyTo(results);

            ship.discoveredEnemies = results.ToList();
            ship.enemysDetectedIDs = results.Select(ship => ship.GetInstanceID()).ToList();

            gameEngineAssistant.CheckShipsInVisionRadius_Results.Remove(jobNumber);
        }
    }









    







    

    



















    


}
