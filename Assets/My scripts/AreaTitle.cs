using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Cinematic title card when the player enters an area, like "The Garden" or "The Mansion".
// Letters fade in one by one while rising slightly, the spacing slowly closes,
// then everything dissolves letter by letter and drifts up.
// Just put it on any GameObject in the scene and fill in the text: the UI is built from code.
public class AreaTitle : MonoBehaviour
{
    [Header("Text")]
    public string title = "The Garden";
    [Tooltip("Small line above the title, e.g. \"Chapter I\" (leave empty to hide it)")]
    public string chapter = "Chapter I";

    [Header("Look")]
    public TMP_FontAsset titleFont;
    public TMP_FontAsset chapterFont;
    public float titleSize = 120f;
    public float chapterSize = 26f;
    public Color textColor = new Color(0.93f, 0.89f, 0.82f);
    [Tooltip("Height of the title from the center of the screen")]
    public float verticalOffset = 60f;

    [Header("Timing (seconds)")]
    [Tooltip("Wait for the scene's fade from black before showing the title")]
    public float startDelay = 1.5f;
    [Tooltip("How long one letter takes to fade in")]
    public float letterFadeTime = 0.9f;
    [Tooltip("Time between two letters starting to appear")]
    public float letterDelay = 0.09f;
    public float holdTime = 2.5f;
    public float fadeOutTime = 1.8f;

    [Header("Extra")]
    [Tooltip("Optional sound when the title appears")]
    public AudioClip appearSound;
    [Range(0f, 1f)] public float soundVolume = 0.7f;
    [Tooltip("Don't show it again when the scene restarts after dying")]
    public bool onlyOncePerSession = true;

    private static readonly HashSet<string> shownThisSession = new HashSet<string>();

    private GameObject canvasObject;
    private RectTransform card;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI chapterText;
    private Image line;
    private Material titleMaterial;

    const float LineWidth = 520f;
    const float LetterRise = 18f;
    const float DriftUp = 30f;

    void Start()
    {
        string key = gameObject.scene.name + "/" + title;
        if (onlyOncePerSession && shownThisSession.Contains(key)) return;
        shownThisSession.Add(key);

        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        // Realtime, so it still plays if the game happens to be paused (tutorial panel...)
        yield return new WaitForSecondsRealtime(startDelay);

        BuildUI();
        if (appearSound != null)
        {
            AudioSource source = canvasObject.AddComponent<AudioSource>();
            source.spatialBlend = 0f;
            source.PlayOneShot(appearSound, soundVolume);
        }

        titleText.ForceMeshUpdate();
        int letters = Mathf.Max(1, titleText.textInfo.characterCount);
        float revealTime = letterFadeTime + letterDelay * (letters - 1);
        float totalTime = revealTime + holdTime + fadeOutTime;
        Vector2 basePosition = card.anchoredPosition;

        float time = 0f;
        while (time < totalTime)
        {
            time += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(time / totalTime);
            float fadeOut = Mathf.Clamp01((time - revealTime - holdTime) / fadeOutTime);

            // The whole card drifts up while it disappears
            card.anchoredPosition = basePosition + Vector2.up * (EaseIn(fadeOut) * DriftUp);

            // Letters slowly come together during the whole show
            titleText.characterSpacing = Mathf.Lerp(28f, 4f, EaseOut(progress));
            AnimateLetters(time, fadeOut, letters);

            // Chapter line and underline appear a bit after the first letters
            float sideFade = Smooth(Mathf.Clamp01((time - 0.3f) / 1.2f)) * (1f - Smooth(fadeOut));
            if (chapterText != null)
            {
                chapterText.characterSpacing = Mathf.Lerp(60f, 28f, EaseOut(progress));
                chapterText.alpha = 0.75f * sideFade;
            }

            float lineGrow = EaseOut(Mathf.Clamp01((time - 0.4f) / revealTime));
            line.rectTransform.sizeDelta = new Vector2(LineWidth * lineGrow, 2f);
            Color lineColor = textColor;
            lineColor.a = 0.55f * sideFade;
            line.color = lineColor;

            yield return null;
        }

        Destroy(canvasObject);
    }

    // Each letter fades in (and rises) on its own, then they dissolve in the same order
    void AnimateLetters(float time, float fadeOut, int letters)
    {
        titleText.ForceMeshUpdate();
        TMP_TextInfo info = titleText.textInfo;

        for (int i = 0; i < info.characterCount; i++)
        {
            TMP_CharacterInfo character = info.characterInfo[i];
            if (!character.isVisible) continue;

            float appear = Smooth(Mathf.Clamp01((time - i * letterDelay) / letterFadeTime));

            // Dissolve: the first letters start disappearing first
            float dissolveStart = letters > 1 ? (float)i / (letters - 1) * 0.5f : 0f;
            float dissolve = Smooth(Mathf.Clamp01((fadeOut - dissolveStart) / 0.5f));

            float alpha = appear * (1f - dissolve) * textColor.a;
            float rise = (1f - appear) * -LetterRise + dissolve * LetterRise * 0.5f;

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;
            Color32[] colors = info.meshInfo[materialIndex].colors32;
            Vector3[] vertices = info.meshInfo[materialIndex].vertices;

            for (int v = 0; v < 4; v++)
            {
                colors[vertexIndex + v].a = (byte)(alpha * 255f);
                vertices[vertexIndex + v].y += rise;
            }
        }

        titleText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
    }

    void BuildUI()
    {
        canvasObject = new GameObject("Area Title Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;

        // Same size on every screen resolution
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        card = CreateRect("Card", canvasObject.transform, new Vector2(1600f, 400f), new Vector2(0f, verticalOffset));

        titleText = CreateText("Title", title, titleFont, titleSize, new Vector2(0f, 0f));
        titleText.fontStyle = FontStyles.Normal;

        // Soft dark shadow so the title stays readable over bright places
        titleMaterial = titleText.fontMaterial;
        titleMaterial.EnableKeyword(ShaderUtilities.Keyword_Underlay);
        titleMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0f, 0f, 0f, 0.75f));
        titleMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.6f);
        titleMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.2f);

        if (!string.IsNullOrEmpty(chapter))
        {
            chapterText = CreateText("Chapter", chapter, chapterFont, chapterSize, new Vector2(0f, titleSize * 0.85f));
            chapterText.fontStyle = FontStyles.UpperCase;
            chapterText.alpha = 0f;
        }

        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(card, false);
        line = lineObject.AddComponent<Image>();
        line.raycastTarget = false;
        line.rectTransform.anchoredPosition = new Vector2(0f, -titleSize * 0.6f);
        line.rectTransform.sizeDelta = new Vector2(0f, 2f);
        line.color = Color.clear;
    }

    RectTransform CreateRect(string name, Transform parent, Vector2 size, Vector2 position)
    {
        GameObject rectObject = new GameObject(name);
        rectObject.transform.SetParent(parent, false);
        RectTransform rect = rectObject.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    TextMeshProUGUI CreateText(string name, string content, TMP_FontAsset font, float size, Vector2 position)
    {
        RectTransform rect = CreateRect(name, card, new Vector2(1600f, size * 1.6f), position);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.text = content;
        text.fontSize = size;
        text.color = textColor;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        return text;
    }

    static float Smooth(float t) => t * t * (3f - 2f * t);
    static float EaseOut(float t) => 1f - (1f - t) * (1f - t) * (1f - t);
    static float EaseIn(float t) => t * t;

    void OnDestroy()
    {
        if (titleMaterial != null) Destroy(titleMaterial);
        if (canvasObject != null) Destroy(canvasObject);
    }
}
