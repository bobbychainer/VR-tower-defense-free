using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour {
    public GameObject enemyPrefab; // Prefab für die zu spawnende Kugel
    public float spawnInterval = 5f; // Intervall zwischen den Spawns

    IEnumerator Start() {
        while (true) {
            SpawnEnemies();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemies() {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

}