using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Core.Events;
using UnityEngine.InputSystem;

namespace UI
{
    //Keep track of all upgrade related UI
    public class UpgradeMenu : MonoBehaviour
    {
        //Refs'
        [SerializeField] private StatsUpgrade statsUpgradeCS;
        [SerializeField] private GameObject upgradeMenuObj;
        [SerializeField] private Slider expSlider;

        [Space]
        //Dictionary<string, UpdateUI> upgradeGroupDictionary = new Dictionary<string, UpdateUI>();
        Dictionary<UpgradeType, UpdateUI> upgradeUIDictionary = new Dictionary<UpgradeType, UpdateUI>();
        [SerializeField] UpdateUI healthUpgrade;
        [SerializeField] UpdateUI strengthUpgrade;
        [SerializeField] UpdateUI speedUpgrade;
        [SerializeField] UpdateUI recoveryUpgrade;
        [SerializeField] UpdateUI enduranceUgrade;
        [SerializeField] UpdateUI dashChargesUpgrade;
        [Space]
 
        [SerializeField] Text sacrificialGemCountText;

        private int _totalCurrentGemUse; //Gem use temporary
        private float targetValue;
        private PlayerInput playerInput;
        
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            playerInput = new PlayerInput();
            exitButton?.onClick.AddListener(CloseUpgradeMenu);
        }

        void Start()
        {
            SetUpUI();
        }

        private void OnEnable()
        {
            playerInput.Player.OpenUpgradeMenu.performed += EnableUpgradeMenu;
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Player.OpenUpgradeMenu.performed -= EnableUpgradeMenu;
            playerInput.Disable();
        }

        private void EnableUpgradeMenu(InputAction.CallbackContext context)
        { 
            if (GameManager.Instance.state != GameState.PLAYING) { return; }
            upgradeMenuObj.SetActive(true);
            GameManager.Instance.UpdateGameState(GameState.OPENMENU);
        }

        private void CloseUpgradeMenu()
        {
            upgradeMenuObj.SetActive(false);
            GameManager.Instance.UpdateGameState(GameState.PLAYING);
        }
        

        public void UpdateExpBar(float currentExpAmount, float maxExpAmount)
        {
            if(expSlider != null)
            {
                sacrificialGemCountText.text = "Sacrificial Gem:" + statsUpgradeCS.GemCount.ToString();
                targetValue = currentExpAmount / maxExpAmount;
                StartCoroutine(UpdateExpGauge());
            }
            else
            {
                Debug.LogWarning("Missing Slider Reference");
            }
        }

        IEnumerator UpdateExpGauge()
        {
            float timer = 0f;
            float maxTimer = 0.2f;
            while(timer < maxTimer)
            {
                timer += Time.deltaTime;
                float t = timer / maxTimer;
                expSlider.value = Mathf.Lerp(expSlider.value, targetValue, t);
                
                yield return null;
            }
            expSlider.value = targetValue;
        }

        private void SetUpUI()
        {

            upgradeUIDictionary = new Dictionary<UpgradeType, UpdateUI>
            {
                { UpgradeType.Health, healthUpgrade },
                { UpgradeType.Damage, strengthUpgrade },
                { UpgradeType.MovementSpeed, speedUpgrade },
                { UpgradeType.DashRecovery, enduranceUgrade },
                { UpgradeType.Recovery, recoveryUpgrade },
                { UpgradeType.DashCharge, dashChargesUpgrade },
            };

            ResetUpdateText();
            sacrificialGemCountText.text = "Sacrificial Gem:" + statsUpgradeCS.GemCount.ToString();
        }

