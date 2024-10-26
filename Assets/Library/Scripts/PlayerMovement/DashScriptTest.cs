using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScriptTest : MonoBehaviour
{
    //3 states: Not Dashing, Dashing, Overheat
    //in Not dashing state, regen charge
    //in Dashing state, stop regen, dash and countdown cooldown
    //in Overheat state, set timer to overheatCooldown
    [Header("Dash Stack")]
    [SerializeField] bool isDashing = false;
    [SerializeField] int maxCharge = 3;
    [SerializeField] int currentCharge;

    [Header("Dash Stat")]
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float regenCooldown = 2.0f;
    [SerializeField] private float overheatCooldown = 5.0f;

    private bool canDash = true;
    private bool isOverheated = false;
    private Coroutine regenCoroutine;

    private void Start()
    {
        currentCharge = maxCharge;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentCharge > 0) //do dash
        {
            StartCoroutine(Dash());
        }

        if (regenCoroutine == null)  //Regen whenever not dashing
        {
            regenCoroutine = StartCoroutine(RegenCharge());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        currentCharge--;

        //DASH GOES HERE

        //Overheat check
        if (currentCharge == 0)
        {
            isOverheated = true;
            StartCoroutine(HandleOverheat());
            yield return null; //assign cooldown to Overheat instead
        }

        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
        canDash = true;
    }

    private IEnumerator RegenCharge()
    {
        yield return new WaitForSeconds(regenCooldown);
        if(currentCharge < maxCharge && !isOverheated) //avoid overload
        {
            currentCharge++;
        }
        regenCoroutine = null;  // Reset coroutine reference when regen is done
    }

    private IEnumerator HandleOverheat()
    {
        yield return new WaitForSeconds(overheatCooldown);

        yield return new WaitForSeconds(dashCooldown); //assigned cooldown to Overheat
        isDashing = false;
        canDash = true;

        isOverheated = false;
    }
}


