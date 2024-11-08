using Core.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button CloseUpgradePanelButton;
    [SerializeField] private Button CloseMerchantPanelButton;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private GameObject UpgradeInstructionText;
    [SerializeField] private GameObject MerchantInstructionsText;
    [SerializeField] private GameObject UpgradePanel;
    [SerializeField] private GameObject MerchantPanel;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI MovementSpeedText;
    [SerializeField] private TextMeshProUGUI FConversionRateText;
    [SerializeField] private PlayerBase _playerBase;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        UpgradeInstructionText.SetActive(false);
        MerchantInstructionsText.SetActive(false);
        CloseUpgradePanelButton.onClick.AddListener(() => OnEnableUpgradePanel(false));
        CloseMerchantPanelButton.onClick.AddListener(() => OnEnableMerchantPanel(false));
        UpgradeButton.onClick.AddListener(OnUpgradeCharacter);
    }


    void Update()
    {
        if(_playerBase == null)
        {
            return;
        }

        HPText.text = "HP: " + _playerBase.Health.ToString();
        MovementSpeedText.text = "Move Speed: " + _playerBase.MoveSpeed.ToString();
        FConversionRateText.text = "FConversion Rate: " + _playerBase.FConversionRate.ToString();
    }

    public void OnEnableUpgradeInstructionText(bool isEnable)
    {
        
        UpgradeInstructionText.SetActive(isEnable);
    }

    public void OnEnableMerchantInstructionText(bool isEnable) 
    {
        MerchantInstructionsText.SetActive(isEnable);
    }

    public void OnEnableUpgradePanel(bool isEnable)
    {
        UpgradePanel.SetActive(isEnable);
    }

    public void OnEnableMerchantPanel(bool isEnable)
    {
        MerchantPanel.SetActive(isEnable);  
    }

    public void OnUpgradeCharacter()
    {
        _playerBase.OnUpgradeCharacter();
    }
}
