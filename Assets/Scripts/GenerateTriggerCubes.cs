using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderGenerator : MonoBehaviour {
    public GameObject triggerColliderPrefab; // Das Prefab für den Trigger-Collider-Block
    public Vector2 gridSize; // Die Größe des Gitters (z. B. 50x50)
    public Material cubeMaterial; // Das Material für die Würfel

    public float size = 0.1f;

    void Start() {
        GenerateTriggerColliders();
    }

    void GenerateTriggerColliders() {
        // Iteriere über jede Position auf der Ebene darüber
        for (float x = 0.5f; x < gridSize.x; x++) {
            // Erzeuge ein leeres GameObject als Elternobjekt für die aktuelle Reihe
            GameObject rowParent = new GameObject("Row " + (x - 0.5f));
            rowParent.transform.parent = transform; // Setze das Elternobjekt in die Hierarchie

            for (float z = 0.5f; z < gridSize.y; z++) {
                GameObject cube = Instantiate(triggerColliderPrefab, new Vector3(x, 1.5f, z), Quaternion.identity);
                cube.name = "Cube " + x.ToString() + " | " + z.ToString();
                cube.AddComponent<BoxCollider>().isTrigger = true;
                cube.transform.parent = rowParent.transform;
                
                Renderer cubeRenderer = cube.GetComponent<Renderer>(); // Füge das Material zum Cube hinzu
                if (cubeRenderer != null && cubeMaterial != null) {
                    cubeRenderer.material = cubeMaterial;
                }
            }
            DontDestroyOnLoad(rowParent);
        }
        Debug.Log("Fertig");
    }
}