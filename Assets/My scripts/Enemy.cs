using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;

    private EnemyManager manager;
    public Transform playerTransform;
    public GameObject deathEffect;
    [Header("Audio settings")]
    [Tooltip("Plays while the flashlight beam is burning the ghost")]
    public AudioSource attackSound;
    [Tooltip("Looping whisper that gets louder as the ghost comes closer")]
    public AudioSource ambientSound;
    [Tooltip("One of these plays when the ghost hits the player")]
    public AudioClip[] hitPlayerClips;

    [Header("Chase")]
    public float chaseSpeed = 1.8f;
    [Tooltip("Speed multiplier while the normal flashlight beam is on the ghost")]
    [Range(0f, 1f)] public float litSpeedMultiplier = 0.4f;
    [Tooltip("Speed multiplier while the focused beam (right mouse) is burning the ghost")]
    [Range(0f, 1f)] public float burningSpeedMultiplier = 0.05f;
    [Tooltip("If there's no walkable path to the player for this long, the ghost floats straight through walls")]
    public float phaseThroughWallsAfter = 3f;

    [Header("Attack")]
    public float attackRange = 1.8f;
    public float healthDamage = 20f;
    public float sanityDamage = 15f;
    [Tooltip("After hitting the player the ghost vanishes for a while and comes back somewhere else")]
    public float vanishTimeMin = 4f;
    public float vanishTimeMax = 8f;
    public float reappearDistance = 10f;

    [Header("Reward")]
    [Tooltip("Sanity the player gets back when this ghost is destroyed")]
    public float sanityRewardOnKill = 10f;

    private NavMeshAgent agent;
    private PlayerStats playerStats;
    private FlashlightAttack flashlight;
    private Renderer[] renderers;
    private Collider[] colliders;
    private Vector3 baseScale;

    private bool isVanished;
    private bool isPhasing;
    private float noPathTimer;
    private float hoverHeight = 1.5f;
    private float vanishTimer;
    private float lastDamageTime = -10f;
    private float nextRepathTime;
    private float nextIdleFlickerTime;
    private float idleFlickerEndTime;

    void Start()
    {
        manager = FindFirstObjectByType<EnemyManager>();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
        if (playerTransform != null) playerStats = playerTransform.GetComponentInParent<PlayerStats>();
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        // The flashlight object stays switched off until it's picked up, so search inactive objects too
        flashlight = FindFirstObjectByType<FlashlightAttack>(FindObjectsInactive.Include);

        if (attackSound == null)
        {
            attackSound = GetComponent<AudioSource>();
        }
        SetupAudio();

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        baseScale = transform.localScale;
        nextIdleFlickerTime = Time.time + Random.Range(2f, 6f);

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // How high the ghost floats above the floor, kept when it flies through walls
            hoverHeight = agent.baseOffset * transform.lossyScale.y;

            // EnemyBillboard turns the ghost to face the camera, so the agent must not rotate it
            agent.updateRotation = false;
            agent.stoppingDistance = attackRange * 0.5f;
            PlaceOnNavMesh(transform.position);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (playerStats != null && playerStats.isDead)
        {
            StopMoving();
            return;
        }

        if (isVanished)
        {
            vanishTimer -= Time.deltaTime;
            if (vanishTimer <= 0f) Reappear();
            return;
        }

        bool isBurning = Time.time - lastDamageTime < 0.15f;
        bool isLit = !isBurning && flashlight != null && flashlight.IsLighting(transform);

        float speed = chaseSpeed;
        if (isBurning) speed *= burningSpeedMultiplier;
        else if (isLit) speed *= litSpeedMultiplier;

        ChasePlayer(speed);
        UpdateGlitch(isBurning);

        if (IsInReach())
        {
            HitPlayer();
        }
    }

    void ChasePlayer(float speed)
    {
        if (!isPhasing && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.speed = speed;
            if (Time.time >= nextRepathTime)
            {
                agent.SetDestination(playerTransform.position);
                nextRepathTime = Time.time + 0.25f;
            }

            // The house NavMesh has gaps (narrow doors, stairs...). If the player can't be reached,
            // don't stand there stuck: after a moment start floating through the walls instead.
            // (While a new path is being calculated we don't know yet, so the timer just waits.)
            if (!agent.pathPending)
            {
                bool noFullPath = agent.pathStatus != NavMeshPathStatus.PathComplete;
                bool standingStill = speed > 0.5f && agent.velocity.sqrMagnitude < 0.04f;
                noPathTimer = noFullPath || standingStill ? noPathTimer + Time.deltaTime : 0f;
            }
            if (noPathTimer >= phaseThroughWallsAfter) StartPhasing();
        }
        else
        {
            // No NavMesh path: it's a ghost, so it just floats straight through everything
            Vector3 target = playerTransform.position + Vector3.up * hoverHeight;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    void StartPhasing()
    {
        isPhasing = true;
        noPathTimer = 0f;
        if (agent != null) agent.enabled = false;
    }

    // Close enough to hit, and on the same floor (not through the ceiling)
    bool IsInReach()
    {
        Vector3 offset = playerTransform.position - transform.position;
        float heightDifference = Mathf.Abs(offset.y);
        offset.y = 0f;
        return offset.magnitude <= attackRange && heightDifference <= hoverHeight + 1.5f;
    }

    void HitPlayer()
    {
        if (playerStats != null) playerStats.TakeHit(healthDamage, sanityDamage);

        if (hitPlayerClips != null && hitPlayerClips.Length > 0)
        {
            AudioClip clip = hitPlayerClips[Random.Range(0, hitPlayerClips.Length)];
            if (clip != null) AudioSource.PlayClipAtPoint(clip, playerTransform.position);
        }

        Vanish();
    }

    void Vanish()
    {
        isVanished = true;
        vanishTimer = Random.Range(vanishTimeMin, vanishTimeMax);
        StopAttackSound();
        StopMoving();
        SetVisible(false);
        if (ambientSound != null) ambientSound.Pause();
    }

    void Reappear()
    {
        Vector3 position = transform.position;
        if (manager != null && manager.TryGetReappearPoint(reappearDistance, out Vector3 point))
        {
            position = point;
        }

        // Back to walking on the NavMesh (if there is one where it reappears)
        isPhasing = false;
        noPathTimer = 0f;
        if (agent != null) PlaceOnNavMesh(position);
        else transform.position = position;

        isVanished = false;
        SetVisible(true);
        if (ambientSound != null) ambientSound.UnPause();
    }

    void PlaceOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 4f, NavMesh.AllAreas))
        {
            if (!agent.enabled)
            {
                // Move first, so the agent wakes up already standing on the NavMesh
                transform.position = hit.position;
                agent.enabled = true;
            }
            agent.Warp(hit.position);
        }
        else
        {
            // Off the NavMesh the agent would only throw errors, use the straight-line fallback instead
            agent.enabled = false;
            transform.position = position;
        }
    }

    void StopMoving()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = visible;
        }
        foreach (Collider c in colliders)
        {
            if (c != null) c.enabled = visible;
        }
    }

    void UpdateGlitch(bool isBurning)
    {
        // Ghost shrinks as it loses health, so the player can see the damage working
        float healthPercent = maxHealth > 0f ? Mathf.Clamp01(health / maxHealth) : 1f;
        Vector3 scale = baseScale * Mathf.Lerp(0.6f, 1f, healthPercent);

        bool visible = true;
        if (isBurning)
        {
            // Violent flicker + shake while burning
            visible = Random.value > 0.3f;
            scale *= 1f + Random.Range(-0.08f, 0.08f);
        }
        else if (Time.time >= nextIdleFlickerTime)
        {
            // Every few seconds a short blink, just to be creepy
            idleFlickerEndTime = Time.time + Random.Range(0.05f, 0.2f);
            nextIdleFlickerTime = Time.time + Random.Range(2f, 6f);
        }

        if (Time.time < idleFlickerEndTime) visible = Random.value > 0.5f;

        transform.localScale = scale;
        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = visible;
        }
    }

    void SetupAudio()
    {
        if (ambientSound == null)
        {
            foreach (AudioSource source in GetComponents<AudioSource>())
            {
                if (source != attackSound) ambientSound = source;
            }
        }

        // 3D sound so the player can hear where the ghost is coming from
        if (ambientSound != null)
        {
            ambientSound.loop = true;
            ambientSound.spatialBlend = 1f;
            ambientSound.rolloffMode = AudioRolloffMode.Linear;
            ambientSound.minDistance = 1f;
            ambientSound.maxDistance = 15f;
            if (ambientSound.resource != null && !ambientSound.isPlaying) ambientSound.Play();
        }
        if (attackSound != null)
        {
            attackSound.loop = true;
            attackSound.spatialBlend = 1f;
            attackSound.maxDistance = 25f;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isVanished) return;

        health -= amount;
        lastDamageTime = Time.time;

        if (health <= 0)
        {
            Evaporate();
        }
    }

    void Evaporate()
    {
        float heightOffset = -1f;
        Vector3 spawnPos = transform.position + new Vector3(0, heightOffset, 0);

         if (deathEffect != null)
        {
            Instantiate(deathEffect, spawnPos, Quaternion.Euler(-90, 0, 0));
        }

        if (playerStats != null) playerStats.RestoreSanity(sanityRewardOnKill);

        if (manager != null)
        {
            manager.EnemyKilled(gameObject);
        }

        Destroy(gameObject);
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && !attackSound.isPlaying)
        {
            attackSound.Play();
        }
    }

    public void StopAttackSound()
    {
        if (attackSound != null && attackSound.isPlaying)
        {
            attackSound.Stop();
        }
    }

    // Used by PlayerStats: a vanished ghost doesn't scare anyone
    public bool IsThreatening => !isVanished;
}
