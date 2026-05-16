using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public string pickupMessage = "Press 'E' to pick up Battery";
    private bool isPlayerNearby = false;

    void Update()
    {
        // Â–« «·”ÿ— Ì÷„‰ √‰Â ·Ê ÷€ÿ E ÊÂÊ ﬁ—Ì» „‰ «·ﬂÊ·«Ìœ—  ‘ €·
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject();
        }
    }

    // «·œ«·… «·”Õ—Ì… «·„› ÊÕ… ··Õ“„… (Public)
    public void InteractWithObject()
    {
        FlashlightAttack player = FindFirstObjectByType<FlashlightAttack>();
        if (player != null)
        {
            player.inventoryManager.AddItem("Battery");
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