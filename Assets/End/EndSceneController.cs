using UnityEngine;
using UnityEngine.Video;

public class EndSceneController : MonoBehaviour
{
    [Header("Components")]
    public VideoPlayer videoPlayer;
    public GameObject toBeContinuedUI; 

    void Start()
    {
        
        if (toBeContinuedUI != null)
        {
            toBeContinuedUI.SetActive(false);
        }

        
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned!");
        }
    }

    
    void OnVideoFinished(VideoPlayer vp)
    {
        if (toBeContinuedUI != null)
        {
            toBeContinuedUI.SetActive(true);
        }
    }

    void OnDestroy()
    {
        
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}