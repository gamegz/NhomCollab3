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
    [SerializeField] private Button BackToHomeLobbyButton;
    [SerializeField] private GameObject UpgradePanel;
    [SerializeField] private GameObject MerchantPanel;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI MovementSpeedText;
    [SerializeField] private TextMeshProUGUI FConversionRateText;
    [SerializeField] private PlayerBase _playerBase;
    [SerializeField] private GameObject LosePanel;
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
        CloseUpgradePanelButton?.onClick.AddListener(() => OnEnableUpgradePanel(false));
        CloseMerchantPanelButton?.onClick.AddListener(() => OnEnableMerchantPanel(false));
        UpgradeButton?.onClick.AddListener(OnUpgradeCharacter);
        BackToHomeLobbyButton?.onClick.AddListener(() => BackToHomeLobby());
    }


    void Update()
    {
        if(HPText == null && MovementSpeedText == null && FConversionRateText == null) { return; }
        HPText.text = "HP: " + PlayerDatas.Instance.GetStats.Health.ToString();
        MovementSpeedText.text = "Move Speed: " + PlayerDatas.Instance.GetStats.MoveSpeed.ToString();
        //FConversionRateText.text = "FConversion Rate: " + PlayerDatas.Instance.GetStats.FConversionRate.ToString();
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
        PlayerBase.Instance.OnUpgradeCharacter();
    }

    public void OnEnableLosePanel()
    {
        LosePanel.SetActive(true);
    }

    public void BackToHomeLobby()
    {
        GameManager.Instance.UpdateGameState(GameState.HOMELOBBY);
    }
}
