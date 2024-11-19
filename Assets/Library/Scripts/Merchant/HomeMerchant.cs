using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMerchant : MonoBehaviour, IInteractable
{
    private GameObject homeInstructionText;

    private void Awake()
    {
        homeInstructionText = transform.parent.Find("HomeMerchantInstructionText").gameObject;
    }

    void Start()
    {
        homeInstructionText.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            homeInstructionText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            homeInstructionText.SetActive(false);
        }
    }

    public void OnInteract()
    {
        HomeShopUI.Instance.OnEnablePanel(true);
    }
}
