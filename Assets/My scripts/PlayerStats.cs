using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float sanity = 100f;
    public int pillInventory = 0;

    [Header("Rates")]
    public float baseSanityDrop = 2f;
    public float sanityRegenRate = 1f;
    public float damageRateAtZeroSanity = 5f;

    [Header("UI Images")]
    public Image healthBarImage;
    public Image sanityBarImage;

    [Header("Prompt UI")]
    public GameObject healPromptUI;


    public EnemyManager enemyManager;

    void Update()
    {
        int enemyCount = enemyManager.activeEnemies.Count;

        if (enemyCount > 0)
        {
            sanity -= baseSanityDrop * enemyCount * Time.deltaTime;
        }
        else
        {
            sanity += sanityRegenRate * Time.deltaTime;
        }
        sanity = Mathf.Clamp(sanity, 0, 100);

        if (sanity <= 0)
        {
            health -= damageRateAtZeroSanity * enemyCount * Time.deltaTime;
            if (health <= 0) Die();
        }

        if (Input.GetKeyDown(KeyCode.H) && pillInventory > 0)
        {
            Heal();
        }

        if (health <= 30f && pillInventory > 0)
        {
            healPromptUI.SetActive(true);
        }
        else
        {
            healPromptUI.SetActive(false);
        }

        healthBarImage.fillAmount = health / 100f;
        sanityBarImage.fillAmount = sanity / 100f;
    }

    void Heal()
    {
        health = 100f;
        pillInventory--;
    }

    void Die()
    {
        Debug.Log("Player has died.");
    }
}
