using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            transform.position += (transform.forward * animator.GetFloat("MoveSpeed")) * Time.deltaTime;
        }
    }
}
