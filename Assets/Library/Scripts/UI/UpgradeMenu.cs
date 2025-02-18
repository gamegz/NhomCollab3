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
        [SerializeField] UpdateUI dashCharges;
        [Space]
        [SerializeField] int sacrificialGemCount;
        [SerializeField] Text sacrificialGemCountText;

        private int _totalCurrentGemUse;

        int _currentPointUse;

        void Start()
        {
            SetUpUI();
        }

        private void SetUpUI()
        {
            upgradeGroupDictionary.Add("health", healthUpgrade);
            upgradeGroupDictionary.Add("strength", strengthUpgrade);
            upgradeGroupDictionary.Add("speed", strengthUpgrade);
            upgradeGroupDictionary.Add("endurance", strengthUpgrade);
            upgradeGroupDictionary.Add("recovery", strengthUpgrade);
            upgradeGroupDictionary.Add("dashCharge", strengthUpgrade);

            foreach(UpdateUI groupUI in upgradeGroupDictionary.Values)
            {
                groupUI.upgradeCountText.text = " ";
            }
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

            upgradeGroupDictionary[upgradeType] = upgradeTarget;

            sacrificialGemCount -= upgradeTarget.upgradeRequirement;
            sacrificialGemCountText.text = sacrificialGemCount.ToString();

            _totalCurrentGemUse++;

        }

        public void OnConsumeGem()
        {
            PlayerDatas.Instance.OnStatsUpgrade(UpgradeType.MovementSpeed, 1);
        }

        public void OnCancelUpgrade()
        {
            //Return gem

            foreach (UpdateUI groupUI in upgradeGroupDictionary.Values)
            {
                groupUI.upgradeCountText.text = " ";
            }
        }
        #endregion

        //Event
        public void OnGemCollected()
        {

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
