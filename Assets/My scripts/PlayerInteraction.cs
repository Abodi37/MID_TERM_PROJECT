using UnityEngine;
using TMPro; // Add this if using TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f;
    public GameObject interactUI; // Drag your "Press E" Text object here
    public FlashlightAttack flashlightScript; // Drag your Flashlight script here

    void Update()
    {
        RaycastHit hit;
        // Shoot a ray from the center of the screen
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            // If we hit a battery
            if (hit.collider.CompareTag("Battery"))
            {
                interactUI.SetActive(true); // Show "Press E"

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.batteryInventory++;
                    Destroy(hit.collider.gameObject);
                    interactUI.SetActive(false);
                }
            }
            else
            {
                interactUI.SetActive(false); // Hide if looking at something else
            }
        }
        else
        {
            interactUI.SetActive(false); // Hide if looking at nothing
        }
    }
}
