using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject endingChoicePanel;
    public CanvasGroup fadeScreenCanvasGroup; // Drag your FadeScreen Canvas Group here

    [Header("Player Tracking")]
    public PlayerStats playerStats; 

    [Header("Fade Settings")]
    public float fadeDuration = 2f; // How many seconds the fade to black takes

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
            
            // Freeze movement but keep Time.timeScale at 1 so UI animations and Coroutines can run
            if (playerStats != null) playerStats.isDead = true; 
            
            // Unlock mouse cursor so they can select an ending choice
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Connect this to your 'Stay with Memories' Button
    public void ChooseStayEnding()
    {
        StartCoroutine(FadeAndLoadScene("StayEnding"));
    }

    // Connect this to your 'Move On' Button
    public void ChooseMoveOnEnding()
    {
        StartCoroutine(FadeAndLoadScene("MoveOnEnding"));
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 1. Hide the choice buttons so the screen feels clean during the fade
        if (endingChoicePanel != null) endingChoicePanel.SetActive(false);
        
        // 2. Turn on the fade screen object
        if (fadeScreenCanvasGroup != null)
        {
            fadeScreenCanvasGroup.gameObject.SetActive(true);
            float timer = 0;

            // 3. Smoothly fade the black image alpha from 0 to 1
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null; // Wait for the next frame
            }
        }

        // 4. Load the typewriter scene after the screen is completely pitch black
        SceneManager.LoadScene(sceneName);
    }
}
