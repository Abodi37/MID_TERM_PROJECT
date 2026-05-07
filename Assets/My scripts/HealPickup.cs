using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public string pickupMessage = "Press 'E' to pick up Pill";
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PlayerStats player = FindFirstObjectByType<PlayerStats>();
            if (player != null)
            {
                player.pillInventory++;
                Destroy(gameObject);
            }
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
