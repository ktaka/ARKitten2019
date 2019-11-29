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
            Rigidbody rb = GetComponent<Rigidbody>();
            // 子猫が向いている方向に経過時間分だけ動かす
            rb.position += (transform.forward * animator.GetFloat("MoveSpeed")) * Time.deltaTime;
        }
    }
}
