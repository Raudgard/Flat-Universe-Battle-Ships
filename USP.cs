using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;

//[RequireComponent(typeof(ForbiddenToCreatingUSPWhenClickOnThisObject))]
public class USP : MonoBehaviour
{
    private TrailRenderer USPTrail = null;
    public bool isMoving = false;//переменная введена для гена Telekinesis, чтоб притягивать мог только 1 микрот
    public int team; //чья команда создала эту еду

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Ship ship))
            return;

        if (ship != null && ship.State == Ship.States.TO_USP)
        {
            //Debug.Log("USP OnTriggerEnter2D");
            USPTaken(ship);
        }
    }

    public void USPTaken(Ship ship)
    {
        References.Instance.reusableObjects.AddAndInactivateUSP(this);
        if (BattleSceneController.Instance.freeUSPs.Contains(this))
        {
            BattleSceneController.Instance.freeUSPs.Remove(this);
            //Debug.Log($"BattleSceneController.Instance.freeUSPs.Remove(this). freeUSPs.count: {BattleSceneController.Instance.freeUSPs.Count}");
        }

        ship.USP_taken();
        EventManager.Instance.PostNotification(EVENT_TYPE.USP_TAKEN, this, this);

        //Destroy(gameObject);
    }

    /// <summary>
    /// Создает новый объект USP.
    /// </summary>
    /// <param name="withNotification"></param>
    /// <returns></returns>
    public static USP Create(Vector2 position, int team, bool withShockwave = true)
    {
        USP _newUSP;
        if (References.Instance.reusableObjects.TryToGetUSP(out USP usp))
        {
            _newUSP = usp;
        }
        else
        {
            _newUSP = Instantiate(Prefabs.Instance.uspPrefab);
            _newUSP.transform.parent = BattleSceneController.Instance?.USPs_Transform;
        }

        _newUSP.transform.position = new Vector3(position.x, position.y, -0.5f);
        _newUSP.team = team;

        if (BattleSceneController.Instance.isNeedTrailForUSP && _newUSP.USPTrail == null)
        {
            var uspTrail = Instantiate(Prefabs.Instance.USPTrail);
            uspTrail.transform.SetParent(_newUSP.transform);
            uspTrail.transform.localPosition = Vector3.zero;
            _newUSP.USPTrail = uspTrail;
        }

        BattleSceneController.Instance.freeUSPs.Add(_newUSP);
        EventManager.Instance.PostNotification(EVENT_TYPE.USP_CREATED, _newUSP, position);

        if(withShockwave)
        {
            var shockwave = Instantiate(Prefabs.Instance.shockWaveFromUSPCreation);
            //shockwave.transform.parent = _newUSP.transform;
            shockwave.transform.position = position;
            shockwave.transform.localScale = Vector3.one;

            Global_Controller.Instance.ShockwaveFromCreatedUSP(_newUSP);
        }
        return _newUSP;
    }

}
