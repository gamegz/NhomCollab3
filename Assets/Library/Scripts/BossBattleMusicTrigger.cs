using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleMusicTrigger : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if(GameManager.Instance.bossBattleMusic != null)
            GameManager.Instance.OnEnterBossRoom();
    }

    private void OnDisable()
    {
        if(GameManager.Instance.bossBattleMusic != null)
            GameManager.Instance.OnFinishBossRoom();
    }

    private void OnDestroy()
    {
        if(GameManager.Instance.bossBattleMusic != null)
            GameManager.Instance.OnFinishBossRoom();
    }
}
