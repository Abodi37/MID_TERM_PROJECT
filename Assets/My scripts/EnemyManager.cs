using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public List<GameObject> activeEnemies = new List<GameObject>();
    
    public float spawnInterval = 10f;
    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    public void EnemyKilled(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }
}
