using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject endingChoicePanel;
    public CanvasGroup fadeScreenCanvasGroup; // Drag your FadeScreen Canvas Group here

    [Header("Fade Settings")]
    public float fadeDuration = 2f; // How many seconds the fade takes

    void Start()
    {
        if (endingChoicePanel != null) endingChoicePanel.SetActive(false);
        if (fadeScreenCanvasGroup != null) fadeScreenCanvasGroup.gameObject.SetActive(false);
    }

    public void OpenEndingMenu()
    {
        if (endingChoicePanel != null)
        {
            endingChoicePanel.SetActive(true);
            
            // 1. FREEZE EVERYTHING: Stops player movement, enemy AI, animations, and physics instantly!
            Time.timeScale = 0f; 
            
            // 2. Unlock mouse cursor so they can select an ending choice
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Connect this to your 'Stay with Memories' Button
    public void ChooseStayEnding()
    {
        StartCoroutine(FadeAndLoadScene("Ending_Stay"));
    }

    // Connect this to your 'Move On' Button
    public void ChooseMoveOnEnding()
    {
        StartCoroutine(FadeAndLoadScene("Ending_MoveOn"));
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Hide the choice buttons so the screen feels clean during the fade
        if (endingChoicePanel != null) endingChoicePanel.SetActive(false);
        
        // Turn on the fade screen object
        if (fadeScreenCanvasGroup != null)
        {
            fadeScreenCanvasGroup.gameObject.SetActive(true);
            float timer = 0;

            // Smoothly fade the black image alpha from 0 to 1
            while (timer < fadeDuration)
            {
                // CRUCIAL CHANGE: Using unscaledDeltaTime allows this loop to run 
                // perfectly even though Time.timeScale is completely at 0!
                timer += Time.unscaledDeltaTime; 
                fadeScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null; 
            }
        }

        // 3. RESET TIME MATRIX: Always unfreeze time right before loading a new scene!
        Time.timeScale = 1f; 
        SceneManager.LoadScene(sceneName);
    }
}
