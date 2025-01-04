using UnityEngine;
using TMPro; // For TextMeshPro UI

public class DialogueActivator : MonoBehaviour, IInteractable
{

    public enum InteractionType
    {
        TalkToNPC,
        PickUpItem,
        ShowHint
    }

    [SerializeField] private DialogueObject dialogueObject; // The dialogue object to display when this activator is interacted with
    [SerializeField] private GameObject interactPrompt; 
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private InteractionType interactionType;

    private bool isPlayerInRange = false; 

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    // Called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player))
        {
            // Show the interaction prompt
            isPlayerInRange = true;
            interactPrompt.SetActive(true);
            //promptText.text = "Press [E] to talk to";

            // Change the text dynamically based on the interaction type
            switch (interactionType)
            {
                case InteractionType.TalkToNPC:
                    promptText.text = "Press [E] to talk to";
                    break;
                case InteractionType.PickUpItem:
                    promptText.text = "Press [E] to pick up";
                    break;
                case InteractionType.ShowHint:
                    promptText.text = "Press [E] for hint";
                    break;
            }

            // Assign this DialogueActivator as the player's current interactable
            player.Interactable = this;
        }
    }

    // Called when another collider exits the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player))
        {
            // Hide the interaction prompt
            isPlayerInRange = false;
            interactPrompt.SetActive(false);

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

        interactPrompt.SetActive(false);
    }
}
