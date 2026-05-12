using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightAttack : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;
    public float batteryLevel = 100f;
    public float drainNormal = 2f;
    public float drainAttack = 10f;

    [Header("Attack Settings")]
    public float attackAngle = 15f;
    public float normalAngle = 50f;
    public float damagePerSecond = 50f;
    public float range = 10f;
    private bool isAttacking = false;
    public AudioSource attackSound;

    public int batteryInventory = 0;

    [Header("UI References")]
    public GameObject reloadPromptUI;

    [Header("UI Image")]
    public Image batteryBarImage;
    public GameObject FlashlightIcon;

    [Header("Script References")]
    public PlayerStats playerstats;
    public InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (batteryLevel > 0 || !flashlight.enabled)
            {
                ToggleFlashlight();
            }
        }

        if (flashlight.enabled && Input.GetMouseButton(1))
        {
            flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, attackAngle, Time.deltaTime * 10f);

            batteryLevel -= drainAttack * Time.deltaTime;

            PerformAttack();
        }
        else
        {
            flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, normalAngle, Time.deltaTime * 10f);
            if (flashlight.enabled) batteryLevel -= drainNormal * Time.deltaTime;
            StopAttack();
        }

        if (Input.GetKeyDown(KeyCode.R) && inventoryManager.HasItem("Battery"))
        {
            batteryLevel = 100f;

            inventoryManager.RemoveItem("Battery");
        }

        if (batteryLevel <= 0 && flashlight.enabled)
        {
            ToggleFlashlight();
        }

        if (batteryLevel <= 0 && inventoryManager.HasItem("Battery"))
        {
            reloadPromptUI.SetActive(true);
        }
        else
        {
            reloadPromptUI.SetActive(false);
        }

        batteryBarImage.fillAmount = batteryLevel / 100f;

        if (playerstats.isDead) return;
    }

    void PerformAttack()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                if (!attackSound.isPlaying)
                {
                    attackSound.Play();
                }
                else if (enemy == null)
                {
                    attackSound.Stop();
                }
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                isAttacking = true;
                return;
            }
        }
    }
    void StopAttack()
    {
         if (attackSound.isPlaying)
        {
            attackSound.Stop();
        }
        isAttacking = false;
    }
    void ToggleFlashlight()
    {
        flashlight.enabled = !flashlight.enabled;

        if (FlashlightIcon != null)
        {
            FlashlightIcon.SetActive(flashlight.enabled);
        }
    }
}