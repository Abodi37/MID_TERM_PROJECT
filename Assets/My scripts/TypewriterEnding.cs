using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TypewriterEnding : MonoBehaviour
{
    [Header("UI Text Reference")]
    public TextMeshProUGUI textDisplay;
    
    [Header("Story Paragraph")]
    [TextArea(5, 10)] // Gives you a nice big box in the inspector
    public string fullStoryText;

    [Header("Settings")] 
    public float typingSpeed = 0.05f; // Time in seconds between letters (lower is faster)

    [Header("Menu Button Reference")]
    public GameObject mainMenuButton;

    void Start()
    {
        // Ensure text starts empty and button stays hidden
        if (textDisplay != null) textDisplay.text = "";
        if (mainMenuButton != null) mainMenuButton.SetActive(false);

        // Start the slow-writing typewriter animation
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        // Loop through each character of the text one by one
        foreach (char letter in fullStoryText.ToCharArray())
        {
            if (textDisplay != null)
            {
                textDisplay.text += letter; // Add the next letter to the screen
            }
            
            // Wait a split second before drawing the next letter
            yield return new WaitForSeconds(typingSpeed); 
        }

        // Once the entire paragraph is finished typing, reveal the Main Menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.SetActive(true);
        }
    }

    // BUTTON FUNCTION: Connect this to your Return Button's OnClick event
    public void LoadMainMenu()
    {
        // Replace "MainMenu" with the exact name of your main menu scene file
        SceneManager.LoadScene("main-1"); 
    }
}