using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 17f; // Speed at which the text is typed out

    public bool IsRunning { get; private set; } // Indicates whether the typewriter effect is currently running

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>{'.', '!', '?'}, 0.6f),
        new Punctuation(new HashSet<char>{',', ';', ':'}, 0.3f),
    };

    private Coroutine typingCoroutine; // Reference to the current typing coroutine

    // Starts the typewriter effect for the given text and text label
    public void Run(string textToType, TMP_Text textLabel)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    // Stops the typewriter effect immediately
    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    // Coroutine that types out the given text one character at a time
    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
        textLabel.text = string.Empty; // Clear the text label before starting

        float t = 0; // Timer to control the typing speed
        int charIndex = 0; // Index of the character currently being typed

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            // Increment the timer and calculate the next character index
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            // Type each new character in the current frame
            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1; // Check if this is the last character

                textLabel.text = textToType.Substring(0, i + 1); // Update the text label with the typed characters

                // Pause briefly if the current character is punctuation
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }

        IsRunning = false; // Indicate that the typewriter effect has finished
    }

    // Checks if a character is a punctuation mark and returns its associated wait time
    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    private readonly struct Punctuation{
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
