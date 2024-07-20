using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(ForbiddenToCreatingUSPWhenClickOnThisObject))]
public class PreUSP : MonoBehaviour
{
    [SerializeField] private const float timeToTurningIntoUSP = 10.0f;
    //private BattleSceneController battleSceneController;
    //private Global_Controller global_Data;

    private USP USPPrefab;

    public float minForce, maxForce;


    void Start()
    {
        StartCoroutine(TurningIntoUSP());
        USPPrefab = Prefabs.Instance.uspPrefab;
    }

    /// <summary>
    /// Метод движения preUSP от микрота
    /// </summary>
    /// <param name="from">позиция микрота (его центр)</param>
    /// <returns></returns>
    public IEnumerator GoingOut(Vector2 from)
    {
        Vector2 direction = ((Vector2)transform.position - from).normalized;
        float force = Random.Range(minForce, maxForce);
        //при силе в 5 и коэфф торможения 0,95 откидывает еду примерно на 2 клетки

        //print("force = " + force);

        Vector2 moving = direction * force;

        //while (moving.sqrMagnitude > 0.1f)
        //{
        //    transform.Translate(moving * Time.deltaTime, Space.World);
        //    //body.velocity = moving * Time.fixedDeltaTime;
        //    moving *= 0.95f;
        //    //print("moveDirection.sqrMagnitude = " + moveDirection.sqrMagnitude);
        //    yield return new WaitForFixedUpdate();
        //}

        var body = GetComponent<Rigidbody2D>();
        body.AddForce(moving, ForceMode2D.Impulse);
        while (body.velocity.sqrMagnitude > 0)
        {
            yield return null;
        }

        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CircleCollider2D>().isTrigger = true;

    }

    private IEnumerator TurningIntoUSP()
    {
        float timeToAppear = 2.0f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float needToFullAlfaChannel = 1 - spriteRenderer.color.a;
        float coeff = needToFullAlfaChannel / timeToAppear;

        yield return new WaitForSeconds(timeToTurningIntoUSP - timeToAppear);

        while (timeToAppear > 0)
        {
            yield return null;
            timeToAppear -= Time.deltaTime;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + coeff * Time.deltaTime);
        }

        //Vector3 positionOfNewUSP = new Vector3(transform.position.x, transform.position.y, -2);


        USP.Create(transform.position, 0, false); //создаем еду на месте PreUSP
        //global_Data.freeUSPs.Add(newUSP);
        //Destroy(global_Data.freeUSPs[global_Data.freeUSPs.Count - 1].GetComponentInChildren<ParticleSystem>().gameObject);
        //global_Data.freeUSPs[global_Data.freeUSPs.Count - 1].GetComponent<Transform>().position = positionOfNewUSP;
        //global_Data.freeUSPs[global_Data.freeUSPs.Count - 1].team = 0;
        //newUSP.transform.position = positionOfNewUSP;
        //newUSP.team = 0;

        if (gameObject != null)
            Destroy(gameObject);
    }
}
