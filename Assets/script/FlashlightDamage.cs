using UnityEngine;

public class FlashlightDamage : MonoBehaviour
{
    public float range = 20f;
    
    public Light flashlightLight;

    void Update()
    {
       
        if (flashlightLight != null && flashlightLight.enabled)
        {
            ShootLightRay();
        }
    }

    void ShootLightRay()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
               
                Destroy(hit.collider.gameObject);
                
            }
        }
    }
}