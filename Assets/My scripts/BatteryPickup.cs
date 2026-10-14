using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public string pickupMessage = "Press 'E' to pick up Battery";
    private bool isPlayerNearby = false;

    void Update()
    {
        // هذا السطر يضمن أنه لو ضغط E وهو قريب من الكولايدر تشتغل
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }
    }

    // الدالة السحرية المفتوحة للحزمة (Public)
    public void InteractWithObject()
    {
        // Already picked up this frame by another script (PlayerInteraction / UHFPS)
        if (!gameObject.activeInHierarchy) return;

        // This battery is also a UHFPS item: UHFPS adds it, and InventoryBridge copies it
        // into our inventory. Adding it here as well would count it twice.
        if (GetComponentInParent<UHFPS.Runtime.InteractableItem>() != null) return;

        // Include inactive: the flashlight object is switched off until the player finds it,
        // and batteries should still be collectable before that
        FlashlightAttack player = FindFirstObjectByType<FlashlightAttack>(FindObjectsInactive.Include);
        if (player != null)
        {
            player.inventoryManager.AddItem("Battery");
            gameObject.SetActive(false); // so nothing else can pick it up again before Destroy happens
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}