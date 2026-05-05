using Unity.VisualScripting;
using UnityEngine;

public class FlashlightAttack : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;
    public float batteryLevel = 100f;
    public float drainNormal = 2f;
    public float drainAttack = 10f;
    
    [Header("Attack Settings")]
    public float attackAngle = 15f;    // Narrow beam
    public float normalAngle = 50f;    // Wide beam
    public float damagePerSecond = 50f;
    public float range = 10f;

    public int batteryInventory = 0; // Simple counter for collected batteries

    [Header("UI References")]
    public GameObject reloadPromptUI;


    void Update()
    {
        // 1. Toggle On/Off
        if (Input.GetKeyDown(KeyCode.F) && batteryLevel > 0)
        {
            flashlight.enabled = !flashlight.enabled;
        }

        // 2. Attack (Hold Right Mouse Button)
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
        }

        // 3. Recharge with R
        if (Input.GetKeyDown(KeyCode.R) && batteryInventory > 0)
        {
            batteryLevel = 100f;
            batteryInventory--;
        }

        // 4. Emergency Shutoff
        if (batteryLevel <= 0)
        {
            batteryLevel = 0;
            flashlight.enabled = false;
        }

            if (batteryLevel <= 0 && batteryInventory > 0)
        {
            reloadPromptUI.SetActive(true);
        }
        else
        {
            reloadPromptUI.SetActive(false);
        }
    }

    void PerformAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            // Check if what we hit has an Enemy script
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
