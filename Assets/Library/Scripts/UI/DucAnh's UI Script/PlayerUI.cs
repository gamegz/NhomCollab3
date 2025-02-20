using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor.Compilation;

public class PlayerUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Image healBarUI; 
    //[SerializeField] private TextMeshProUGUI levelText; // May or may not use in the future 
    [SerializeField] private Image[] playerHearts;
    private PlayerBase playerBase;
    private Color orgHBColor;
    private Color minColor = Color.red;
    private Color maxColor = Color.green;


    [Header("Values")]
    private Vector3 orgHeartScale = new Vector3(0.85f, 0.85f, 0.85f);
    private int level = 1;
    private int expRequirement = 5;
    private int expCurrent = 0;
    private bool increaseReq = true;


    [Header("Coroutine Stuff")]
    private Coroutine heartCoroutine = null;
    private Coroutine overHealingCoroutine = null;

    private void OnEnable()
    {
        PlayerBase.HealthModified += UpdateHealth;
        PlayerBase.HBOverheal += OverHealingEffect;
    }

    private void OnDisable()
    {
        PlayerBase.HealthModified -= UpdateHealth;
        PlayerBase.HBOverheal -= OverHealingEffect;
    }

    private void Awake()
    {
        orgHBColor = healBarUI.color;

        playerBase = GetComponent<PlayerBase>();
        if (playerBase == null)
        {
            Debug.Log("No player base");
        }
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

    private void UpdateProgressFill(float value)
    {
        float curVelocity = 0f;
        float tempValue = Mathf.SmoothDamp(healBarUI.fillAmount, value, ref curVelocity, 0.0075f);
        healBarUI.fillAmount = tempValue;
    }

    private void Update()
    {
        //levelText.text = "LVL: " + level;

        float? progress = playerBase?.GetHealBarProgress();
        if (progress != null)
        {
            UpdateProgressFill(progress.Value);
        }

    }

    private void OverHealingEffect(bool isOverHealing)
    {
        if (overHealingCoroutine != null)
        {
            StopCoroutine(overHealingCoroutine);
            overHealingCoroutine = null;
        }

        if (overHealingCoroutine == null)
            overHealingCoroutine = StartCoroutine(OverHealingCoroutine(isOverHealing));

        
    }

    private IEnumerator OverHealingCoroutine(bool isOverHealing)
    {
        if (isOverHealing)
        {
            while (healBarUI.color != minColor)
            {
                healBarUI.color = Color.Lerp(minColor, maxColor, playerBase.GetHealBarProgress());
                yield return null;
            }

        }

        if (!isOverHealing)
            healBarUI.color = orgHBColor;

        overHealingCoroutine = null;
    }


    private void UpdateHealth(float modifiedHealth, bool? increased)
    {
        if (heartCoroutine != null)
        {
            StopCoroutine(heartCoroutine);
            heartCoroutine = null;
        }

        if (heartCoroutine == null)
            heartCoroutine = StartCoroutine(ModifyHearts(modifiedHealth, increased));
    }

    private IEnumerator ModifyHearts(float modifiedHealth, bool? increased)
    {
        if (increased == true)
        {
            if (modifiedHealth <= 5)
            {
                Vector3 currentVel = Vector3.zero;

                for (int i = 0; i <= modifiedHealth - 1; i++)
                {
                    while (playerHearts[i].transform.localScale != orgHeartScale)
                    {
                        Vector3 tempScale = Vector3.SmoothDamp(playerHearts[i].transform.localScale, orgHeartScale, ref currentVel, 0.05f);
                        playerHearts[i].transform.localScale = tempScale;
                        yield return null;
                    }

                }
            }
        }

        else if (increased == false)
        {
            if (modifiedHealth > 0)
            {
                Vector3 currentVel = Vector3.zero;

                Debug.Log("Player's Health: " + modifiedHealth);    
                for (int i = 4; i >= modifiedHealth; i--)
                {

                    while (playerHearts[i].transform.localScale != Vector3.zero)
                    {
                        Vector3 tempScale = Vector3.SmoothDamp(playerHearts[i].transform.localScale, Vector3.zero, ref currentVel, 0.05f);
                        playerHearts[i].transform.localScale = tempScale;
                        yield return null;
                    }

                }
            }

        }

        heartCoroutine = null;
    }




}
