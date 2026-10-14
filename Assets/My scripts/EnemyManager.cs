using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public List<GameObject> activeEnemies = new List<GameObject>();

    public float spawnInterval = 10f;
    private float spawnTimer;

    [Header("Spawn Rules")]
    [Tooltip("Never more ghosts than this at the same time")]
    public int maxActiveEnemies = 3;
    [Tooltip("Ghosts don't spawn closer than this to the player")]
    public float minSpawnDistance = 8f;
    [Tooltip("Ghosts start coming only after the player picked up the flashlight, so they can fight back")]
    public bool waitForFlashlight = true;

    [Header("Difficulty")]
    [Tooltip("Each spawn multiplies the interval by this, so ghosts come faster over time (1 = always the same)")]
    [Range(0.5f, 1f)] public float spawnIntervalMultiplier = 0.9f;
    [Tooltip("The interval never gets shorter than this")]
    public float minSpawnInterval = 15f;

    private float currentInterval;
    private Transform player;
    private Camera playerCamera;
    private FlashlightAttack flashlight;

    void Start()
    {
        currentInterval = spawnInterval;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
        // The flashlight object stays switched off until it's picked up, so search inactive objects too
        flashlight = FindFirstObjectByType<FlashlightAttack>(FindObjectsInactive.Include);
    }

    void Update()
    {
        // Destroyed ghosts leave empty slots behind
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;
        if (waitForFlashlight && flashlight != null && !flashlight.hasPickedUpFromTable) return;
        if (activeEnemies.Count >= maxActiveEnemies) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
            currentInterval = Mathf.Max(Mathf.Min(minSpawnInterval, spawnInterval), currentInterval * spawnIntervalMultiplier);
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = ChooseSpawnPoint(minSpawnDistance);
        if (spawnPoint == null) return;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    public void EnemyKilled(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    // Used by a ghost that just attacked: where should it come back from?
    public bool TryGetReappearPoint(float minDistance, out Vector3 point)
    {
        point = Vector3.zero;
        if (spawnPoints == null || spawnPoints.Length == 0) return false;

        Transform spawnPoint = ChooseSpawnPoint(minDistance);
        if (spawnPoint == null) return false;

        point = spawnPoint.position;
        return true;
    }

    // Prefer a point that is far enough AND out of the player's sight, so ghosts never pop in on screen
    Transform ChooseSpawnPoint(float minDistance)
    {
        if (player == null) return spawnPoints[Random.Range(0, spawnPoints.Length)];

        List<Transform> farAndHidden = new List<Transform>();
        List<Transform> far = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            if (Vector3.Distance(point.position, player.position) < minDistance) continue;

            far.Add(point);
            if (!IsVisibleToPlayer(point.position)) farAndHidden.Add(point);
        }

        if (farAndHidden.Count > 0) return farAndHidden[Random.Range(0, farAndHidden.Count)];
        if (far.Count > 0) return far[Random.Range(0, far.Count)];
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    bool IsVisibleToPlayer(Vector3 position)
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return false;

        Vector3 viewport = playerCamera.WorldToViewportPoint(position + Vector3.up);
        bool onScreen = viewport.z > 0f && viewport.x > 0f && viewport.x < 1f && viewport.y > 0f && viewport.y < 1f;
        if (!onScreen) return false;

        // On screen, but maybe behind a wall
        Vector3 from = playerCamera.transform.position;
        Vector3 to = position + Vector3.up;
        return !Physics.Linecast(from, to, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }
}
