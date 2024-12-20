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
    private TypewriterEffect typewriterEffect;
    private InputAction interactAction;

    public void Start(){
        var playerActionMap = new InputActionMap("Player");
        interactAction = playerActionMap.AddAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
        typewriterEffect = GetComponent<TypewriterEffect>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject){
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject){
        yield return new WaitForSeconds(2);
        
        foreach (string dialogue in dialogueObject.Dialogue){
            yield return typewriterEffect.Run(dialogue, text_label);
            yield return new WaitUntil(() => interactAction.triggered);
            interactAction.Reset();
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox(){
        dialogueBox.SetActive(false);
        text_label.text = string.Empty;
    }


}