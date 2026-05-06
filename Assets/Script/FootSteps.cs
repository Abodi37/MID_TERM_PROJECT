using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSystem : MonoBehaviour
{
    [Header("Footstep Audio Clips")]
    public AudioClip[] footstepSounds;

    [Header("Settings")]
    public float baseStepRate = 0.5f;
    [Range(0f, 0.2f)] public float volumeVariance = 0.1f;
    [Range(0f, 0.2f)] public float pitchVariance = 0.1f;

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();

        // ≈Ã»«— «·„Õ—ﬂ ⁄·Ï  ⁄ÿÌ· Â–Â «·ŒÌ«—«  · Ã‰» √Ì  œ«Œ·
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        // 1. «·Õ· «·Ã–—Ì: Õ”«» «·”—⁄… «·√›ﬁÌ… ›ﬁÿ (X Ê Z) Ê Ã«Â· «·Ã«–»Ì… (Y)
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // 2. «· Õﬁﬁ „‰ √‰ «··«⁄» ⁄·Ï «·√—÷ ÊÌ Õ—ﬂ ›⁄·Ì«
        if (!controller.isGrounded || currentSpeed < 0.1f)
        {
            stepTimer = 0f; //  ’›Ì— «·„ƒﬁ  ·ÌﬂÊ‰ Ã«Â“« ··ŒÿÊ… «·ﬁ«œ„… ›Ê—«
            return;
        }

        // 3. Õ”«» „⁄œ· «·ŒÿÊ«  »‰«¡ ⁄·Ï «·”—⁄… «·Õ«·Ì…
        float currentStepRate = baseStepRate / (currentSpeed / 5f);

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayRandomFootstep();
            stepTimer = currentStepRate;
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        AudioClip clipToPlay = footstepSounds[randomIndex];

        audioSource.volume = 1f - Random.Range(0f, volumeVariance);
        audioSource.pitch = 1f - Random.Range(-pitchVariance, pitchVariance);

        audioSource.PlayOneShot(clipToPlay);
    }
}