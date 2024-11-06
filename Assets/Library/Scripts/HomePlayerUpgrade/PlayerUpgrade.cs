using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUpgrade : MonoBehaviour, IInteractable
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
            UIManager.Instance.OnEnableUpgradeInstructionText(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.OnEnableUpgradeInstructionText(false);
        }
    }

    public void OnInteract()
    {
        UIManager.Instance.OnEnableUpgradePanel(true);
    }
}
