using UnityEngine;

namespace FlashlightSystem
{
    // Ensures this script is only attached to objects with a Camera component
    [RequireComponent(typeof(Camera))]
    public class FlashlightInteractor : MonoBehaviour
    {
        [Header("Raycast Features")]
        [SerializeField] private float rayDistance = 5; // Max distance for detecting flashlight items

        [Header("Pickup Input")]
        [SerializeField] private KeyCode pickupKey = KeyCode.Mouse0; // Key used to pick up flashlight items

        private FlashlightItem interactiveItem; // Currently targeted flashlight item
        private Camera _camera; // Cached reference to the attached camera

        void Start()
        {
            // Try to get the camera component on this GameObject
            if (!TryGetComponent<Camera>(out _camera))
            {
                Debug.LogError("Camera component not found on the GameObject.");
            }
        }

        void Update()
        {
            // Cast a ray from the center of the screen forward
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayDistance))
            {
                // Check if the object hit has a FlashlightItem component
                var flashlightItem = hit.collider.GetComponent<FlashlightItem>();

                if (flashlightItem != null)
                {
                    interactiveItem = flashlightItem; // Store reference to item
                    HighlightCrosshair(true); // Enable crosshair highlight
                }
                else
                {
                    ClearSelected(); // Clear if hit object isn't interactable
                }
            }
            else
            {
                ClearSelected(); // No object hit — clear selection
            }

            // Handle input to pick up the flashlight item
            if (interactiveItem != null)
            {
                if (Input.GetKeyDown(pickupKey))
                {
                    interactiveItem.ObjectInteract(); // Call item's interaction logic
                }
            }
        }

        // Clears the currently selected item and disables the crosshair highlight
        private void ClearSelected()
        {
            if (interactiveItem != null)
            {
                HighlightCrosshair(false);
                interactiveItem = null;
            }
        }

        // Toggles crosshair highlight via the UI Manager
        void HighlightCrosshair(bool on)
        {
            FLUIManager.instance.HighlightCrosshair(on);
        }
    }
}
