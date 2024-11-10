using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeShopUI : MonoBehaviour
{
    public static HomeShopUI Instance { get; private set; }

    [SerializeField] private Button closeHomeShopPanelButton;
    [SerializeField] private GameObject homeShopPanel;

    [SerializeField] private List<WeaponItem> weaponItemList;
    [SerializeField] private List<Transform> weaponDisplaySlotList;
    [SerializeField] private List<Transform> itemSpawnSlotList;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); 
    }

    void Start()
    {
        OnEnablePanel(false);
        closeHomeShopPanelButton.onClick.AddListener(() => OnEnablePanel(false));
    }

    void Update()
    {
        
    }

    public void OnEnablePanel(bool isEnable)
    {
        homeShopPanel.SetActive(isEnable);
    }

}
