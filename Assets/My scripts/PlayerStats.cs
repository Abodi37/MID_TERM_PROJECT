using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

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

    [Header("Death")]
    public GameObject deathScreenUI;
    public bool isDead = false;

    [Header("Camera")]
    public CinemachineCamera virtualCamera;

    public EnemyManager enemyManager;

    void Start()
    {
        isDead = false;
        Time.timeScale = 1f;
    }

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

        if (isDead) return;
    }

    void Heal()
    {
        health = 100f;
        pillInventory--;
    }

    void Die()
    {
        if (health <= 0)
        {
            deathScreenUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isDead = true;
            virtualCamera.enabled = false;
        }
    }

     public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
