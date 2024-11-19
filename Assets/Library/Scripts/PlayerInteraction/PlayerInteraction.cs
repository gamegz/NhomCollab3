using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Check if there's an interactable in range and if "E" is pressed
            if (currentInteractable != null)
            {
                currentInteractable.OnInteract();
            }
            else
            {
                Debug.Log("Nothing To Interact with");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has an IInteractable component
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear the current interactable if the player leaves its range
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
