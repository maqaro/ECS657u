using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text text_label;
    [SerializeField] private DialogueObject testDialogue;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private InputAction interactAction;

    public void Start(){
        var playerActionMap = new InputActionMap("Player");
        interactAction = playerActionMap.AddAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
    
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++){
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, text_label);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;
        

            yield return new WaitUntil(() => interactAction.triggered);
            interactAction.Reset();
        }

        if (dialogueObject.HasResponses){
            responseHandler.ShowResponses(dialogueObject.Responses);
        } else {
        CloseDialogueBox();
        }
        
    }

    private void CloseDialogueBox(){
        dialogueBox.SetActive(false);
        text_label.text = string.Empty;
    }


}