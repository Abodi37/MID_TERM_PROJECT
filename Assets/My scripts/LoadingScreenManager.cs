using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public string sceneToLoad;
    public Image loadingBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI promptText;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            loadingBar.fillAmount = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

             if (operation.progress >= 0.9f)
            {
                promptText.text = "Press 'Space' to Start"; 

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
