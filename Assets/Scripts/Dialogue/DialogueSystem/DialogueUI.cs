using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text text_label;

    public bool IsDialogueActive { get; private set; }
    
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private InputAction interactAction;
    private InputAction skipAction;


    public void Start(){
        var playerActionMap = new InputActionMap("Player");
        interactAction = playerActionMap.AddAction("Interact", binding: "<Keyboard>/e");
        skipAction = playerActionMap.AddAction("Skip", binding: "<Keyboard>/space");
        
        interactAction.Enable();
        skipAction.Enable();
        
        

        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        IsDialogueActive = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
    
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++){
            string dialogue = dialogueObject.Dialogue[i];
            
            yield return RunTypingEffect(dialogue);

            text_label.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;
        
            yield return null;
            yield return new WaitUntil(() => interactAction.triggered);
            interactAction.Reset();
        }

        if (dialogueObject.HasResponses){
            responseHandler.ShowResponses(dialogueObject.Responses);
        } else {
            CloseDialogueBox();
        }
        
    }

    private IEnumerator RunTypingEffect(string dialogue){
        bool isSkipping = false;
        typewriterEffect.Run(dialogue, text_label);
        
        while (typewriterEffect.IsRunning){
            yield return null;
            if (skipAction.triggered && !isSkipping)
            {
                isSkipping = true;
                typewriterEffect.Stop();
                text_label.text = dialogue;
            }
        
        }
        
    }

    private void CloseDialogueBox(){
        IsDialogueActive = false;
        dialogueBox.SetActive(false);
        text_label.text = string.Empty;

        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.EndInteraction();
        }
    }


}