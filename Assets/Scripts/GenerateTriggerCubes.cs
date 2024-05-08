using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderGenerator : MonoBehaviour {
    public GameObject triggerColliderPrefab; // prefab
    public Vector2 gridSize; // grid size
    public Material cubeMaterial; // cube material

    public float height = 0.1f;

    void Start() {
        GenerateTriggerColliders();
    }

    void GenerateTriggerColliders() {
        for (float x = 0.5f; x < gridSize.x; x++) {
            // Create row parent
            GameObject rowParent = new GameObject("Row " + (x - 0.5f));
            rowParent.transform.parent = transform; 

            for (float z = 0.5f; z < gridSize.y; z++) {
                GameObject cube = Instantiate(triggerColliderPrefab, new Vector3(x, 1f + (height / 2), z), Quaternion.identity);
                cube.name = "Cube " + x.ToString() + " | " + z.ToString();
                cube.AddComponent<BoxCollider>().isTrigger = true;
                cube.transform.parent = rowParent.transform;
                
                Renderer cubeRenderer = cube.GetComponent<Renderer>(); // Add material
                if (cubeRenderer != null && cubeMaterial != null) {
                    cubeRenderer.material = cubeMaterial;
                }
            }
            // DontDestroyOnLoad(rowParent); TODO: fix me maybe
        }
        Debug.Log("Fertig");
    }
}