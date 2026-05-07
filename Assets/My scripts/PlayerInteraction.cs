using UnityEngine;
using TMPro; // Add this if using TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f;
    public GameObject interactUI;
    public FlashlightAttack flashlightScript;
    public PlayerStats playerStats;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Battery"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.batteryInventory++;
                    Destroy(hit.collider.gameObject);
                    interactUI.SetActive(false);
                }
            }
            else if (hit.collider.CompareTag("Pill"))
            {
                interactUI.SetActive(true);

                if(Input.GetKeyDown(KeyCode.E))
                {
                    playerStats.pillInventory++;
                    Destroy(hit.collider.gameObject);
                    interactUI.SetActive(false);
                }
            }
            else
            {
                interactUI.SetActive(false);
            }
        }
        else
        {
            interactUI.SetActive(false);
        }
    }
}
