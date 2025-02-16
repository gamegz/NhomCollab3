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


        int _currentPointUse;

        void Start()
        {
            upgradeGroupDictionary.Add("health", healthUpgrade);
            upgradeGroupDictionary.Add("strength", strengthUpgrade);
        }

        //Button event
        public void OnUpgradeClick(string upgradeType)
        {
            UpdateUI upgradeTarget = upgradeGroupDictionary[upgradeType];
            if(sacrificialGemCount < upgradeTarget.upgradeRequirement) { return; }
            upgradeTarget.upgradeCount ++;
            upgradeTarget.upgradeCountText.text = upgradeTarget.upgradeCount.ToString();

            upgradeGroupDictionary[upgradeType] = upgradeTarget;

            sacrificialGemCount -= upgradeTarget.upgradeRequirement;
            sacrificialGemCountText.text = sacrificialGemCount.ToString();

        }

        public void OnConsumeGem()
        {
            
        }

        public void OnCancelUpgrade()
        {

        }

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
        }
    }

    

}
