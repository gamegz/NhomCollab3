using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject playerUIHolder;
    [SerializeField] GameObject nonMenuUIHolder;

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.SELECTGAME:
                playerUIHolder.SetActive(false);
                nonMenuUIHolder.SetActive(false);
                break;
            case GameState.OPENMENU:
                nonMenuUIHolder.SetActive(false);
                break;
            case GameState.PLAYING:
                nonMenuUIHolder.SetActive(true);
                playerUIHolder.SetActive(true);
                break;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChanged;
    }
}
