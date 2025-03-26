using Core.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private GameObject respawnSelectionUI;  // The UI Panel
    [SerializeField] private Transform buttonContainer;  // The UI Panel's content area
    [SerializeField] private Button respawnButtonPrefab;  // Prefab for buttons
    [SerializeField] private Button closeButtonForRespawnPanle;

    private Dictionary<GameObject, Button> respawnButtons = new Dictionary<GameObject, Button>();
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
        closeButtonForRespawnPanle?.onClick.AddListener(CloseRespawnSelectionUI);
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
        if(LosePanel == null) { return; }
        LosePanel.SetActive(true);
    }

    public void BackToHomeLobby()
    {
        GameManager.Instance.UpdateGameState(GameState.HOMELOBBY);
    }

    public void ShowRespawnSelectionUI()
    {
        List<GameObject> claimedPoints = GameManager.Instance.GetClaimedRespawnPoints();
        GameObject currentPoint = GameManager.Instance.GetCurrentRespawnPoint();
        Debug.LogWarning("Claimed Respawn Points: " + string.Join(", ", claimedPoints.Select(p => p.name)));
        Debug.LogWarning("Current Respawn Point: " + (currentPoint != null ? currentPoint.name : "None"));
        Debug.LogWarning("Current Respawn Point in GameManager: " + (GameManager.Instance.GetCurrentRespawnPoint() != null ? GameManager.Instance.GetCurrentRespawnPoint().name : "None"));
        // Clear old buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        respawnButtons.Clear();

        // Create new buttons for each claimed respawn point (except current one)
        foreach (GameObject point in claimedPoints)
        {
            Debug.Log($"Checking Point: {point.name}, Current: {currentPoint?.name ?? "None"}");
            if (point == currentPoint) continue;  // Skip current point

            Button newButton = Instantiate(respawnButtonPrefab, buttonContainer);
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = point.name;
            newButton.onClick.AddListener(() => SelectRespawnPoint(point));
            respawnButtons[point] = newButton;
        }

        respawnSelectionUI.SetActive(true);  // Show the UI
        GameManager.Instance.TogglePause();
    }

    private void SelectRespawnPoint(GameObject respawnPoint)
    {
        // Teleport the player and close UI
        GameManager.Instance.TeleportPlayerToRespawnPoint(respawnPoint);
        GameManager.Instance.TogglePause();
        CloseRespawnSelectionUI();
    }

    public void CloseRespawnSelectionUI()
    {
        respawnSelectionUI.SetActive(false);
        GameManager.Instance.ExitOverviewMode();
    }
}
