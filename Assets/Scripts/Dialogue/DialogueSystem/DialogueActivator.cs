using UnityEngine;

// This class activates a dialogue interaction when the player enters a trigger zone
public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject; // The dialogue object to display when this activator is interacted with

    public void UpdateDialogueObject(DialogueObject dialogueObject){
        this.dialogueObject = dialogueObject;
    }

    // Called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and if the player has a PlayerMovement component
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player))
        {
            // Assign this DialogueActivator as the player's current interactable
            player.Interactable = this;
        }
    }

    // Called when another collider exits the trigger zone
    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player and if the player has a PlayerMovement component
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player))
        {
            // If the current interactable is this DialogueActivator, set it to null
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
            }
        }
    }

    // Called when the player interacts with this activator
    public void Interact(PlayerMovement player)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        // Display the associated dialogue object in the DialogueUI
        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
