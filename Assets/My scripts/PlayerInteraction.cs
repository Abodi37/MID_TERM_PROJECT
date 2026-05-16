using UnityEngine;
using TMPro;

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
            // Battery
            if (hit.collider.CompareTag("Battery"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.inventoryManager.AddItem("Battery");

                    Destroy(hit.collider.gameObject);

                    interactUI.SetActive(false);
                }
            }

            // Pill
            else if (hit.collider.CompareTag("Pill"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.inventoryManager.AddItem("Pill");

                    Destroy(hit.collider.gameObject);

                    interactUI.SetActive(false);
                }
            }

            // Key
            else if (hit.collider.CompareTag("Key"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.inventoryManager.AddItem("Key");

                    Destroy(hit.collider.gameObject);

                    interactUI.SetActive(false);
                }
            }

            // Fuse
            else if (hit.collider.CompareTag("Fuse"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flashlightScript.inventoryManager.AddItem("Fuse");

                    Destroy(hit.collider.gameObject);

                    interactUI.SetActive(false);
                }
            }
            else if (hit.collider.CompareTag("FlashlightItem"))
            {
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
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

