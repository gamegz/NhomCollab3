using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private Button BackToMenuButton;
    [SerializeField] private Button retryButton;
    // Start is called before the first frame update
    void Start()
    {
        retryButton?.onClick.AddListener(Retry);
        BackToMenuButton?.onClick.AddListener(() => BackToMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnEnableLosePanel()
    {
        if(LosePanel == null) { return; }
        List<GameObject> claimedPoints = GameManager.Instance.GetClaimedRespawnPoints();
        LosePanel.SetActive(true);
        
        if (claimedPoints.Count > 0)
        {
            retryButton.gameObject.SetActive(true);
        }
        else
        {
            retryButton.gameObject.SetActive(false);
        }
    }
    
    private void BackToMenu()
    {
        LosePanel.SetActive(false);
        GameManager.Instance.isPlayerDead = false;
        if (PlayerDatas.Instance.GetStats.currentPlayerHealth <= 0)
        {
            PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
            PlayerDatas.Instance.SaveGame();
        }
        GameManager.Instance.UpdateGameState(GameState.SELECTGAME);
    }

    private void Retry()
    {
        //GameManager.Instance.EnterOverviewMode();
        LosePanel.SetActive(false);
        PlayerDatas.Instance.LoadGame();
        PlayerDatas.Instance.GetStats.currentPlayerHealth = PlayerDatas.Instance.GetStats.Health;
        PlayerDatas.Instance.SaveGame();
        List<GameObject> claimedPoints = GameManager.Instance.GetClaimedRespawnPoints();
        if (claimedPoints.Count > 0)
        {
            GameObject latestRespawnPoint = claimedPoints[claimedPoints.Count - 1]; 
            GameManager.Instance.TeleportPlayerToRespawnPoint(latestRespawnPoint);
        }
        StartCoroutine(WaitToUpdatePlayerHealthAfterRetry());
        
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
        
    }

    private IEnumerator WaitToUpdatePlayerHealthAfterRetry()
    {
        yield return new WaitForSeconds(0.2f);
        PlayerBase.Instance.UpdatePlayerHealth();
    }
}
