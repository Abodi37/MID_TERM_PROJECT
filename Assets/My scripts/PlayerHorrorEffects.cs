using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Screen and sound feedback for the player's state: the lower the sanity (or health),
// the darker, grainier and more distorted the screen gets, with a heartbeat that speeds up.
// PlayerStats adds this automatically and everything is built from code,
// so nothing has to be set up in the scene.
[RequireComponent(typeof(PlayerStats))]
public class PlayerHorrorEffects : MonoBehaviour
{
    [Header("When Effects Start")]
    [Tooltip("Effects fade in once sanity drops below this (0.7 = 70%)")]
    [Range(0f, 1f)] public float sanityThreshold = 0.7f;
    [Tooltip("Effects also kick in when health drops below this (0.3 = 30%)")]
    [Range(0f, 1f)] public float healthThreshold = 0.3f;

    [Header("Strength At Zero Sanity")]
    public float maxVignette = 0.55f;
    public Color vignetteColor = new Color(0.3f, 0f, 0f);
    public float maxChromaticAberration = 1f;
    public float maxFilmGrain = 0.8f;
    [Tooltip("Colors drain out of the world (-100 = black and white)")]
    public float minSaturation = -60f;

    [Header("Heartbeat")]
    [Tooltip("Leave empty to use a generated heartbeat sound")]
    public AudioClip heartbeatClip;
    [Range(0f, 1f)] public float heartbeatVolume = 0.8f;

    [Header("Hit Flash")]
    public Color hitFlashColor = new Color(0.6f, 0f, 0f, 0.55f);
    public float hitFlashDuration = 0.6f;

    private PlayerStats stats;
    private Volume volume;
    private VolumeProfile profile;
    private Vignette vignette;
    private AudioSource heartbeat;
    private GameObject flashCanvas;
    private Image flashImage;
    private float flashTimer;
    private float fear;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        CreateVolume();
        CreateHeartbeat();
        CreateHitFlash();
    }

    void Update()
    {
        float sanityFear = Mathf.Clamp01((sanityThreshold - stats.Sanity01) / Mathf.Max(0.01f, sanityThreshold));
        float healthFear = Mathf.Clamp01((healthThreshold - stats.Health01) / Mathf.Max(0.01f, healthThreshold));
        float targetFear = stats.isDead ? 1f : Mathf.Max(sanityFear, healthFear);

        // Smooth it so a sudden hit doesn't snap the screen
        fear = Mathf.MoveTowards(fear, targetFear, Time.unscaledDeltaTime * 0.5f);

        UpdateHitFlash();
        UpdateVolume();
        UpdateHeartbeat();
    }

    // Called by PlayerStats when a ghost hits the player
    public void PlayHitFlash()
    {
        flashTimer = hitFlashDuration;
    }

    void CreateVolume()
    {
        GameObject volumeObject = new GameObject("Horror Effects Volume");
        volumeObject.transform.SetParent(transform, false);
        volumeObject.layer = GetCameraVolumeLayer();

        volume = volumeObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100f;
        volume.weight = 0f;

        // Only the values set with Override() are touched, the rest of the scene's look stays the same.
        // The volume weight then blends between the normal look (0) and full terror (1).
        profile = ScriptableObject.CreateInstance<VolumeProfile>();
        volume.sharedProfile = profile;

        vignette = profile.Add<Vignette>();
        vignette.intensity.Override(maxVignette);
        vignette.smoothness.Override(0.45f);
        vignette.color.Override(vignetteColor);

        ChromaticAberration chromatic = profile.Add<ChromaticAberration>();
        chromatic.intensity.Override(maxChromaticAberration);

        FilmGrain grain = profile.Add<FilmGrain>();
        grain.type.Override(FilmGrainLookup.Medium2);
        grain.intensity.Override(maxFilmGrain);

        ColorAdjustments colorAdjustments = profile.Add<ColorAdjustments>();
        colorAdjustments.saturation.Override(minSaturation);
    }

    // The camera only reads volumes on certain layers, put ours on one of them
    int GetCameraVolumeLayer()
    {
        Camera cam = Camera.main;
        if (cam == null) return 0;

        UniversalAdditionalCameraData cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) return 0;

        int mask = cameraData.volumeLayerMask.value;
        for (int layer = 0; layer < 32; layer++)
        {
            if ((mask & (1 << layer)) != 0) return layer;
        }
        return 0;
    }

    void UpdateVolume()
    {
        float flash = hitFlashDuration > 0f ? flashTimer / hitFlashDuration : 0f;

        // The vignette breathes with the heartbeat
        float beatsPerSecond = heartbeat != null ? heartbeat.pitch : 1f;
        float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2f * beatsPerSecond);
        vignette.intensity.value = maxVignette * (0.85f + 0.15f * pulse * fear);

        volume.weight = Mathf.Max(fear, flash);
    }

    void CreateHeartbeat()
    {
        heartbeat = gameObject.AddComponent<AudioSource>();
        heartbeat.clip = heartbeatClip != null ? heartbeatClip : CreateHeartbeatClip();
        heartbeat.loop = true;
        heartbeat.playOnAwake = false;
        heartbeat.spatialBlend = 0f;
        heartbeat.volume = 0f;
    }

    void UpdateHeartbeat()
    {
        bool shouldPlay = fear > 0.05f && !stats.isDead && Time.timeScale > 0f;

        if (shouldPlay)
        {
            heartbeat.volume = heartbeatVolume * Mathf.Clamp01(fear * 1.5f);
            heartbeat.pitch = Mathf.Lerp(0.85f, 1.7f, fear); // faster beat when more scared
            if (!heartbeat.isPlaying) heartbeat.Play();
        }
        else if (heartbeat.isPlaying)
        {
            heartbeat.Stop();
        }
    }

    // Two low "lub-dub" thumps, one second long, so it loops as a heartbeat
    AudioClip CreateHeartbeatClip()
    {
        const int sampleRate = 44100;
        float[] samples = new float[sampleRate];

        AddThump(samples, sampleRate, 0f, 1f);
        AddThump(samples, sampleRate, 0.28f, 0.7f);

        AudioClip clip = AudioClip.Create("Heartbeat", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    void AddThump(float[] samples, int sampleRate, float startTime, float amplitude)
    {
        int start = (int)(startTime * sampleRate);
        int length = (int)(0.2f * sampleRate);

        for (int i = 0; i < length && start + i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Exp(-t * 25f) * (1f - Mathf.Exp(-t * 400f));
            float frequency = Mathf.Lerp(60f, 40f, t / 0.2f);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * t) + 0.35f * Mathf.Sin(4f * Mathf.PI * frequency * t);
            samples[start + i] += amplitude * envelope * wave * 0.8f;
        }
    }

    void CreateHitFlash()
    {
        flashCanvas = new GameObject("Hit Flash Canvas");
        Canvas canvas = flashCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 400;

        GameObject imageObject = new GameObject("Hit Flash");
        imageObject.transform.SetParent(flashCanvas.transform, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        flashImage = imageObject.AddComponent<Image>();
        flashImage.raycastTarget = false;
        flashImage.color = Color.clear;
    }

    void UpdateHitFlash()
    {
        if (flashTimer <= 0f) return;

        // Unscaled, so the flash still fades out on the death screen (time is frozen there)
        flashTimer = Mathf.Max(0f, flashTimer - Time.unscaledDeltaTime);
        Color color = hitFlashColor;
        color.a *= flashTimer / hitFlashDuration;
        flashImage.color = color;
    }

    void OnDestroy()
    {
        if (profile != null) Destroy(profile);
        if (flashCanvas != null) Destroy(flashCanvas);
    }
}
