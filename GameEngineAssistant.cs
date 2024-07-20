using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using JOBS;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;

[BurstCompile]
public class GameEngineAssistant : MonoBehaviour
{
    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        CheckShipsInVisionRadius_Results.Clear();
        CheckShipsInVisionRadius_Results = new Dictionary<uint, List<Ship>>();
        jobsCounter = 0;
    }



    /// <summary>
    /// Returns whether the event occurred with the specified probability percentage or not (¬озвращает, произошло ли событие с указанным процентом веро€тности или нет)
    /// </summary>
    /// <param name="percentage">Percentage of probability of event triggering (ѕроцент веро€тности срабатывани€ событи€)</param>
    /// <returns></returns>
    public static bool GetProbability(float percentage) => UnityEngine.Random.Range(0.0f, 100.0f) <= percentage;

    #region JOBS

    private uint jobsCounter = 0;
    /// <summary>
    /// —четчик заданных работ. ѕредоставл€ет уникальный номер.
    /// </summary>
    public uint JobsCounter
    {
        get 
        {
            //if (jobsCounter == uint.MaxValue - 1) jobsCounter = 0;
            //print("jobsCounter = " + (jobsCounter + 1) + "res number = " + CheckShipsInVisionRadius_Results.Count);
            return ++jobsCounter; 
        }
    }
    /// <summary>
    /// ѕредставл€ет результаты работы поиска кораблей в зоне видимости. uint - уникальный номер выполненной работы. List<Ship> - результат работы.
    /// </summary>
    public Dictionary<uint, List<Ship>> CheckShipsInVisionRadius_Results = new Dictionary<uint, List<Ship>>();

    /// <summary>
    /// ќпредел€ет, какие из списка кораблей (2 аргумент) наход€тс€ у корабл€ (1 аргумент) в радиусе видимости.
    /// </summary>
    /// <param name="ship"> орабль, чей радиус видимости провер€етс€.</param>
    /// <param name="otherShips">—писок кораблей, которых нужно проверить, наход€тс€ они в радиусе видимости у первого корабл€ или нет.</param>
    /// <param name="jobNumber">”никальный номер работы, под которым потом нужно будет искать результат.</param>
    /// <returns></returns>
    [BurstCompile]
    public IEnumerator CheckShipsInVisionRadius(Ship ship, List<Ship> otherShips, uint jobNumber)
    {
        Vector2 position = ship.transform.position;
        var OP = otherShips.Select(otherShip => otherShip.transform.position).ToArray();

        NativeArray<Vector2> shipPosition = new NativeArray<Vector2>(1, Allocator.TempJob);
        shipPosition[0] = position;
        NativeArray<Vector3> enemyPositions = new NativeArray<Vector3>(OP, Allocator.TempJob);
        float visionRadius = ship.VisionRadius;
        NativeArray<float> otherShipRadiusSizes = new NativeArray<float>(otherShips.Select(enemy => enemy.radiusSize).ToArray(), Allocator.TempJob);
        NativeArray<byte> resultsOfJob = new NativeArray<byte>(otherShips.Count, Allocator.TempJob);

        Jobs.CheckShipsInVisionRadiusJob job = new Jobs.CheckShipsInVisionRadiusJob();
        job.shipPosition = shipPosition;
        job.otherShipsPositions = enemyPositions;
        job.shipVisionRadius = visionRadius;
        job.otherShipRadiusSizes = otherShipRadiusSizes;
        job.results = resultsOfJob;

        // Schedule the job with one Execute per index in the results array and only 1 item per processing batch
        JobHandle jobHandle = job.Schedule(resultsOfJob.Length, 4);

        //DisposeWhenQuit(shipPosition, enemyPositions, otherShipRadiusSizes, resultsOfJob);

        //int frames = 0;
        // Wait for the job to complete
        while (!jobHandle.IsCompleted /*&& frames < 5*/)
        {
            //print("Task don't complete yet");
            yield return null;
            //frames++;
        }
        jobHandle.Complete();

        // Free the memory allocated by the arrays
        shipPosition.Dispose();
        enemyPositions.Dispose();
        otherShipRadiusSizes.Dispose();

        List<Ship> results = new List<Ship>();
        //List<int> _IDsOfEnemiesThatAreStillVisible = new List<int>();

        for (int j = 0; j < resultsOfJob.Length; j++)
        {
            if (j > otherShips.Count - 1)
            {
                break;
            }
            if (resultsOfJob[j] == 1 && otherShips[j] != null)
            {
                results.Add(otherShips[j]);
            }
        }
        resultsOfJob.Dispose();

        CheckShipsInVisionRadius_Results.Add(jobNumber, results);
    }

   


    /// <summary>
    /// ѕровер€ет содержитс€ ли ID первого корабл€ в списках обнаруженных врагов у союзных кораблей (2 аргумент).
    /// ¬озвращает список дружественных кораблей, у которых не содержитс€ информаци€ об этом корабле.
    /// </summary>
    /// <param name="enemyShip">¬ражеский корабль, который провер€ют</param>
    /// <param name="alliedShips">ƒружеские корабли, в списке которых провер€ют этот корабль.</param>
    /// <param name="jobNumber">”никальный номер выполн€емой работы.</param>
    /// <returns></returns>
    [BurstCompile]
    public List<Ship> ContainsEnemyShipInDetectedEnemys(Ship enemyShip, List<Ship> alliedShips/*, uint jobNumber*/)
    {
        var enemysIDs = alliedShips.Select(frendShip => frendShip.enemysDetectedIDs).ToArray();
        int enemyShipID = enemyShip.GetInstanceID();
        //NativeArray<NativeArray<int>> IDsInArrayOfDetectedEnemys = new NativeArray<NativeArray<int>>(enemysIDs.Length, Allocator.TempJob);
        List<Ship> results = new List<Ship>();


        for (int i = 0; i < enemysIDs.Length; i ++)
        {
            NativeArray<int> IDsInArrayOfDetectedEnemys = new NativeArray<int>(enemysIDs[i].ToArray(), Allocator.Temp);
            NativeArray<byte> resultsOfJob = new NativeArray<byte>(1, Allocator.Temp);

            //DisposeWhenQuit(resultsOfJob, IDsInArrayOfDetectedEnemys);

            /*Jobs.CheckContainsEnemyInListJob job = */
            new Jobs.CheckContainsEnemyInListJob()
            {
                enemyID = enemyShipID,
                IDsInListOfEnemys = IDsInArrayOfDetectedEnemys,
                result = resultsOfJob
            }.Execute();

            //JobHandle jobHandle = job.Schedule();
            //while (!jobHandle.IsCompleted)
            //{
            //    yield return null;
            //}
            //jobHandle.Complete();

            if (resultsOfJob[0] == 0 /*&& alliedShips[i] != null*/) results.Add(alliedShips[i]);

            IDsInArrayOfDetectedEnemys.Dispose();
            resultsOfJob.Dispose();

        }

        return results;

        //ContainsEnemyShipInDetectedEnemys_Results.Add(jobNumber, results);



        //NativeArray<byte> resultsOfJob = new NativeArray<byte>(enemysIDs.Length, Allocator.TempJob);
        //Jobs.CheckContainsEnemyInListJob job = new Jobs.CheckContainsEnemyInListJob()
        //{
        //    enemyID = enemyShipID,
        //    IDsInListOfEnemys = IDsInArrayOfDetectedEnemys,
        //    results = resultsOfJob
        //};

        //JobHandle jobHandle = job.Schedule(enemysIDs.Length, 4);
        //while (!jobHandle.IsCompleted)
        //{
        //    yield return null;
        //}
        //jobHandle.Complete();


        //List<int> _IDsOfEnemiesThatAreStillVisible = new List<int>();

        //for (int j = 0; j < resultsOfJob.Length; j++)
        //{
        //    if (j > alliedShips.Count - 1)
        //    {
        //        break;
        //    }
        //    if (resultsOfJob[j] == 1 && alliedShips[j] != null)
        //    {
        //        results.Add(alliedShips[j]);
        //    }
        //}
        //resultsOfJob.Dispose();

    }



    //private void DisposeWhenQuit(params IDisposable[] disposables)
    //{
    //    Application.quitting += delegate
    //    {
    //        Debug.Log("quitting");
    //        for(int i = 0; i < disposables.Length; i++)
    //        {
    //            if(disposables[i].)
    //            disposables[i]?.Dispose();
    //        }
    //    };
    //}


    private void DisposeWhenQuit(params IDisposable[] disposables)
    {
        Application.quitting += delegate
        {
            Debug.Log("quitting");
            try
            {
                for (int i = 0; i < disposables.Length; i++)
                {
                    disposables[i].Dispose();
                }
            }
            catch(Exception)
            {
                Debug.Log("NativeArrays already disposed!");
            }
        };
    }





    private void OnDestroy()
    {
        
    }

    #endregion
}
