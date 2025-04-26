using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuHolder;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private RectTransform mainPanel;
    //[SerializeField] private string mainMenuName = "MainMenu";

    private Sequence menuTween;
    private PlayerInput input;
    private bool gamePaused;
    private TMP_Text continueText;
    private TMP_Text mainMenuText;
    private TMP_Text quitText;

    private void Awake()
    {
        continueText = continueButton.GetComponentInChildren<TMP_Text>();
        mainMenuText = mainMenuButton.GetComponentInChildren<TMP_Text>();
        quitText = quitButton.GetComponentInChildren<TMP_Text>();

        input = new PlayerInput();

        continueButton?.onClick.AddListener(() => {
            gamePaused = false;
            Time.timeScale = 1.0f;
            PauseMenuHolder.SetActive(false);
            GameManager.Instance.UpdateGameState(GameState.PLAYING);
        });

        mainMenuButton?.onClick.AddListener(() => {
            Time.timeScale = 1.0f;
            gamePaused = false;
            PauseMenuHolder.SetActive(false);
            GameManager.Instance.UpdateGameState(GameState.SELECTGAME);
        });

        quitButton?.onClick.AddListener(() => GameManager.Instance.PublicOnApplicationQuit());
        PauseMenuHolder.SetActive(false);       
    }

    private void OnEnable()
    {
        input.Enable();
        input.UI.Escape.performed += OnRequestPauseMenu;
    }

    private void OnDisable()
    {
        input.UI.Escape.performed -= OnRequestPauseMenu;
        input.Disable();
    }

    private void OnRequestPauseMenu(InputAction.CallbackContext context)
    {
        if (gamePaused) { return; }       
        if (GameManager.Instance.state != GameState.PLAYING) { return; }

        mainPanel.anchoredPosition = new Vector3(-mainPanel.rect.width, 0, 0);
        ChangeTextAlphaToZero(continueText);
        ChangeTextAlphaToZero(mainMenuText);
        ChangeTextAlphaToZero(quitText);
        
        PauseMenuHolder.SetActive(true);
        gamePaused = true;
        GameManager.Instance.UpdateGameState(GameState.OPENMENU);

        menuTween.SetUpdate(true);
        menuTween.Kill();
        menuTween = DOTween.Sequence();
        menuTween
            .Append(mainPanel.DOAnchorPos(new Vector2(0, 0), 0.3f, false).SetEase(Ease.OutSine).SetUpdate(true))
            .Append(continueText.DOFade(1, 0.2f)).SetUpdate(true)
            .Append(mainMenuText.DOFade(1, 0.2f)).SetUpdate(true)
            .Append(quitText.DOFade(1, 0.2f)).SetUpdate(true);

    }

    private void ChangeTextAlphaToZero(TMP_Text text)
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }
}
