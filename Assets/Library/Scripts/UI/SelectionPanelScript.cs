using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class SelectionPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject respawnSelectionUI;  // The UI Panel
    [SerializeField] private Transform buttonContainer;  // The UI Panel's content area
    [SerializeField] private Button respawnButtonPrefab;  // Prefab for buttons
    [SerializeField] private Button closeButtonForRespawnPanle;

    private Dictionary<GameObject, Button> respawnButtons = new Dictionary<GameObject, Button>();
    // Start is called before the first frame update
    void Start()
    {
        closeButtonForRespawnPanle?.onClick.AddListener(CloseRespawnSelectionUI);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (!GameManager.Instance.isPlayerDead && point == currentPoint) continue;  // Skip current point

            Button newButton = Instantiate(respawnButtonPrefab, buttonContainer);
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = point.name;
            newButton.onClick.AddListener(() => SelectRespawnPoint(point));
            respawnButtons[point] = newButton;
        }
        respawnSelectionUI.SetActive(true);  // Show the UI
        
        if(!GameManager.Instance.isPlayerDead) { GameManager.Instance.UpdateGameState(GameState.OPENMENU); } 
    }

    private void SelectRespawnPoint(GameObject respawnPoint)
    {
        // Teleport the player and close UI
        GameManager.Instance.TeleportPlayerToRespawnPoint(respawnPoint);
        CloseRespawnSelectionUI();
    }

    public void CloseRespawnSelectionUI()
    {
        respawnSelectionUI.SetActive(false);
        GameManager.Instance.ExitOverviewMode();
        
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
    }
}
