using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator m_Animator;

    //optimization
    Vector2 localMovement;
    Vector2 moveVector2D;
    Vector2 forwardVector2D;
    Vector2 rightVector2D;
    float deltaAngle;

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    private void AnimationUpdate()
    {
        /*
        //move
        moveVector2D = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        forwardVector2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        rightVector2D = new Vector2(forwardVector2D.y, -forwardVector2D.x);

        localMovement = new Vector2(Vector2.Dot(moveVector2D, rightVector2D), Vector2.Dot(moveVector2D, forwardVector2D));

        m_Animator.SetFloat("xMove", localMovement.x);
        m_Animator.SetFloat("yMove", localMovement.y);

        //turn
        deltaAngle = Vector3.SignedAngle(transform.forward, InputManager.instance.mouseDirection(transform.position), Vector3.up);
        deltaAngle = Mathf.Clamp(deltaAngle, -1, 1);
        m_Animator.SetFloat("Turn", deltaAngle);
        */
    }
}
