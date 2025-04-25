using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Animator healTextAnimator;
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
    }

    private void OnDisable()
    {
        PlayerBase.HealReady -= ReadyTextAnimation;
        PlayerBase.HealActivated -= ActivatedTextAnimation;
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

    #region UI Animation 
    private void ReadyTextAnimation(bool isReady, string displayText)
    {
        //healText.text = displayText;

        // if (isReady)
        //     healTextAnimator.Play("Ready Text Animation", 0); // Animator.Play(string Animation Name, Animation Layer (Base Layer = 0, ...))
        //
        // else
        //     healTextAnimator.Play("Not Ready Text Animation", 0);
    }

    private void ActivatedTextAnimation()
    {
        // healTextAnimator.Play("Activated Text Animation", 0);
    }

    #endregion
}
