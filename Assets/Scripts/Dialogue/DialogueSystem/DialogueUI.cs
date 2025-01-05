using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text text_label;
    [SerializeField] private GameObject HUD;

    public bool IsDialogueActive { get; private set; }
    
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private InputAction interactAction;
    private InputAction skipAction;

    // Initializes the dialogue system, sets up input actions, and ensures the dialogue box is hidden at start
    public void Start()
    {
        var playerActionMap = new InputActionMap("Player");

        // Define input actions for interacting and skipping dialogue
        interactAction = playerActionMap.AddAction("Interact", binding: "<Keyboard>/e");
        skipAction = playerActionMap.AddAction("Skip", binding: "<Keyboard>/space");

        // Enable input actions
        interactAction.Enable();
        skipAction.Enable();

        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
    }

    // Activates the dialogue box and starts the dialogue display process
    public void ShowDialogue(DialogueObject dialogueObject)
    {
        DisableHUD();
        IsDialogueActive = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void ShowHUD()
    {
        HUD.SetActive(true);
    }

    private void DisableHUD()
    {
        HUD.SetActive(false);
    }



    public void AddResponseEvents(ResponseEvent[] responseEvents){
        responseHandler.AddResponseEvents(responseEvents);
    }

    // Steps through the dialogue lines one by one and waits for player input to proceed
    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            // Run typing effect with optional skipping
            yield return RunTypingEffect(dialogue);

            text_label.text = dialogue;

            // If this is the last dialogue line with responses, break to show responses
            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => interactAction.triggered);
            interactAction.Reset();
        }

        // Show response options or close the dialogue box if none are present
        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    // Runs the typewriter effect for displaying dialogue and allows skipping
    private IEnumerator RunTypingEffect(string dialogue)
    {
        bool isSkipping = false;
        typewriterEffect.Run(dialogue, text_label);

        // Wait until the typewriter effect finishes or is skipped
        while (typewriterEffect.IsRunning)
        {
            yield return null;
            if (skipAction.triggered && !isSkipping)
            {
                isSkipping = true;
                typewriterEffect.Stop();
                text_label.text = dialogue;
            }
        }
    }

    // Closes the dialogue box and resets its state
    public void CloseDialogueBox()
    {
        IsDialogueActive = false;
        dialogueBox.SetActive(false);
        text_label.text = string.Empty;

        // Notify the PlayerMovement script that the interaction has ended
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.EndInteraction();
        }
        ShowHUD();
    }
}
