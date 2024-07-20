using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleFormation
{
    public class Dragable : MonoBehaviour
    {
        private Rigidbody2D rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnMouseDrag()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//посылаем луч "в экран"
            rb.MovePosition(ray.origin);
        }



    }
}