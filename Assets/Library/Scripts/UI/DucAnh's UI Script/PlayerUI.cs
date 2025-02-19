using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{
    [Header("Reference")]
    //[SerializeField] private Image uiFillImage; // Will use later
    //[SerializeField] private TextMeshProUGUI levelText; // May or may not use in the future 
    [SerializeField] private Image[] playerHearts;


    [Header("Values")]
    private int level = 1;
    private int expRequirement = 5;
    private int expCurrent = 0;
    private bool increaseReq = true;


    [Header("Coroutine Stuff")]
    private Coroutine heartCoroutine = null;

    private void OnEnable()
    {
        PlayerBase.HealthModified += UpdateHealth;
        //PlayerBase.HBFull +=  
    }

    private void OnDisable()
    {
        PlayerBase.HealthModified -= UpdateHealth;
    }

    private void Awake()
    {
        
    }

    public void AddEXP() 
    {
        //expCurrent++;

        //if (level % 2 == 0 && increaseReq)
        //{
        //    increaseReq = false;
        //    expRequirement += 1;
        //}
        //else if (level % 2 != 0)
        //{
        //    increaseReq = true;
        //}

        //if (expCurrent == expRequirement)
        //{
        //    level += 1;
        //    expCurrent = 0;
        //}

        //Debug.Log(expCurrent + " / " + expRequirement + " [Level: " + level + "] ");
    }

    //private void UpdateProgressFill(float value)
    //{
    //    float curVelocity = 0f;
    //    float tempValue = Mathf.SmoothDamp(uiFillImage.fillAmount, value, ref curVelocity, 0.015f);
    //    uiFillImage.fillAmount = tempValue;
    //}

    private void Update()
    {
        //levelText.text = "LVL: " + level;

        float progressValue = Mathf.InverseLerp(0f, expRequirement, expCurrent);
        //UpdateProgressFill(progressValue);
    }

    private void UpdateHealth(float modifiedHealth, bool? increased)
    {
        if (heartCoroutine == null)
            heartCoroutine = StartCoroutine(ModifyHearts(modifiedHealth, increased));
    }

    private IEnumerator ModifyHearts(float modifiedHealth, bool? increased)
    {
        Debug.Log(increased);

        if (increased == true)
        {

            if (modifiedHealth <= 5)
            {
                Debug.Log("Player's Health: " + modifiedHealth);
                for (int i = 0; i <= modifiedHealth - 1; i++)
                {
                    if (!playerHearts[i].gameObject.activeSelf)
                        playerHearts[i].gameObject.SetActive(true);

                    yield return null;
                }
            }
        }

        else if (increased == false)
        {

            if (modifiedHealth > 0)
            {
                Debug.Log("Player's Health: " + modifiedHealth);    
                for (int i = 4; i >= modifiedHealth; i--)
                {
                    playerHearts[i].gameObject.SetActive(false);

                    yield return null;
                }
            }

        }

        heartCoroutine = null;
    }
}
