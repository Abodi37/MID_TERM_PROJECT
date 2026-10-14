using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightAttack : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [Tooltip("Drag your active 'Flashlight' hand GameObject from the hierarchy here")]
    public GameObject handsFlashlightObject;
    [Tooltip("Drag the child 'Spot Light' component here")]
    public Light flashlight;

    public float batteryLevel = 100f;
    public float drainNormal = 2f;
    public float drainAttack = 10f;

    [Header("Attack Settings")]
    public float attackAngle = 15f;
    public float normalAngle = 50f;
    public float damagePerSecond = 50f;
    public float range = 10f;
    [Tooltip("How thick the burning beam is. Bigger = easier to hit a ghost.")]
    public float attackRadius = 0.35f;
    private Enemy lastHitEnemy;
    private bool isAttacking = false;

    [Header("Low Battery")]
    [Tooltip("Below this battery level the light starts to flicker and the reload hint shows up")]
    public float lowBatteryThreshold = 20f;
    [Tooltip("How strong the flicker is when the battery is low (0 = no flicker)")]
    [Range(0f, 1f)] public float lowBatteryFlicker = 0.7f;

    [Header("UI References")]
    public GameObject FlashlightIcon;
    public Image batteryBarImage;
    public TextMeshProUGUI promptText; // Strictly for the "Press R to reload" text!
    public AudioSource flashlightSwitchSound;
    public AudioSource flashlightRelaodSound;

    [Header("Tutorial UI")]
    [Tooltip("Drag your tutorial panel GameObject here")]
    public GameObject tutorialPanel;
    private bool waitingForTutorialClose = false;
    private int tutorialOpenedFrame;

    [Header("Script References")]
    public PlayerStats playerstats;
    public InventoryManager inventoryManager;

    private bool isLightOn = false;
    private float baseIntensity;
    [HideInInspector] public bool hasPickedUpFromTable = false;

    // Read by the ghosts so they can react to the light
    public bool IsLightOn => isLightOn && flashlight != null && flashlight.enabled;
    public bool IsAttacking => isAttacking;

    void Start()
    {
        if (promptText != null) promptText.gameObject.SetActive(false);

        // Hide UI elements on startup
        if (FlashlightIcon != null) FlashlightIcon.SetActive(false);
        if (batteryBarImage != null) batteryBarImage.gameObject.SetActive(false);

        if (flashlight != null) baseIntensity = flashlight.intensity;
    }

    void Update()
    {
        if (playerstats != null && playerstats.isDead)
        {
            StopAttack();
            StopCurrentEnemySound();
            return;
        }

        if (waitingForTutorialClose)
        {
            // Skip the frame the tutorial was opened in, otherwise the same 'E' press closes it instantly
            if (Time.frameCount > tutorialOpenedFrame && Input.anyKeyDown)
            {
                CloseTutorial();
            }
            return; // Stop running any other flashlight code while paused
        }

        // Safety block: Keep everything turned off until collected from the table
        if (!hasPickedUpFromTable)
        {
            if (FlashlightIcon != null) FlashlightIcon.SetActive(false);
            if (batteryBarImage != null) batteryBarImage.gameObject.SetActive(false);
            if (promptText != null) promptText.gameObject.SetActive(false);
            return;
        }

        // Show the battery bar frame because they now own the item
        if (batteryBarImage != null) batteryBarImage.gameObject.SetActive(true);

        // Don't react to keys while the game is paused (pause menu, ending choice...)
        if (Time.timeScale == 0f) return;

        // --- Toggle Light Switch (F) ---
        if (Input.GetKeyDown(KeyCode.F) && batteryLevel > 0)
        {
            isLightOn = !isLightOn;
            if (flashlight != null) flashlight.enabled = isLightOn;
            if (flashlightSwitchSound != null) flashlightSwitchSound.Play();
        }

        // Sync the UI Icon image directly with the light bulb power state
        if (FlashlightIcon != null) FlashlightIcon.SetActive(isLightOn);

        // --- Reload Logic (R) ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (batteryLevel < 100f && inventoryManager != null && inventoryManager.HasItem("Battery"))
            {
                batteryLevel = 100f;
                inventoryManager.RemoveItem("Battery");
                if (flashlight != null) flashlight.enabled = isLightOn;
                if (flashlightRelaodSound != null) flashlightRelaodSound.Play();
            }
        }

        UpdateReloadPrompt();

        // --- Battery Depletion Tracking ---
        if (batteryLevel <= 0)
        {
            batteryLevel = 0;
            isLightOn = false;
            if (flashlight != null) flashlight.enabled = false;
            StopAttack();
            StopCurrentEnemySound();

            if (batteryBarImage != null) batteryBarImage.fillAmount = 0;
            return;
        }

        // --- Active Attack Mechanics Loop ---
        if (isLightOn && flashlight != null && flashlight.enabled)
        {
            if (Input.GetMouseButton(1))
            {
                flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, attackAngle, Time.deltaTime * 10f);
                batteryLevel -= drainAttack * Time.deltaTime;
                PerformAttack();
            }
            else
            {
                flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, normalAngle, Time.deltaTime * 10f);
                batteryLevel -= drainNormal * Time.deltaTime;
                StopAttack();
                StopCurrentEnemySound();
            }
        }
        else
        {
            StopAttack();
            StopCurrentEnemySound();
        }

        UpdateLowBatteryFlicker();

        // Keep foreground fill meter synced
        if (batteryBarImage != null)
        {
            batteryBarImage.fillAmount = batteryLevel / 100f;
        }
    }

    void PerformAttack()
    {
        // A thin sphere instead of a single ray, so the player doesn't need pixel-perfect aim.
        // Trigger colliders (pickup zones, ending zones...) are ignored so they can't block the beam.
        RaycastHit hit;
        if (Physics.SphereCast(flashlight.transform.position, attackRadius, flashlight.transform.forward, out hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                if (lastHitEnemy != null && lastHitEnemy != enemy) lastHitEnemy.StopAttackSound();
                enemy.PlayAttackSound();
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                isAttacking = true;
                lastHitEnemy = enemy;
                return;
            }
        }
        isAttacking = false;
        StopCurrentEnemySound();
    }

    // True when the flashlight cone is shining on the target and nothing is in between
    public bool IsLighting(Transform target)
    {
        if (!IsLightOn || target == null) return false;

        Vector3 origin = flashlight.transform.position;
        Vector3 toTarget = target.position - origin;
        float distance = toTarget.magnitude;

        if (distance > flashlight.range) return false;
        if (Vector3.Angle(flashlight.transform.forward, toTarget) > flashlight.spotAngle * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(origin, toTarget / distance, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }
        return true;
    }

    void UpdateReloadPrompt()
    {
        if (promptText == null) return;

        bool hasBattery = inventoryManager != null && inventoryManager.HasItem("Battery");
        bool showPrompt = hasBattery && batteryLevel <= lowBatteryThreshold;

        if (showPrompt) promptText.text = "Press 'R' to reload battery";
        promptText.gameObject.SetActive(showPrompt);
    }

    void UpdateLowBatteryFlicker()
    {
        if (flashlight == null) return;

        if (batteryLevel > lowBatteryThreshold || lowBatteryFlicker <= 0f)
        {
            flashlight.intensity = baseIntensity;
            return;
        }

        // The emptier the battery, the worse the flicker. Perlin noise keeps it organic,
        // with a rare full drop-out for the scare.
        float emptiness = 1f - (batteryLevel / lowBatteryThreshold);
        float noise = Mathf.PerlinNoise(Time.time * 12f, 0f);
        float dip = noise * lowBatteryFlicker * emptiness;
        if (Random.value < 0.02f * emptiness) dip = 1f;

        flashlight.intensity = baseIntensity * (1f - dip);
    }

    void StopAttack() { isAttacking = false; }

    void StopCurrentEnemySound()
    {
        if (lastHitEnemy != null)
        {
            lastHitEnemy.StopAttackSound();
            lastHitEnemy = null;
        }
    }

    public void TriggerTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f; // Freeze the game so the player can read it safely
            waitingForTutorialClose = true;
            tutorialOpenedFrame = Time.frameCount;

            // Unlock mouse cursor so they feel in control (optional)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Unfreeze the game!
        waitingForTutorialClose = false;

        // Re-lock the cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
