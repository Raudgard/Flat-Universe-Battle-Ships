using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWhenIdle : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private SearchingForUSP searchingForUSP;
    [SerializeField] private float speedIdle;
    private Coroutine crawlingCoroutine = null;

    public void Crawling()
    {
        if (crawlingCoroutine == null)
        {
            crawlingCoroutine = StartCoroutine(CrawlingProcess());
        }
    }


    private IEnumerator CrawlingProcess()
    {
        while (ship.State == Ship.States.IDLE)
        {
            ship.moveDirection = Random.insideUnitCircle * speedIdle;
            int randomNumberOfMovement = Random.Range(100, 500);

            for (int i = 0; ship.State == Ship.States.IDLE && i < randomNumberOfMovement; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        crawlingCoroutine = null;
    }






    //public IEnumerator CrawlingProcess()
    //{
    //    if (ship.State == Ship.States.IDLE)
    //    {
    //        ship.moveDirection = Random.insideUnitCircle * speedIdle;
    //        int randomNumberOfMovement = Random.Range(100, 500);

    //        for (int i = 0; ship.State == Ship.States.IDLE && i < randomNumberOfMovement; i++)
    //        {
    //            //body.velocity = crawling * Time.fixedDeltaTime;
    //            //print("Crawling is working");

    //            //if(searchingForUSP.SearchingNearestUSP(out USP usp))
    //            //{
    //            //    searchingForUSP.SendShipForUSP(usp);
    //            //}

    //            yield return new WaitForFixedUpdate();
    //        }

    //        StartCoroutine(CrawlingProcess());
    //    }
    //}
}
