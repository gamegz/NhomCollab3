using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton?.onClick.AddListener(() => {
            Time.timeScale = 1.0f;
            GameManager.Instance.UpdateGameState(GameState.PLAYING);           
        });

        quitButton?.onClick.AddListener(() => GameManager.Instance.PublicOnApplicationQuit());
    }
}
