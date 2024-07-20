using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice_anim_controller : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        if(transform.parent.TryGetComponent(out Ship ship) && ship.State != Ship.States.STUNNED)
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
    }

    


    public void StopStunAnimation()
    {
        animator.speed = 0;
    }

    public void Resume()
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
        else
        {
            Debug.LogWarning("Animator in null");
        }
    }

    public void EndOfAnimation()
    {
        Destroy(gameObject);
    }
}
