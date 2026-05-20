using UnityEngine;

public class ReadPaper : MonoBehaviour
{
    // „—Ã⁄ ·„ﬂÊ‰ «·’Ê 
    [SerializeField] private AudioSource paperAudioSource;

    // „—Ã⁄ ·Ê«ÃÂ… «·—”«·… («·‹ Canvas √Ê «·‹ GameObject «·Œ«’ »«·Ê—ﬁ…)
    [SerializeField] private GameObject paperUI;

    public void OpenPaper()
    {
        // 1. ≈ŸÂ«— «·—”«·… ··„·«⁄»
        paperUI.SetActive(true);

        // 2. «· Õﬁﬁ „‰ ÊÃÊœ «·’Ê  Ê ‘€Ì·Â
        if (paperAudioSource != null)
        {
            paperAudioSource.Play();
        }
    }

    public void ClosePaper()
    {
        paperUI.SetActive(false);

        // «Œ Ì«—Ì: ≈Ìﬁ«› «·’Ê  ≈–« √€·ﬁ «··«⁄» «·—”«·… ›Ã√…
        if (paperAudioSource != null && paperAudioSource.isPlaying)
        {
            paperAudioSource.Stop();
        }
    }
}