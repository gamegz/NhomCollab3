using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradeMenu : MonoBehaviour
    {
        Dictionary<string, UpdateUI> upgradeGroupDictionary = new Dictionary<string, UpdateUI>();
        [SerializeField] UpdateUI healthUpgrade;
        [SerializeField] UpdateUI strengthUpgrade;
        [SerializeField] UpdateUI speedUpgrade;
        [SerializeField] UpdateUI recoveryUpgrade;
        [SerializeField] UpdateUI enduranceUgrade;
        [SerializeField] UpdateUI dashChargesUpgrade;
        [Space]
        [SerializeField] int sacrificialGemCount; //Need to be increase from other script
        [SerializeField] Text sacrificialGemCountText;

        private int _totalCurrentGemUse; //Gem use temporary

        int _currentPointUse;

        void Start()
        {
            SetUpUI();
        }

        private void SetUpUI()
        {
            upgradeGroupDictionary.Add("health", healthUpgrade);
            upgradeGroupDictionary.Add("strength", strengthUpgrade);
            upgradeGroupDictionary.Add("speed", speedUpgrade);
            upgradeGroupDictionary.Add("endurance", enduranceUgrade);
            upgradeGroupDictionary.Add("recovery", recoveryUpgrade);
            upgradeGroupDictionary.Add("dashCharge", dashChargesUpgrade);

            ResetUpdateText();
            sacrificialGemCountText.text = "Sacrificial Gem:" + sacrificialGemCount.ToString();
        }

        #region Button Event
        //Connect to all upgrade button
        public void OnUpgradeClick(string upgradeType)
        {
            UpdateUI upgradeTarget = upgradeGroupDictionary[upgradeType];
            //Check amount gem required
            if(sacrificialGemCount < upgradeTarget.upgradeRequirement) { return; }

            upgradeTarget.upgradeCount ++;
            upgradeTarget.upgradeCountText.text = "+" + upgradeTarget.upgradeCount.ToString();

            
            sacrificialGemCount -= upgradeTarget.upgradeRequirement;
            sacrificialGemCountText.text = "Sacrificial Gem:" + sacrificialGemCount.ToString();

            _totalCurrentGemUse += upgradeTarget.upgradeRequirement;

            upgradeGroupDictionary[upgradeType] = upgradeTarget;
        }

        public void OnConsumeGem()
        {
            PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.MovementSpeed, 1);
            ResetUpdateText();
            _totalCurrentGemUse = 0;
        }

        public void OnCancelUpgrade()
        {
            //Return gem
            sacrificialGemCount += _totalCurrentGemUse;
            Debug.Log(sacrificialGemCount);
            sacrificialGemCountText.text = "Sacrificial Gem:" + sacrificialGemCount.ToString();
            _totalCurrentGemUse = 0;

            foreach (UpdateUI groupUI in upgradeGroupDictionary.Values)
            {
                groupUI.upgradeCountText.text = " ";
            }

            
        }
        #endregion

        public void ResetUpdateText()
        {
            
            foreach (UpdateUI groupUI in upgradeGroupDictionary.Values)
            {
                UpdateUI tempGroupUI = groupUI;
                tempGroupUI.upgradeCountText.text = " ";
                tempGroupUI.upgradeCount = 0;
                //groupUI = tempGroupUI.upgradeCount;
            }
        }


        //Event
        public void OnGemCollected()
        {

        }

        public void OnGemAmountChange(int changeAmount)
        {
            sacrificialGemCount += changeAmount;
            sacrificialGemCountText.text = sacrificialGemCount.ToString();
        }

        [System.Serializable]
        public struct UpdateUI
        {
            public Button upgradeButton;
            public Text upgradeCountText;
            public Text statsValue;
            public int upgradeRequirement;
            public int upgradeCount;
            public UpgradeType upgradeType;
        }
    }

    

}
