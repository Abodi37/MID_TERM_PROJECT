using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 3f; // كم ثانية يستغرق الصوت ليصل لأعلى قوة؟
    public float targetVolume = 1f; // أعلى مستوى تبغى الصوت يوصل له (من 0 إلى 1)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // نبدأ بالصوت صفر تماماً
        audioSource.volume = 0f;

        // تشغيل الكوروتين المسؤول عن رفع الصوت بالتدريج
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // رفع الصوت تدريجياً بناءً على الوقت
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }

        // التأكيد على ثبات الصوت عند القيمة المطلوبة في النهاية
        audioSource.volume = targetVolume;
    }
}