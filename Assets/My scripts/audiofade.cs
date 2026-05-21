using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 3f; // ßã ËÇäíÉ íÓÊÛÑŞ ÇáÕæÊ áíÕá áÃÚáì ŞæÉ¿
    public float targetVolume = 1f; // ÃÚáì ãÓÊæì ÊÈÛì ÇáÕæÊ íæÕá áå (ãä 0 Åáì 1)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // äÈÏÃ ÈÇáÕæÊ ÕİÑ ÊãÇãÇğ
        audioSource.volume = 0f;

        // ÊÔÛíá ÇáßæÑæÊíä ÇáãÓÄæá Úä ÑİÚ ÇáÕæÊ ÈÇáÊÏÑíÌ
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // ÑİÚ ÇáÕæÊ ÊÏÑíÌíÇğ ÈäÇÁğ Úáì ÇáæŞÊ
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }

        // ÇáÊÃßíÏ Úáì ËÈÇÊ ÇáÕæÊ ÚäÏ ÇáŞíãÉ ÇáãØáæÈÉ İí ÇáäåÇíÉ
        audioSource.volume = targetVolume;
    }
}