using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button MainMenuButton;
    private void Awake()
    {
        BossEnemyBase.OnBossDeath += OnEnableWinScreen;
        MainMenuButton?.onClick.AddListener(BackToMainMenu);
    }

    private void OnEnable()
    {
        BossEnemyBase.OnBossDeath += OnEnableWinScreen;
    }

    private void OnDisable()
    {
        BossEnemyBase.OnBossDeath -= OnEnableWinScreen;
    }

    private void OnDestroy()
    {
        BossEnemyBase.OnBossDeath -= OnEnableWinScreen;
    }

    private void OnEnableWinScreen()
    {
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }

    private void BackToMainMenu()
    {
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.SELECTGAME);
    }
}
