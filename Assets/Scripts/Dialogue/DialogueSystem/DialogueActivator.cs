
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    public void Interact(PlayerMovement player){
        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
