using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Dash(bool isForward)
    {
        animator.SetBool("IsDashForward", isForward);
    }

    public void Parry()
    {
        animator.SetTrigger("Parry");
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackType", Random.Range(0, 3));
    }

    public void Move(float x, float y)
    {
        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }

    public void RotateUpperBody(Transform playerLowerSpine, Vector3 mousePos)
    {
        playerLowerSpine.LookAt(transform.position + mousePos);
    }
}
