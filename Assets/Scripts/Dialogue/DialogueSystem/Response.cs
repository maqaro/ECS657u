using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A serializable class representing a dialogue response and its associated follow-up dialogue
[System.Serializable]
public class Response
{
    [SerializeField] private string responseText; // The text displayed for this response
    [SerializeField] private DialogueObject dialogueObject; // The follow-up dialogue object triggered by selecting this response

    // Public getter for the response text
    public string ResponseText => responseText;

    // Public getter for the associated follow-up dialogue object
    public DialogueObject DialogueObject => dialogueObject;
}
