using UnityEngine;

public class EndingTriggerZone : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Make sure it's the player walking into the zone, and only trigger ONCE
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            EndingManager endingManager = FindFirstObjectByType<EndingManager>();
            if (endingManager != null)
            {
                endingManager.OpenEndingMenu();
            }
        }
    }
}
