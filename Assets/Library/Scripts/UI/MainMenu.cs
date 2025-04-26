using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuScene = "AssetFillMain";

    private void Awake()
    {
        playButton?.onClick.AddListener(() => {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(mainMenuScene);
            GameManager.Instance.UpdateGameState(GameState.PLAYING);  
        });

        quitButton?.onClick.AddListener(() => GameManager.Instance.PublicOnApplicationQuit());
    }
}
