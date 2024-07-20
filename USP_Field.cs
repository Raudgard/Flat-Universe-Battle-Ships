using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  руглое поле, содержащее некоторое количество USP
/// </summary>
public class USP_Field : MonoBehaviour
{
    //public Vector2 centerOfField;
    public float radiusOfField;
    public int USPCount;
    private WaitForFixedUpdate fixUpdate = new WaitForFixedUpdate();
    //private Global_Controller global_Controller;
    //private BattleSceneController battle_Scene_Controller;

    public bool isInitialized = false;


    private void Awake()
    {
        //global_Controller = Global_Controller.Instance;
        //battle_Scene_Controller = BattleSceneController.Instance;
    }

    /// <summary>
    /// —оздает указанное количество USP в заданной области
    /// </summary>
    public void Initialize()
    {
        if (!isInitialized)
        {
            StartCoroutine(CreateUSPs());
            isInitialized = true;
        }
    }

    private IEnumerator CreateUSPs()
    {
        Vector2 place;
        var timeDelay = new WaitForSeconds(References.Instance.settings.timeBetweenUSPCreationWhileHolding);

        for (int i = 0; i < USPCount; USPCount--)
        {
            place = (Vector2)transform.position + Random.insideUnitCircle * radiusOfField;
            USP.Create(place, 0, false);

            if(Application.isPlaying)
            {
                yield return timeDelay;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusOfField);
    }


}
