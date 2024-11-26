using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    float previousTimeScale = 1f;
    bool isPaused;
    private void Awake()
    {
        PlayerDatas.Instance.LoadGame();
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void TogglePause()
    {
        if(Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            isPaused = true;
        }
        else if(Time.timeScale == 0)
        {
            Time.timeScale = previousTimeScale;
            isPaused = false;
        }
    }
}
