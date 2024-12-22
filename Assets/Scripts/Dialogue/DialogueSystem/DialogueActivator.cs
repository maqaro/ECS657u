
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player)){
            player.Interactable = this;
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player)){
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this){
                player.Interactable = null;
            }
        }
    }

    public void Interact(PlayerMovement player){
        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
