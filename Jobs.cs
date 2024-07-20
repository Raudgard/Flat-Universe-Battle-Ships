using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using System.Linq;
using Unity.Mathematics;

namespace JOBS
{
    /// <summary>
    /// Предоставляет различные структуры для многопоточных вычислений.
    /// </summary>
    public class Jobs
    {

        /// <summary>
        /// Проверяет, находятся ли корабли в радиусе обзора данного корабля.
        /// </summary>
        [BurstCompile]
        public struct CheckShipsInVisionRadiusJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector2> shipPosition;
            [ReadOnly] public NativeArray<Vector3> otherShipsPositions;
            [ReadOnly] public float shipVisionRadius;
            [ReadOnly] public NativeArray<float> otherShipRadiusSizes;
            [WriteOnly] public NativeArray<byte> results;

            public void Execute(int index)
            {
                var distanceSqr = (shipPosition[0] - (Vector2)otherShipsPositions[index]).sqrMagnitude;
                float dist = shipVisionRadius + otherShipRadiusSizes[index];
                results[index] = (byte)math.select(1, 0, distanceSqr > dist * dist);
                //print("into job result: " + results[index]);
            }
        }




        /// <summary>
        /// Проверяет, находится ли enemyID в IDsInListOfEnemys.
        /// </summary>
        [BurstCompile]
        public struct CheckContainsEnemyInListJob_old : IJobParallelFor
        {
            [ReadOnly] public int enemyID;
            [ReadOnly] public NativeArray<NativeArray<int>> IDsInListOfEnemys;
            [WriteOnly] public NativeArray<byte> results;

            public void Execute(int index)
            {
                //bool res = IDsInListOfEnemys[index].Contains(enemyID);
                //bool res = false;
                results[index] = 0;
                var listOfEnemys = IDsInListOfEnemys[index];
                for (int i = 0; i < listOfEnemys.Length; i++)
                {
                    if (listOfEnemys[i] == enemyID)
                    {
                        results[index] = 1;
                        break;
                    }
                }

                //results[index] = (byte)math.select(1, 0, res);
                //print("into job result: " + results[index]);
            }
        }

        /// <summary>
        /// Проверяет, находится ли enemyID в IDsInListOfEnemys.
        /// </summary>
        [BurstCompile]
        public struct CheckContainsEnemyInListJob : IJob
        {
            [ReadOnly] public int enemyID;
            [ReadOnly] public NativeArray<int> IDsInListOfEnemys;
            [WriteOnly] public NativeArray<byte> result;

            public void Execute()
            {
                result[0] = 0;
                for (int i = 0; i < IDsInListOfEnemys.Length; i++)
                {
                    if (IDsInListOfEnemys[i] == enemyID)
                    {
                        result[0] = 1;
                        break;
                    }
                }
            }
        }






        [BurstCompile]
        private struct CheckShipsStatusJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Ship.States> shipStates;
            [WriteOnly] public NativeArray<byte> results;

            public void Execute(int index)
            {
                results[index] = (byte)math.select(1, 0, shipStates[index] == Ship.States.IDLE || shipStates[index] == Ship.States.TO_USP);

                //print("into job result: " + results[index]);
            }
        }


    }
}
