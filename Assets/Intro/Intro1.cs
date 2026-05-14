using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject pressAnyKeyUI; // ÇÓÍÈ äÕ "ÇÖÛØ Ãí ÒÑ" åäÇ
    private bool videoHasFinished = false;

    void Start()
    {
        // ÅÎİÇÁ äÕ "ÇÖÛØ Ãí ÒÑ" İí ÇáÈÏÇíÉ
        if (pressAnyKeyUI != null) pressAnyKeyUI.SetActive(false);

        // äÑÈØ ÇáßæÏ ÈÍÏË ÇäÊåÇÁ ÇáİíÏíæ
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoHasFinished = true;

        // ÅÙåÇÑ ÇáäÕ ÚäÏ ÇäÊåÇÁ ÇáİíÏíæ
        if (pressAnyKeyUI != null) pressAnyKeyUI.SetActive(true);
    }

    void Update()
    {
        // ÅĞÇ ÇäÊåì ÇáİíÏíæ æÖÛØ ÇáãÓÊÎÏã Ãí ÒÑ¡ ÇäÊŞá ááãÔåÏ ÇáÊÇáí
        if (videoHasFinished && Input.anyKeyDown)
        {
            // ÇÓÊÈÏá "MainMenu" ÈÇÓã ÇáãÔåÏ ÇáĞí ÊÑíÏ ÇáÇäÊŞÇá Åáíå
            SceneManager.LoadScene("SampleScene");
        }
    }
}