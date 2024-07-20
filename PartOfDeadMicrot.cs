using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartOfDeadShip : MonoBehaviour
{
    Vector2 centerOfDeadShip;
    private float speed; // регулируется в Player_Data.Instance.partOfDeadShipSpeed
    private float reductionSpeed;
    //public float timeOfMoving = 1;

    void Start()
    {
        //speed = Player_Data.Instance.partOfDeadShipSpeed;
        //reductionSpeed = Player_Data.Instance.partOfDeadShipSpeedReduction;
        centerOfDeadShip = transform.parent.transform.position;

        StartCoroutine(Moving());
        StartCoroutine(Vanishing());
    }

    private IEnumerator Moving()
    {
        Vector2 direction = (Vector2)transform.position - centerOfDeadShip;

        while(gameObject != null)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            speed *= reductionSpeed;
            yield return null;
        }
    }

    private IEnumerator Vanishing()
    {
        yield return new WaitForSeconds(1);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        while(spriteRenderer.color.a > 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - Time.deltaTime);
            yield return null;
        }

        if (transform.parent.gameObject != null)
            Destroy(transform.parent.gameObject);
    }
}
