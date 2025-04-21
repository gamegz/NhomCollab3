using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuHolder;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuName = "MainMenu";

    private GameManager gameManager;

    private PlayerInput input;

    private void Awake()
    {
        input = new PlayerInput();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        continueButton?.onClick.AddListener(OnContinue);
        mainMenuButton?.onClick.AddListener(OnMainMenu);
        quitButton?.onClick.AddListener(() => gameManager.PublicOnApplicationQuit());
        PauseMenuHolder.SetActive(false);
    }

    private void OnEnable()
    {
        input.Player.Escape.performed += OnRequestPauseMenu;
    }

    private void OnRequestPauseMenu(InputAction.CallbackContext context)
    {
        
        PauseMenuHolder.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1.0f;
        PauseMenuHolder.SetActive(false);
        SceneManager.LoadScene(mainMenuName);
    }

    private void OnContinue()
    {
        Time.timeScale = 1.0f;
        PauseMenuHolder.SetActive(false);
    }
}
