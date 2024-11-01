using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantUI : MonoBehaviour, IInteractable
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter");
            UIManager.Instance.OnEnableMerchantInstructionText(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.OnEnableMerchantInstructionText(false);
        }
    }

    public void OnInteract()
    {
        Debug.Log("OpenMerchant");
        UIManager.Instance.OnEnableMerchantPanel(true);
    }
}
