using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;

    private EnemyManager manager;
    public Transform playerTransform;

    void Start()
    {
        manager = FindFirstObjectByType<EnemyManager>();
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    void Update()
    {
        if (playerTransform != null)
        {
        // 1. Make the enemy look exactly at the player
        transform.LookAt(playerTransform);

        // 2. OPTIONAL: If the enemy tilts weirdly (like leaning forward), 
        // use this instead to only rotate on the Y axis (staying upright):
        /*
        Vector3 targetPostition = new Vector3(playerTransform.position.x, 
                                            this.transform.position.y, 
                                            playerTransform.position.z);
        this.transform.LookAt(targetPostition);
        */
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Evaporate();
        }
    }

    void Evaporate()
    {
        if (manager != null)
        {
            manager.EnemyKilled(gameObject);
        }

        Destroy(gameObject);
    }
}
