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
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            transform.position += (forward * animator.GetFloat("MoveSpeed")) * Time.deltaTime;
        }
    }
}
