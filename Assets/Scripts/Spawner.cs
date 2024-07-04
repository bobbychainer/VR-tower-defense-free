using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spawns enemies
public class Spawner : MonoBehaviour {
    public List<GameObject> enemyPrefabs;
    private bool spawningEnabled = false;
    private float spawnInterval = 3f; 

    // Base probabilities
    private Dictionary<int, float[]> baseProbabilities = new Dictionary<int, float[]> {
        {1, new float[] {0.7f, 0.2f, 0.07f, 0.03f}}, // Runde 1: A hoch, B mittel, C niedrig, D kaum
        {2, new float[] {0.4f, 0.4f, 0.15f, 0.05f}}, // Runde 2: A hoch, B hoch, C mittel, D niedrig
        {3, new float[] {0.34f, 0.33f, 0.23f, 0.1f}}, // Runde 3: A hoch, B hoch, C hoch, D mittel
        {4, new float[] {0.25f, 0.25f, 0.25f, 0.25f}} // Runde 4: A hoch, B hoch, C hoch, D hoch
    };

    // Variable to store actual spawn probabilities
    private Dictionary<int, float[]> spawnProbabilities;

    void Start() {
        GenerateRandomProbabilities();
    }

    // Generate random probabilities based on base probabilities
    void GenerateRandomProbabilities() {
        spawnProbabilities = new Dictionary<int, float[]>();

        foreach (var entry in baseProbabilities) {
            float[] baseProbs = entry.Value;
            float[] randomizedProbs = new float[baseProbs.Length];

            float total = 0;
            for (int i = 0; i < baseProbs.Length; i++) {
                // Apply random variation to each base probability
                float variation = Random.Range(-0.05f, 0.05f); // Adjust this range for more or less variation
                randomizedProbs[i] = Mathf.Clamp(baseProbs[i] + variation, 0, 1); // Ensure probability stays between 0 and 1
                total += randomizedProbs[i];
            }

            // Normalize probabilities so they sum up to 1
            for (int i = 0; i < randomizedProbs.Length; i++) randomizedProbs[i] /= total;
            spawnProbabilities[entry.Key] = randomizedProbs;
        }
    }

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
        GameObject randomEnemyPrefab = GetRandomEnemyPrefab();
        Instantiate(randomEnemyPrefab, transform.position, Quaternion.identity);
    }

    // spawns enemy with a certain probability
    private GameObject GetRandomEnemyPrefab() {
        int currentRound = GameManager.instance.GetRound();
        float[] probabilities = spawnProbabilities.ContainsKey(currentRound) ? spawnProbabilities[currentRound] : spawnProbabilities[4];

        float total = 0;
        foreach (float prob in probabilities) total += prob;

        float randomPoint = Random.value * total;

        for (int i = 0; i < probabilities.Length; i++) {
            if (randomPoint < probabilities[i]) {
                return enemyPrefabs[i];
            } else {
                randomPoint -= probabilities[i];
            }
        }

        return enemyPrefabs[0]; // Fallback
    }

    // Increases the spawn rate every 4 rounds by decreasing the interval
    public void IncreaseSpawnRate(int round) {
        Debug.Log("Increasing spawn rate and enemy stats");
        if (round > 0 && round % 4 == 0) {
            spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.5f); // Decrease spawnInterval, but not below 0.5f
            GenerateRandomProbabilities(); // Regenerate random probabilities every 4 rounds
        }
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
