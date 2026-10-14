using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject pressAnyKeyUI; // اسحب نص "اضغط أي زر" هنا
    private bool videoHasFinished = false;

    [Tooltip("المشهد اللي ينفتح بعد المقدمة")]
    public string nextSceneName = "SampleScene";
    [Tooltip("زر تخطي المقدمة")]
    public KeyCode skipKey = KeyCode.Escape;

    void Start()
    {
        // إخفاء نص "اضغط أي زر" في البداية
        if (pressAnyKeyUI != null) pressAnyKeyUI.SetActive(false);

        if (videoPlayer == null)
        {
            // ما فيه فيديو، نعتبر المقدمة خلصت
            OnVideoEnd(null);
            return;
        }

        // نربط الكود بحدث انتهاء الفيديو
        videoPlayer.loopPointReached += OnVideoEnd;

        // لو الفيديو ما اشتغل (ملف ناقص مثلاً) لا نعلق اللاعب في شاشة سوداء
        videoPlayer.errorReceived += OnVideoError;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoHasFinished = true;

        // إظهار النص عند انتهاء الفيديو
        if (pressAnyKeyUI != null) pressAnyKeyUI.SetActive(true);
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogWarning("Intro video failed: " + message);
        OnVideoEnd(vp);
    }

    void Update()
    {
        // تخطي المقدمة في أي وقت
        if (Input.GetKeyDown(skipKey))
        {
            LoadNextScene();
            return;
        }

        // إذا انتهى الفيديو وضغط المستخدم أي زر، انتقل للمشهد التالي
        if (videoHasFinished && Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        enabled = false; // عشان ما ينطلب تحميل المشهد مرتين
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.errorReceived -= OnVideoError;
        }
    }
}
