using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox; // The UI container for displaying response options
    [SerializeField] private RectTransform responseButtonTemplate; // Template for a single response button
    [SerializeField] private RectTransform responseContainer; // Parent container for response buttons

    public PlayerCam playerCam; // Reference to the player's camera script for enabling/disabling camera movement

    private DialogueUI dialogueUI; // Reference to the DialogueUI script
    private List<GameObject> tempResponseButtons = new List<GameObject>(); // List to keep track of dynamically created response buttons

    // Initialise references to other components
    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
    }

    // Displays a list of responses as buttons in the UI
    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;

        // Create a button for each response
        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer); // Create a new response button
            responseButton.gameObject.SetActive(true); // Ensure the button is visible
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText; // Set the button text
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response)); // Add a click listener to handle the response

            tempResponseButtons.Add(responseButton); // Add the button to the list of temporary buttons

            responseBoxHeight += responseButtonTemplate.sizeDelta.y; // Adjust the response box height
        }

        // Resize the response box to fit the buttons
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true); // Show the response box

        // Unlock the cursor and make it visible for selecting responses
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player camera movement while responses are shown
        playerCam.DisableCameraMovement();
    }

    // Handles the logic when a response is selected
    private void OnPickedResponse(Response response)
    {
        responseBox.gameObject.SetActive(false); // Hide the response box

        // Destroy all temporary response buttons
        foreach (GameObject responseButton in tempResponseButtons)
        {
            Destroy(responseButton);
        }
        tempResponseButtons.Clear(); // Clear the list of response buttons

        // Show the next part of the dialogue based on the selected response
        dialogueUI.ShowDialogue(response.DialogueObject);

        // Lock the cursor and make it invisible again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable player camera movement after the responses are handled
        playerCam.EnableCameraMovement();
    }
}
