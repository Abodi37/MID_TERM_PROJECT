using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float sanity = 100f;

    [Header("Rates")]
    public float baseSanityDrop = 2f;
    public float sanityRegenRate = 1f;
    public float damageRateAtZeroSanity = 5f;

    [Header("Ghost Proximity")]
    [Tooltip("Ghosts inside this distance drain sanity faster the closer they get")]
    public float sanityDrainRadius = 12f;
    [Tooltip("Drain multiplier for a ghost standing right next to you")]
    public float closeGhostMultiplier = 3f;
    [Tooltip("Drain multiplier for a ghost outside the radius (0 = far ghosts don't drain at all)")]
    [Range(0f, 1f)] public float farGhostMultiplier = 0.25f;

    [Header("Healing")]
    public float healAmount = 100f;
    public float lowHealthThreshold = 30f;

    [Header("UI Images")]
    public Image healthBarImage;
    public Image sanityBarImage;

    [Header("Prompt UI")]
    public GameObject healPromptUI;

    [Header("Death")]
    public GameObject deathScreenUI;
    public bool isDead = false;

    [Header("Camera")]
    public CinemachineCamera virtualCamera;

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    public EnemyManager enemyManager;

    [Header("Feedback")]
    [Tooltip("Screen effects (vignette, grain, heartbeat...) that get stronger as sanity drops")]
    public bool enableHorrorEffects = true;

    private PlayerHorrorEffects horrorEffects;

    public float Health01 => Mathf.Clamp01(health / 100f);
    public float Sanity01 => Mathf.Clamp01(sanity / 100f);

    void Start()
    {
        isDead = false;

        Time.timeScale = 1f;

        if (enableHorrorEffects)
        {
            horrorEffects = GetComponent<PlayerHorrorEffects>();
            if (horrorEffects == null) horrorEffects = gameObject.AddComponent<PlayerHorrorEffects>();
        }
    }

    void Update()
    {
        if (isDead) return;

        float ghostPressure = GetGhostPressure();

        if (ghostPressure > 0f)
        {
            sanity -= baseSanityDrop * ghostPressure * Time.deltaTime;
        }
        else
        {
            sanity += sanityRegenRate * Time.deltaTime;
        }

        sanity = Mathf.Clamp(sanity, 0, 100);

        if (sanity <= 0 && ghostPressure > 0f)
        {
            health -= damageRateAtZeroSanity * ghostPressure * Time.deltaTime;
        }

        bool hasPill = inventoryManager != null && inventoryManager.HasItem("Pill");

        // No healing from inside the pause menu
        if (Time.timeScale > 0f && Input.GetKeyDown(KeyCode.H) && hasPill && health < 100f)
        {
            Heal();
        }

        if (healPromptUI != null)
        {
            healPromptUI.SetActive(health <= lowHealthThreshold && hasPill);
        }

        UpdateBars();

        if (health <= 0)
        {
            Die();
        }
    }

    // Every ghost adds "pressure": a lot when it's close, a little when it's far away
    float GetGhostPressure()
    {
        if (enemyManager == null) return 0f;

        float pressure = 0f;
        foreach (GameObject ghost in enemyManager.activeEnemies)
        {
            if (ghost == null) continue;

            Enemy enemy = ghost.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsThreatening) continue;

            float distance = Vector3.Distance(transform.position, ghost.transform.position);
            if (distance > sanityDrainRadius)
            {
                pressure += farGhostMultiplier;
            }
            else
            {
                float closeness = 1f - (distance / sanityDrainRadius);
                pressure += Mathf.Lerp(1f, closeGhostMultiplier, closeness);
            }
        }
        return pressure;
    }

    void UpdateBars()
    {
        if (healthBarImage != null) healthBarImage.fillAmount = Health01;
        if (sanityBarImage != null) sanityBarImage.fillAmount = Sanity01;
    }

    // Called by a ghost when it reaches the player
    public void TakeHit(float healthDamage, float sanityDamage)
    {
        if (isDead) return;

        health = Mathf.Max(0f, health - healthDamage);
        sanity = Mathf.Max(0f, sanity - sanityDamage);
        UpdateBars();

        if (horrorEffects != null) horrorEffects.PlayHitFlash();

        if (health <= 0)
        {
            Die();
        }
    }

    public void RestoreSanity(float amount)
    {
        if (isDead) return;
        sanity = Mathf.Clamp(sanity + amount, 0f, 100f);
    }

    void Heal()
    {
        health = Mathf.Min(100f, health + healAmount);

        inventoryManager.RemoveItem("Pill");
    }

    void Die()
    {
        if (isDead) return;

        health = 0f;
        UpdateBars();

        if (deathScreenUI != null) deathScreenUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        isDead = true;

        if (virtualCamera != null) virtualCamera.enabled = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
