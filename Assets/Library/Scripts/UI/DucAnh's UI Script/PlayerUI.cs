using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class PlayerUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Animator healTextAnimator;
    [SerializeField] private Image innerChargeATKBarUI;
    [SerializeField] private Image outerChargeATKBarUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject instructionText;
    [SerializeField] private GameObject instructionText2;

    private TextMeshProUGUI healText;
    //[SerializeField] private TextMeshProUGUI levelText; // May or may not use in the future 
    private PlayerBase playerBase;
    private PlayerMovement playerMovement;
    private Color orgICABcolor; // Original Inner Charge Attack Bar Color  [UI Purpose]
    private Color orgOCABcolor; // Original Outer Charge Attack Bar Color  [UI Purpose] [THIS IS THE PARENT OF "innerChargeATKBarUI"]
    private Color orgHBColor; // Original Heal Bar Color 


    [Header("Values")]
    private Vector3 orgCABarScale = new Vector3(1.25f, 4f, 1f); // Original Charged Attack Bar Scale [For any one who couldn't read]

    
    private Coroutine chargeATKCoroutine = null;


    private void Awake()
    {
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

        //playerHearts[playerHearts.Length - 1].transform.localScale = Vector3.zero;

        playerMovement = GetComponent<PlayerMovement>();
        playerBase = GetComponent<PlayerBase>();
        if (playerBase == null)
        {
            Debug.Log("No player base");
        }

        if (playerMovement == null)
        {
            Debug.Log("No player movement");
        }

        if (weaponManager == null)
        {
            Debug.Log("No weapon manager");
        }

        if (mainCamera == null)
        {
            Debug.Log("No main camera");
        }
    }


    private void Start()
    {
        //dashChargeIndicator.gameObject.SetActive(false);
        instructionText.SetActive(false);
        instructionText2.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerBase.HealReady += ReadyTextAnimation;
        PlayerBase.HealActivated += ActivatedTextAnimation;
        WeaponManager.OnHoldChargeATK += UpdateChargeATK;
    }

    private void OnDisable()
    {
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
    
    #region UI Functions
    
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

    #region InstructionText

    public void ToggleInstructionText(bool onOrOff)
    {
        instructionText.SetActive(onOrOff);
    }

    public void ToggleInstructionText2(bool onOrOff)
    {
        instructionText2.SetActive(onOrOff);
    }
    #endregion
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
