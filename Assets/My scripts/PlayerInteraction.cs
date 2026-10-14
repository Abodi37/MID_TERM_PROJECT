using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f;

    public GameObject interactUI;

    public FlashlightAttack flashlightScript;

    public PlayerStats playerStats;

    [Header("Pickups")]
    [Tooltip("Objects with one of these tags go into the inventory. The tag is also the item name.")]
    public string[] inventoryItemTags = { "Battery", "Pill", "Key", "Fuse" };
    [Tooltip("Optional: text that says what you're looking at, e.g. \"Press E to pick up Battery\"")]
    public TextMeshProUGUI promptText;
    [Tooltip("Optional: sound played when something is picked up")]
    public AudioClip pickupSound;

    const string FlashlightTag = "FlashlightItem";

    void Update()
    {
        // No interacting while paused or dead
        if (Time.timeScale == 0f || (playerStats != null && playerStats.isDead))
        {
            ShowPrompt(null);
            return;
        }

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            ShowPrompt(null);
            return;
        }

        GameObject target = hit.collider.gameObject;

        // Flashlight on the table
        if (target.CompareTag(FlashlightTag))
        {
            ShowPrompt("Press 'E' to pick up Flashlight");
            if (Input.GetKeyDown(KeyCode.E)) PickUpFlashlight(target);
        }
        // Battery, Pill, Key, Fuse...
        else if (IsInventoryItem(target))
        {
            ShowPrompt("Press 'E' to pick up " + target.tag);
            if (Input.GetKeyDown(KeyCode.E)) PickUpItem(target, target.tag);
        }
        else
        {
            ShowPrompt(null);
        }
    }

    bool IsInventoryItem(GameObject target)
    {
        foreach (string itemTag in inventoryItemTags)
        {
            if (target.CompareTag(itemTag)) return true;
        }
        return false;
    }

    void PickUpItem(GameObject item, string itemName)
    {
        if (flashlightScript == null || flashlightScript.inventoryManager == null) return;

        flashlightScript.inventoryManager.AddItem(itemName);
        RemovePickup(item);
    }

    void PickUpFlashlight(GameObject flashlightOnTable)
    {
        if (flashlightScript != null)
        {
            // 1. Unlock the safety variables inside the master script
            flashlightScript.hasPickedUpFromTable = true;
            flashlightScript.batteryLevel = 100f;

            // 2. Explicitly turn on the hand model object linked in your inspector
            if (flashlightScript.handsFlashlightObject != null)
            {
                flashlightScript.handsFlashlightObject.SetActive(true);
            }
            flashlightScript.TriggerTutorial();
        }

        // 3. Remove the object off the table
        RemovePickup(flashlightOnTable);
    }

    void RemovePickup(GameObject item)
    {
        if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, item.transform.position);

        // Deactivate right away: Destroy() only happens at the end of the frame, and until then
        // other pickup scripts on the same object (BatteryPickup, HealPickup) could add it a second time
        item.SetActive(false);
        Destroy(item);
        ShowPrompt(null);
    }

    void ShowPrompt(string message)
    {
        bool show = message != null;
        if (interactUI != null && interactUI.activeSelf != show) interactUI.SetActive(show);

        if (promptText != null)
        {
            promptText.gameObject.SetActive(show);
            if (show) promptText.text = message;
        }
    }
}
