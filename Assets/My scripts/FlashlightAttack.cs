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

    public int batteryInventory = 0;

    [Header("UI References")]
    public GameObject reloadPromptUI;

    [Header("UI Image")]
    public Image batteryBarImage;

    [Header("Script References")]
    public PlayerStats playerstats;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && batteryLevel > 0)
        {
            flashlight.enabled = !flashlight.enabled;
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
        }

        if (Input.GetKeyDown(KeyCode.R) && batteryInventory > 0)
        {
            batteryLevel = 100f;
            batteryInventory--;
        }

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
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
