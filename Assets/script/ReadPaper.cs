using UnityEngine;

public class ReadPaper : MonoBehaviour
{
    // مرجع لمكون الصوت
    [SerializeField] private AudioSource paperAudioSource;

    // مرجع لواجهة الرسالة (الـ Canvas أو الـ GameObject الخاص بالورقة)
    [SerializeField] private GameObject paperUI;

    public void OpenPaper()
    {
        // 1. إظهار الرسالة للملاعب
        paperUI.SetActive(true);

        // 2. التحقق من وجود الصوت وتشغيله
        if (paperAudioSource != null)
        {
            paperAudioSource.Play();
        }
    }

    public void ClosePaper()
    {
        paperUI.SetActive(false);

        // اختياري: إيقاف الصوت إذا أغلق اللاعب الرسالة فجأة
        if (paperAudioSource != null && paperAudioSource.isPlaying)
        {
            paperAudioSource.Stop();
        }
    }
}