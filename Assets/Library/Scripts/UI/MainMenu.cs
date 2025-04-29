using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Library.Scripts.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string gameScene = "AssetFillMain";
    
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        playButton?.onClick.AddListener(() => {
            audioManager.PlaySoundEffect(GameManager.Instance.soundData.PlayButtonSound);
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(gameScene);
            GameManager.Instance.UpdateGameState(GameState.PLAYING);  
        });

        quitButton?.onClick.AddListener(() =>
        {
            audioManager.PlaySoundEffect(GameManager.Instance.soundData.PlayButtonSound);
            GameManager.Instance.PublicOnApplicationQuit();
        });
        
        optionButton?.onClick.AddListener(() =>
        {
            audioManager.PlaySoundEffect(GameManager.Instance.soundData.PlayButtonSound);
        });
    }
}
