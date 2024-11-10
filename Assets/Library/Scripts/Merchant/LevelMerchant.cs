using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMerchant : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject levelInstructionText;

    void Start()
    {
        levelInstructionText.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelInstructionText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelInstructionText.SetActive(false);
        }
    }

    public void OnInteract()
    {
        LevelShopUI.Instance.OnEnablePanel(true); 
    }
}
