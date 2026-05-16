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
    private Enemy lastHitEnemy;
    private bool isAttacking = false;

    [Header("UI References")]
    public GameObject FlashlightIcon;
    public Image batteryBarImage;
    public TextMeshProUGUI promptText; // Strictly for the "Press R to reload" text!
    public AudioSource flashlightSwitchSound;
    public AudioSource flashlightRelaodSound;

    [Header("Script References")]
    public PlayerStats playerstats;
    public InventoryManager inventoryManager;

    private bool isLightOn = false;
    [HideInInspector] public bool hasPickedUpFromTable = false;

    void Start()
    {
        if (promptText != null) promptText.gameObject.SetActive(false);

        // Hide UI elements on startup
        if (FlashlightIcon != null) FlashlightIcon.SetActive(false);
        if (batteryBarImage != null) batteryBarImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerstats.isDead) return;

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
            if (batteryLevel < 100f && inventoryManager.HasItem("Battery"))
            {
                batteryLevel = 100f;
                inventoryManager.RemoveItem("Battery");
                if (flashlight != null) flashlight.enabled = isLightOn;
                if (promptText != null) promptText.gameObject.SetActive(false);
                if (flashlightRelaodSound != null) flashlightRelaodSound.Play();
            }
        }

        // --- Battery Depletion Tracking ---
        if (batteryLevel <= 0)
        {
            batteryLevel = 0;
            isLightOn = false;
            if (flashlight != null) flashlight.enabled = false;
            StopAttack();
            StopCurrentEnemySound();

            if (batteryBarImage != null) batteryBarImage.fillAmount = 0;

            if (promptText != null && inventoryManager.HasItem("Battery"))
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Press 'R' to reload battery";
            }
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

        // Keep foreground fill meter synced
        if (batteryBarImage != null)
        {
            batteryBarImage.fillAmount = batteryLevel / 100f;
        }
    }

    void PerformAttack()
    {
        RaycastHit hit;
        // Shoot directly forward from the camera/flashlight trajectory position
        if (Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out hit, range))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
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
        StopCurrentEnemySound();
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
}



