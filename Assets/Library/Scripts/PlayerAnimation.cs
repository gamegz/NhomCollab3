using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //[SerializeField] Animator m_Animator;
    [SerializeField] Transform player_lower_spine;

    //optimization
    Vector2 localMovement;
    Vector2 moveVector2D;
    Vector2 forwardVector2D;
    Vector2 rightVector2D;
    float deltaAngle;
    [SerializeField] private Animator animator;

    //public void Dash(bool isForward)
    //{
    //    animator.SetBool("IsDashForward", isForward);
    //}

    //public void Parry()
    //{
    //    animator.SetTrigger("Parry");
    //}

    public void Attack(int attackType)
    {
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackType", attackType);
    }

    public void Move(float x, float y)
    {
        moveVector2D = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        forwardVector2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        rightVector2D = new Vector2(forwardVector2D.y, -forwardVector2D.x);

        localMovement = new Vector2(Vector2.Dot(moveVector2D, rightVector2D), Vector2.Dot(moveVector2D, forwardVector2D));

        animator.SetFloat("X", localMovement.x);
        animator.SetFloat("Y", localMovement.y);
    }
}
