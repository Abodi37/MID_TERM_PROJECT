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
    [Tooltip("Space, Enter or a mouse click shows the whole text at once")]
    public bool allowSkip = true;

    [Header("Menu Button Reference")]
    public GameObject mainMenuButton;

    private bool skipRequested = false;

    void Start()
    {
        // Ensure text starts empty and button stays hidden
        if (mainMenuButton != null) mainMenuButton.SetActive(false);

        // The player needs the mouse to press the menu button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start the slow-writing typewriter animation
        StartCoroutine(TypeText());
    }

    void Update()
    {
        if (allowSkip && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            skipRequested = true;
        }
    }

    IEnumerator TypeText()
    {
        if (textDisplay != null)
        {
            // The whole text is set once and revealed letter by letter,
            // this also keeps rich text tags like <b> working
            textDisplay.text = fullStoryText;
            textDisplay.maxVisibleCharacters = 0;
            textDisplay.ForceMeshUpdate();

            int totalCharacters = textDisplay.textInfo.characterCount;
            for (int i = 1; i <= totalCharacters && !skipRequested; i++)
            {
                textDisplay.maxVisibleCharacters = i;

                // Realtime, so it still types even if the game was left frozen
                yield return new WaitForSecondsRealtime(typingSpeed);
            }

            textDisplay.maxVisibleCharacters = int.MaxValue;
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
        Time.timeScale = 1f;
        // Replace "MainMenu" with the exact name of your main menu scene file
        SceneManager.LoadScene("main-1");
    }
}
