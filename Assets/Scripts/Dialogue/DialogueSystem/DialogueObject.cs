using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    //Script for the main dialogue
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    // Array for the amount of responses
    public string[] Dialogue => dialogue;
    public bool HasResponses => Responses != null && Responses.Length > 0;
    public Response[] Responses => responses;
}
