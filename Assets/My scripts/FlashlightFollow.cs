using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
   public Transform camTransform;
    public float speed = 10f;

    void LateUpdate()
    {
        // Option A: Perfect Match (Instant)
        // transform.rotation = camTransform.rotation;

         
        // Option B: Smooth Follow (More realistic)
        // Uncomment the line below and comment the one above for a smooth feel:
        transform.rotation = Quaternion.Slerp(transform.rotation, camTransform.rotation, speed * Time.deltaTime);
    }
}
