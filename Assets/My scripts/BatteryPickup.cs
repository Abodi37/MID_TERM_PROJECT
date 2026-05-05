using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public string pickupMessage = "Press 'E' to pick up Battery";
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            FlashlightAttack player = FindFirstObjectByType<FlashlightAttack>();
            if (player != null)
            {
                player.batteryInventory++;
                Destroy(gameObject);
            }
        }
    }

    // Detect when player is looking at/near the battery
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
