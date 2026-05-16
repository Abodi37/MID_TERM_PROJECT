using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [Header("Setup Connections")]
    public GameObject handFlashlightObject; 
    [Tooltip("Drag the 'PlayerVirtualCamera' object here from your Hierarchy")]
    public Transform playerCamera; 

    [Header("Interaction Settings")]
    public float interactionRange = 5f; 
    public GameObject pressEPromptUI; 

    private bool isPlayerLooking = false;

    void Start()
    {
        if (pressEPromptUI != null)
        {
            pressEPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // Safety check if you forgot to drag the camera in
        if (playerCamera == null) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionRange))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)) 
            {
                if (!isPlayerLooking)
                {
                    isPlayerLooking = true;
                    if (pressEPromptUI != null) pressEPromptUI.SetActive(true); 
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUp();
                }
                return;
            }
        }

        if (isPlayerLooking)
        {
            isPlayerLooking = false;
            if (pressEPromptUI != null) pressEPromptUI.SetActive(false);
        }
    }

    void PickUp()
    {
        if (pressEPromptUI != null) pressEPromptUI.SetActive(false);

        if (handFlashlightObject != null)
        {
            handFlashlightObject.SetActive(true);
        }

        FlashlightAttack attackScript = FindFirstObjectByType<FlashlightAttack>();
        if (attackScript != null)
        {
            attackScript.hasPickedUpFromTable = true; 
            attackScript.batteryLevel = 100f;        
        }

        Debug.Log("Pickup successful!");
        Destroy(gameObject);
    }
}
