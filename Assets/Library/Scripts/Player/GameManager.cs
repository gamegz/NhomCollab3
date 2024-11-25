using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private void Awake()
    {
        PlayerDatas.Instance.LoadGame();
        DontDestroyOnLoad(Player);
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
