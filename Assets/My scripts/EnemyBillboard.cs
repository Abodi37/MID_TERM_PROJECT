using UnityEngine;

public class EnemyBillboard : MonoBehaviour
{
    public Transform playerCamera;
    
    [Header("Fine-Tune Rotation")]
    [Tooltip("Adjust these if your picture looks sideways or backward")]
    public Vector3 rotationOffset = new Vector3(0, 180, 0); 

    void Start()
    {
        // Automatically find the main camera if you didn't assign one
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (playerCamera == null) return;

        // 1. Calculate direction to the camera but lock the Y axis 
        // This keeps the quad standing perfectly upright on the floor
        Vector3 targetDirection = playerCamera.position - transform.position;
        targetDirection.y = 0; 

        if (targetDirection != Vector3.zero)
        {
            // 2. Create the base rotation facing the player
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // 3. Apply your custom rotation offset so it displays correctly
            transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
        }
    }
}