        #region BUTTON EVENT
        //Connect to all upgrade button
        public void OnUpgradeClick(string upgradeType)
        {
            Debug.Log(upgradeType);

            var upgradeEnum = (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeType, true);           

            UpdateUI upgradeTarget = upgradeUIDictionary[upgradeEnum];

            //Access values from the Upgrade stats dictionary
            int gemRequiredForUpgrade = statsUpgradeCS.UpgradeGroupDic[upgradeEnum].upgradeRequirement;
            int currentLevel = statsUpgradeCS.UpgradeGroupDic[upgradeEnum].currentLevel;
            int maxLevel = statsUpgradeCS.UpgradeGroupDic[upgradeEnum].maxLevel;

            //Check upgrade condition
            if (statsUpgradeCS.GemCount < gemRequiredForUpgrade) { return; }
            if (currentLevel >= maxLevel) { return; }
            Debug.Log("currentLevel" + currentLevel);
            //Changes values from the UIgroup
            upgradeTarget.upgradeCount++;
            upgradeTarget.upgradeCountText.text = "+" + upgradeTarget.upgradeCount.ToString();
            upgradeTarget.statsValue.text = statsUpgradeCS.GetPercentIncrease(upgradeEnum, upgradeTarget.upgradeCount).ToString();

            //Changes GemCount
            statsUpgradeCS.GemCount -= gemRequiredForUpgrade;
            sacrificialGemCountText.text = "Sacrificial Gem:" + statsUpgradeCS.GemCount.ToString();

            _totalCurrentGemUse += gemRequiredForUpgrade;

            //Reapply the true value
            upgradeUIDictionary[upgradeEnum] = upgradeTarget;
        }
        //public void OnUpgradeClick(string upgradeType)
        //{
        //    var upgradeEnum = (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeType, true);

        //    UpdateUI upgradeTarget = upgradeGroupDictionary[upgradeType];

        //    //Check amount gem required
        //    if(sacrificialGemCount < upgradeTarget.upgradeRequirement) { return; }

        //    upgradeTarget.upgradeCount ++;
        //    upgradeTarget.upgradeCountText.text = "+" + upgradeTarget.upgradeCount.ToString();


        //    sacrificialGemCount -= upgradeTarget.upgradeRequirement;
        //    sacrificialGemCountText.text = "Sacrificial Gem:" + sacrificialGemCount.ToString();

        //    _totalCurrentGemUse += upgradeTarget.upgradeRequirement;

        //    upgradeGroupDictionary[upgradeType] = upgradeTarget;
        //}



        public void OnConsumeGem()
        {
            //Apply the stored value
            foreach (UpdateUI updateUI in upgradeUIDictionary.Values)
            {
                statsUpgradeCS.UpgradeStats(updateUI.upgradeType, updateUI.upgradeCount);
            }

            //Reset stored value
            ResetUpdateText();
            _totalCurrentGemUse = 0;
            sacrificialGemCountText.text = "Sacrificial Gem:" + statsUpgradeCS.GemCount.ToString();
        }

        public void OnCancelUpgrade()
        {
            //Return gem
            statsUpgradeCS.GemCount += _totalCurrentGemUse;
            sacrificialGemCountText.text = "Sacrificial Gem:" + statsUpgradeCS.GemCount.ToString();

            _totalCurrentGemUse = 0;

            ResetUpdateText();
        }
        #endregion

        public void ResetUpdateText()
        {
            List<UpgradeType> tempUIKey = new List<UpgradeType>(upgradeUIDictionary.Keys);
            foreach (UpgradeType key in tempUIKey)
            {
                UpdateUI tempGroupUI = upgradeUIDictionary[key];
                tempGroupUI.upgradeCountText.text = " ";
                tempGroupUI.statsValue.text = " ";
                tempGroupUI.upgradeCount = 0;
                upgradeUIDictionary[key] = tempGroupUI;
            }

        }


        public void CloseMenu()
        {
            upgradeMenuObj.SetActive(false);
            Time.timeScale = 1;
        }

        public void OpenMenu()
        {
            Time.timeScale = 0;
            upgradeMenuObj.SetActive(true);
        }

        //Event
        public void OnGemCollected()
        {

        }

        public void OnPlayerStatChange()
        {

        }

        public void OnGemAmountChange(int changeAmount)
        {
            statsUpgradeCS.GemCount += changeAmount;
            sacrificialGemCountText.text = statsUpgradeCS.GemCount.ToString();
        }

        [System.Serializable]
        public struct UpdateUI
        {
            public Button upgradeButton;
            public Text upgradeCountText;
            public Text statsValue;
            public int upgradeCount;
            public UpgradeType upgradeType;
        }
    }

    

}
