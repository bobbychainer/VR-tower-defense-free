using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spawns enemies
public class Spawner : MonoBehaviour {
    public List<GameObject> enemyPrefabs;
    private bool spawningEnabled = false;
    public float spawnInterval = 3f; 

    IEnumerator SpawnEnemies() {
        while (spawningEnabled) {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    // Destroy all enemies when timer is over
    private void DestroyAllEnemies() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) Destroy(enemy);
    }

    void SpawnEnemy() {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject randomEnemyPrefab = enemyPrefabs[randomIndex];
        Instantiate(randomEnemyPrefab, transform.position, Quaternion.identity);
    }
    public void StartEnemySpawn() {
        spawningEnabled = true;
        StartCoroutine(SpawnEnemies());
    }

    public void StopEnemySpawn() {
        spawningEnabled = false;
        DestroyAllEnemies();
    }
}