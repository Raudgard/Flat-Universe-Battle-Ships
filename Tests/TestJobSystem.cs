using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using System.Linq;
using Unity.Mathematics;

public class TestJobSystem : MonoBehaviour
{
    public int arraySize;
    public int iterationsCount;
    public int batchCount;
    public bool startCalcWithOUTJobs = false;
    public bool startCalcWithJobsParallel = false;
    public bool startCalcWithJob = false;

    private Coroutine calcWithOUTJobs;
    private Coroutine calcWithJobsParallel;
    private Coroutine calcWithJob;



    private int[] TestArray;

    private void Update()
    {
        if (startCalcWithOUTJobs && calcWithOUTJobs == null) StartCalculationWithOUTJobs();
        if (startCalcWithJobsParallel && calcWithJobsParallel == null) StartCalculationWithJobsParallel();
        if (startCalcWithJob && calcWithJob == null) StartCalculationWithJob();


    }

    private void StartCalculationWithOUTJobs()
    {
        calcWithOUTJobs = StartCoroutine(Testing_WithOUT_Jobs());
    }

    private void StartCalculationWithJobsParallel()
    {
        calcWithJobsParallel = StartCoroutine(Testing_With_Jobs_Parralel());
    }

    private void StartCalculationWithJob()
    {
        calcWithJob = StartCoroutine(Testing_With_Job());
    }

    private IEnumerator Testing_WithOUT_Jobs()
    {
        yield return new WaitForSeconds(1f);
        TestArray = new int[arraySize];
        for (int i = 0; i < TestArray.Length; i++)
        {
            TestArray[i] = i;
        }

        float startTime = Time.realtimeSinceStartup;

        int result = 0;

        for(int i = 0; i < TestArray.Length; i++)
        {
            for(int j = 0; j < iterationsCount; j++)
            {
                if (j % 2 == 0) TestArray[i] *= TestArray[i];
                else TestArray[i] = (int)Mathf.Sqrt(TestArray[i]);
            }

            result += TestArray[i];
        }

        print("result withOUT jobs: " + result);
        float endTime = Time.realtimeSinceStartup;
        //print("End calculation! Time: " + endTime);
        print("The work took time: " + (endTime - startTime));
        startCalcWithOUTJobs = false;
        calcWithOUTJobs = null;
    }


    [BurstCompile]
    private IEnumerator Testing_With_Jobs_Parralel()
    {
        yield return new WaitForSeconds(1f);
        TestArray = new int[arraySize];
        for (int i = 0; i < TestArray.Length; i++)
        {
            TestArray[i] = i;
        }

        float startTime = Time.realtimeSinceStartup;

        NativeArray<int> result = new NativeArray<int>(TestArray, Allocator.TempJob);
        NativeArray<int> array = new NativeArray<int>(TestArray, Allocator.TempJob);

        var job = new TestJobParallelFor
        {
            Result = result,
            Array = array,
            iterationsCount = this.iterationsCount
        };

        JobHandle handle = job.Schedule(TestArray.Length, batchCount);

        int frames = 0;
        if(!handle.IsCompleted)
        {
            yield return null;
            frames++;
            print("frames: " + frames);
        }
        // Wait for the job to complete
        handle.Complete();
        //foreach (var res in results)
        //    print("output result: " + res);
        int res = 0;
        for(int i = 0; i < result.Length; i++)
        {
            res += result[i];
        }




        print("result with jobs Parallel: " + res);
        array.Dispose();
        result.Dispose();

        float endTime = Time.realtimeSinceStartup;
        print("The work took time: " + (endTime - startTime));
        startCalcWithJobsParallel = false;
        calcWithJobsParallel = null;
    }

    [BurstCompile]
    public struct TestJobParallelFor : IJobParallelFor
    {
        [ReadOnly] public NativeArray<int> Array;
        //[NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<int> Result;
        //[ReadOnly] public NativeArray<int> iterationsCount;
        public int iterationsCount;

        public void Execute(int index)
        {
            int temp = Array[index];
            for (int j = 0; j < iterationsCount; j++)
            {
                if (j % 2 == 0) temp *= temp;
                else temp = (int)Mathf.Sqrt(temp);
            }
            Result[index] = temp;
        }
    }






    [BurstCompile]
    private IEnumerator Testing_With_Job()
    {
        yield return new WaitForSeconds(1f);
        TestArray = new int[arraySize];
        for (int i = 0; i < TestArray.Length; i++)
        {
            TestArray[i] = i;
        }

        float startTime = Time.realtimeSinceStartup;

        NativeArray<int> array = new NativeArray<int>(TestArray, Allocator.Temp);
        NativeArray<int> result = new NativeArray<int>(1, Allocator.Temp);

        //NativeArray<int> iterationsCount = new NativeArray<int>(1, Allocator.TempJob);
        //iterationsCount[0] = this.iterationsCount;

        new TestJob
        {
            Result = result,
            Array = array,
            iterationsCount = this.iterationsCount
        }.Execute(); 
        

        //int frames = 0;
        //if (!handle.IsCompleted)
        //{
        //    yield return null;
        //    frames++;
        //    print("frames: " + frames);
        //}
        //// Wait for the job to complete
        //handle.Complete();
        //foreach (var res in results)
        //    print("output result: " + res);

        //int res = 0;
        //for (int i = 0; i < result.Length; i++)
        //{
        //    res += result[i];
        //}




        print("result with job: " + result[0]);
        array.Dispose();
        result.Dispose();

        float endTime = Time.realtimeSinceStartup;
        print("The work took time: " + (endTime - startTime));
        startCalcWithJob = false;
        calcWithJob = null;
    }



    [BurstCompile]
    public struct TestJob : IJob
    {
        [ReadOnly] public NativeArray<int> Array;
        public NativeArray<int> Result;
        public int iterationsCount;


        public void Execute()
        {
            //int[] tempArray = new int[Array.Length];
            for(int i = 0; i < Array.Length; i++)
            {
                int temp = Array[i];

                for (int j = 0; j < iterationsCount; j++)
                {
                    if (j % 2 == 0) temp *= temp;
                    else temp = (int)Mathf.Sqrt(temp);
                }
                Result[0] += temp;
            }
        }
    }
}
