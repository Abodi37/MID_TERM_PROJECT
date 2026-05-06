using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;

    private EnemyManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<EnemyManager>();
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
