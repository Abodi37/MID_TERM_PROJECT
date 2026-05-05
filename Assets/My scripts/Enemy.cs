using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;

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
        // Add particle effects here later!
        Destroy(gameObject);
    }
}
