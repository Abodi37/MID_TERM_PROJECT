using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float maxHealth = 100f;

    private EnemyManager manager;
    public Transform playerTransform;
    public GameObject deathEffect;
    [Header("Audio settings")]
    public AudioSource attackSound;

    void Start()
    {
        manager = FindFirstObjectByType<EnemyManager>();
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (attackSound == null)
        {
            attackSound = GetComponent<AudioSource>();
        }
    }
    // void Update()
    // {
        // if (playerTransform != null)
        // {
        // 1. Make the enemy look exactly at the player
        // transform.LookAt(playerTransform);

        // 2. OPTIONAL: If the enemy tilts weirdly (like leaning forward), 
        // use this instead to only rotate on the Y axis (staying upright):
        /*
        Vector3 targetPostition = new Vector3(playerTransform.position.x, 
                                            this.transform.position.y, 
                                            playerTransform.position.z);
        this.transform.LookAt(targetPostition);
        //} */
    // }

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
        float heightOffset = -1f;
        Vector3 spawnPos = transform.position + new Vector3(0, heightOffset, 0);

         if (deathEffect != null)
        {
            Instantiate(deathEffect, spawnPos, Quaternion.Euler(-90, 0, 0));
        }

        if (manager != null)
        {
            manager.EnemyKilled(gameObject);
        }

        Destroy(gameObject);
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && !attackSound.isPlaying)
        {
            attackSound.Play();
        }
    }

    public void StopAttackSound()
    {
        if (attackSound != null && attackSound.isPlaying)
        {
            attackSound.Stop();
        }
    }
}
