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
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Animator healTextAnimator;
    [SerializeField] private Image innerChargeATKBarUI;
    [SerializeField] private Image outerChargeATKBarUI;
    [SerializeField] private Transform heartSpawnPos;
    [SerializeField] private Image overHealHeart; // Prefab reference
    [SerializeField] private Image playerHeart; // Prefab reference
    [SerializeField] private GameObject heartContainer; // Heart's parent (to make things less messy when spawning in hearts)
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image healBarUI;
    private List<Image> heartList = new List<Image>();
    private Image tempOverHealHeart = null;
    private TextMeshProUGUI healText;
    //[SerializeField] private TextMeshProUGUI levelText; // May or may not use in the future 
    private PlayerBase playerBase;
    private Color orgICABcolor; // Original Inner Charge Attack Bar Color  [UI Purpose]
    private Color orgOCABcolor; // Original Outer Charge Attack Bar Color  [UI Purpose] [THIS IS THE PARENT OF "innerChargeATKBarUI"]
    private Color orgHBColor; // Original Heal Bar Color 
    private Color minColor = Color.red;
    private Color maxColor = Color.green;


    [Header("Values")]
    [SerializeField] private float xOffsetHeartPercent = 0.05f;
    private float xOffsetHeartValue = 0f;
    private Vector3 orgHeartScale = new Vector3(0.85f, 0.85f, 0.85f);
    private Vector3 orgCABarScale = new Vector3(1.25f, 4f, 1f); // Original Charged Attack Bar Scale [For any one who couldn't read]
    //private int level = 1;
    //private int expRequirement = 5;
    //private int expCurrent = 0;
    //private bool increaseReq = true;


    [Header("Coroutine Stuff")]
    private Coroutine heartCoroutine = null;
    private Coroutine overHealingCoroutine = null;
    private Coroutine chargeATKCoroutine = null;

    private void Awake()
    {
        xOffsetHeartValue = Screen.width * xOffsetHeartPercent;

        if (healTextAnimator == null)
            Debug.Log("There isn't any Over Heal animator");
        else
        {
            healText = healTextAnimator.GetComponent<TextMeshProUGUI>();
            
            if (healText == null)
            {
                Debug.Log("Heal text wasn't assigned");
            }
        }

        orgICABcolor = new Color(innerChargeATKBarUI.color.r, innerChargeATKBarUI.color.g, innerChargeATKBarUI.color.b, 0f);
        orgOCABcolor = new Color(outerChargeATKBarUI.color.r, outerChargeATKBarUI.color.g, outerChargeATKBarUI.color.b, 0.5f);
        orgHBColor = healBarUI.color;

        //playerHearts[playerHearts.Length - 1].transform.localScale = Vector3.zero;

        playerBase = GetComponent<PlayerBase>();
        if (playerBase == null)
        {
            Debug.Log("No player base");
        }

        if (weaponManager == null)
        {
            Debug.Log("No weapon manager");
        }

        if (mainCamera == null)
        {
            Debug.Log("No main camera");
        }

        if (heartSpawnPos == null)
        {
            Debug.Log("No heart spawn position");
        }
    }

    private void OnEnable()
    {
        PlayerBase.HealthModified += UpdateHealth;
        PlayerBase.HBOverheal += OverHealingEffect;
        PlayerBase.HealReady += ReadyTextAnimation;
        PlayerBase.HealActivated += ActivatedTextAnimation;

        WeaponManager.OnHoldChargeATK += UpdateChargeATK;
    }

    private void OnDisable()
    {
        PlayerBase.HealthModified -= UpdateHealth;
        PlayerBase.HBOverheal -= OverHealingEffect;
        PlayerBase.HealReady -= ReadyTextAnimation;
        PlayerBase.HealActivated -= ActivatedTextAnimation;

        WeaponManager.OnHoldChargeATK -= UpdateChargeATK;
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
    private void Update()
    {
        //levelText.text = "LVL: " + level;

        float? hbProgress = playerBase?.GetHealBarProgress();
        if (hbProgress != null)
        {
            UpdateHBProgressFill(hbProgress.Value);
        }

        float? caProgress = weaponManager ? weaponManager.GetChargeATKProgress() : null;
        if (caProgress != null)
        {
            UpdateCAProgressFill(caProgress.Value);
        }

        xOffsetHeartValue = Screen.width * xOffsetHeartPercent;

    }


    #region UI Functions
    private void UpdateHBProgressFill(float value)
    {
        float curVelocity = 0f;
        float tempValue = Mathf.SmoothDamp(healBarUI.fillAmount, value, ref curVelocity, 0.0075f);
        healBarUI.fillAmount = tempValue;
    }

    private void UpdateCAProgressFill(float value)
    {
        Vector2 targetPosition = mainCamera.WorldToScreenPoint(this.transform.position);

        float curVelocity = 0f;
        float tempValue = Mathf.SmoothDamp(innerChargeATKBarUI.fillAmount, value, ref curVelocity, 0.0075f);
        innerChargeATKBarUI.fillAmount = tempValue;

        if (innerChargeATKBarUI.fillAmount >= 0.975f) // 1: Bar is filled 
        {

            float xRandomOffset = UnityEngine.Random.Range(Screen.width * (-0.01f), Screen.height * (0.01f)); // Bruh... I really had to put in UnityEngine before it.
            float yRandomOffset = UnityEngine.Random.Range(Screen.height * (-0.005f), Screen.height * (0.005f)); 

            // Scale Y offset based on screen size
            float yOffset = Screen.height * 0.075f; // 7.5% of screen height
            outerChargeATKBarUI.transform.position = new Vector2(targetPosition.x + xRandomOffset, (targetPosition.y + yOffset) + yRandomOffset);

        }
        else
        {
            // Scale Y offset based on screen size
            float yOffset = Screen.height * 0.075f; // 7.5% of screen height
            outerChargeATKBarUI.transform.position = new Vector2(targetPosition.x, targetPosition.y + yOffset);            
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
        Vector3 currentVel = Vector3.zero; 

        if (isOverHealing == true)
        {
            Vector3 spawnPos = heartList[heartList.Count - 1].transform.position + new Vector3(xOffsetHeartValue, 0f, 0f);

            tempOverHealHeart = Instantiate(overHealHeart, spawnPos, Quaternion.identity, heartContainer.transform);

            while (healBarUI.color != minColor)
            {
                healBarUI.color = Color.Lerp(minColor, maxColor, playerBase.GetHealBarProgress());

                Vector3 tempScale = Vector3.SmoothDamp(tempOverHealHeart.transform.localScale, orgHeartScale, ref currentVel, 0.0125f);
                tempOverHealHeart.transform.localScale = tempScale;

                yield return null;
            }

        }

        else if (isOverHealing == false)
        { 
            healBarUI.color = orgHBColor;
                
            Debug.Log("Stop Over Healing.");

            Debug.Log(tempOverHealHeart.transform.localScale);

            while (tempOverHealHeart.transform.localScale != Vector3.zero)
            {
                Vector3 tempScale = Vector3.SmoothDamp(tempOverHealHeart.transform.localScale, Vector3.zero, ref currentVel, 0.0125f);
                tempOverHealHeart.transform.localScale = tempScale;

                yield return null;
            }
            
            tempOverHealHeart = null; // Not affiliated with heart list for safety purpose
        }

        overHealingCoroutine = null;
    }


    private void UpdateHealth(float modifiedHealth, float maxHealth, bool? increased)
    {
        if (heartCoroutine != null)
        {
            StopCoroutine(heartCoroutine);
            heartCoroutine = null;
        }

        heartCoroutine = StartCoroutine(ModifyHearts(modifiedHealth, maxHealth, increased));
    }

    private IEnumerator ModifyHearts(float modifiedHealth, float maxHealth, bool? increased)
    {
        Vector3 currentVel = Vector3.zero;

        if (increased == true)
        {
            if (modifiedHealth <= maxHealth)
            {
                int heartsToSpawn = (int)modifiedHealth - heartList.Count;

                for (int i = 0; i < heartsToSpawn; i++)
                {
                    Vector3 spawnPos = heartList.Count == 0
                        ? heartSpawnPos.position // First heart at spawn position
                        : heartList[heartList.Count - 1].transform.position + new Vector3(xOffsetHeartValue, 0, 0); // Next hearts beside the last one

                    Image tempHeart = Instantiate(playerHeart, spawnPos, Quaternion.identity, heartContainer.transform);
                    heartList.Add(tempHeart);

                    while (tempHeart.transform.localScale != orgHeartScale)
                    {
                        tempHeart.transform.localScale = Vector3.SmoothDamp(tempHeart.transform.localScale, orgHeartScale, ref currentVel, 0.0125f);
                        yield return null;
                    }
                }
            }
        }
        else if (increased == false)
        {
            if (modifiedHealth >= 0 && heartList.Count > 0)
            {
                int heartsToRemove = heartList.Count - (int)modifiedHealth;

                for (int i = 0; i < heartsToRemove; i++)
                {
                    Image lastHeart = heartList[heartList.Count - 1];

                    while (lastHeart.transform.localScale != Vector3.zero)
                    {
                        lastHeart.transform.localScale = Vector3.SmoothDamp(lastHeart.transform.localScale, Vector3.zero, ref currentVel, 0.0125f);
                        yield return null;
                    }

                    Destroy(heartList[heartList.Count - 1].gameObject);
                    heartList.RemoveAt(heartList.Count - 1);
                }
            }
        }

        heartCoroutine = null;
    }



    private void UpdateChargeATK(bool isHolding)
    {
        if (chargeATKCoroutine != null)
        {
            StopCoroutine(chargeATKCoroutine);
            chargeATKCoroutine = null;
        }

        if (chargeATKCoroutine == null)
            chargeATKCoroutine = StartCoroutine(ChargeAttack(isHolding));

    }

    private IEnumerator ChargeAttack(bool isHolding)
    {
        float lerpSpeed = 2.5f;
        Vector3 curVelocity = Vector3.zero;

        while (isHolding && weaponManager.GetWeaponBaseRef() != null)
        {
            Color tempInnerColor = Color.Lerp(orgICABcolor, Color.red, weaponManager.GetChargeATKProgress());
            innerChargeATKBarUI.color = tempInnerColor;

            Color tempOuterColor = Color.Lerp(orgOCABcolor, Color.black, weaponManager.GetChargeATKProgress());
            outerChargeATKBarUI.color = tempOuterColor;

            Vector3 tempScale = Vector3.SmoothDamp(outerChargeATKBarUI.transform.localScale, orgCABarScale, ref curVelocity, 0.025f);
            outerChargeATKBarUI.transform.localScale = tempScale;

            yield return null;
        }

        while (!isHolding && weaponManager.GetWeaponBaseRef() != null)
        {
            Color tempInnerColor = Color.Lerp(innerChargeATKBarUI.color, orgICABcolor, lerpSpeed * Time.deltaTime);
            innerChargeATKBarUI.color = tempInnerColor;

            Color tempOuterColor = Color.Lerp(outerChargeATKBarUI.color, orgOCABcolor, lerpSpeed * Time.deltaTime);
            outerChargeATKBarUI.color = tempOuterColor;

            Vector3 tempScale = Vector3.SmoothDamp(outerChargeATKBarUI.transform.localScale, Vector3.zero, ref curVelocity, 0.025f);
            outerChargeATKBarUI.transform.localScale = tempScale;

            yield return null;
        }

        chargeATKCoroutine = null;

    }

    #endregion

    #region UI Animation 
    private void ReadyTextAnimation(bool isReady, string displayText) 
    {
        healText.text = displayText;

        if (isReady)
            healTextAnimator.Play("Ready Text Animation", 0); // Animator.Play(string Animation Name, Animation Layer (Base Layer = 0, ...))
                    
        else
            healTextAnimator.Play("Not Ready Text Animation", 0);
    }

    private void ActivatedTextAnimation()
    {
        healTextAnimator.Play("Activated Text Animation", 0);
    }

    #endregion
}
