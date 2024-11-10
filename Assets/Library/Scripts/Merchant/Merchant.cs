using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    private GameObject interactionPrompt;
    private bool isPlayerInRange = false;

    void Start()
    {
        interactionPrompt = transform.Find("FloatingText").gameObject;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false); // Hide initially
        }
        else
        {
            Debug.LogWarning("FloatingText not found");
        }
    }

    void Update()
    {
        
    }

    // Remember the TAGGGGGGG!!!!!
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionPrompt.SetActive(true); // Show the interaction prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exited the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionPrompt.SetActive(false); // Hide the interaction prompt
        }
    }

    public void OnInteract()
    {
        Debug.Log("Interacting with the merchant...");
    }
}
