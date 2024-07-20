using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Unity.Burst;
using Unity.Jobs;


public class USPEnlarger : MonoBehaviour
{
    [SerializeField] private int currentQuantity;
    
    public int CurrentQuantity 
    {
        get { return currentQuantity; }
        set 
        {
            if (value < 0) currentQuantity = 0; 
            else if (value > Cap) currentQuantity = Cap;
            else currentQuantity = value;
        }
    }

    [Tooltip("Коэффициент, на который раз в минуту умножается скорость увеличения USP")]
    public float enlargingSpeedCoeff;


    [SerializeField] private int cap;
    public int Cap { get { return cap; } set { cap = value; } }

    [SerializeField] private float enlargingSpeed;
    public float EnlargingSpeed { get { return enlargingSpeed; } set { enlargingSpeed = value; } }


    void Start()
    {
        //EnlargingUSP();
        //EventManager.Instance.PostNotification(EVENT_TYPE.USP_ENLARGED, this);
        StartCoroutine(USP_Enlarger());
        StartCoroutine(Decreasing_Enlarging_Speed());
    }

    
    private IEnumerator USP_Enlarger()
    {
        while (this != null && gameObject != null)
        {
            float time = (float)(1 / EnlargingSpeed);
            yield return new WaitForSeconds(time);
            CurrentQuantity++;
            EventManager.Instance.PostNotification(EVENT_TYPE.USP_ENLARGED, this);
        }
    }


    private IEnumerator Decreasing_Enlarging_Speed()
    {
        while (this != null && gameObject != null)
        {
            yield return new WaitForSeconds(60);
            EnlargingSpeed *= enlargingSpeedCoeff;
            //print("speed = " + USPEnlargingSpeed);
        }
    }


}
