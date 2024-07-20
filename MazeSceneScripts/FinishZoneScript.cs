using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishZoneScript : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public int entranceSide; // сторона, с которой идет вход в финишную зону. 1 - сверху, 2 - справа, 3 - снизу, 4 - слева.

    private void Start()
    {
        StartCoroutine(GetBoxCollider());
    }

    /// <summary>
    /// Получаем ссылку на BoxCollider2D. В корутине на тот случай, если финишная зона не будет готова в первом кадре.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetBoxCollider()
    {
        while(boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            yield return null;
        }

        ResizeBoxCollider();
    }

    private void ResizeBoxCollider()
    {
        boxCollider.size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y);
        MoveBoxCollider();
    }

    private void MoveBoxCollider()
    {
        switch (entranceSide)
        {
            case 1: //вход сверху
                boxCollider.transform.Rotate(new Vector3(0, 0, 90));
                boxCollider.offset = new Vector2(boxCollider.offset.x - boxCollider.size.y / 4, boxCollider.offset.y);
                break;
            case 2: //вход справа
                //boxCollider.transform.Rotate(new Vector3(0, 0, 90));
                boxCollider.offset = new Vector2(boxCollider.offset.x - boxCollider.size.y / 4, boxCollider.offset.y);
                break;
            case 3: //вход снизу
                boxCollider.transform.Rotate(new Vector3(0, 0, 90));
                boxCollider.offset = new Vector2(boxCollider.offset.x + boxCollider.size.y / 4, boxCollider.offset.y);
                break;
            case 4: //вход слева
                //boxCollider.transform.Rotate(new Vector3(0, 0, 90));
                boxCollider.offset = new Vector2(boxCollider.offset.x + boxCollider.size.y / 4, boxCollider.offset.y);
                break;
        } 
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("enter");

    }

    
}
