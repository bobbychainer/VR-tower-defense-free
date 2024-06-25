using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for implement the game field with ground an path
public class GenerateCubes : MonoBehaviour {
    public GameObject cubePrefab; // prefab
    public Vector2 gridSize; // grid size
    public Material pathMaterial; // Material for cubes on the path
    public Material transparentMaterial; // Material for cubes not on the path
	public Material blockedMaterial;
	public Material buildMaterial;
    public Transform[] waypoints; // Waypoints for the path, included in scene
    private GameObject spawnObject;
    private GameObject baseObject;
    public float height = 0.1f;

    void Start() {
        spawnObject = GameObject.FindGameObjectWithTag("Spawn");
        baseObject = GameObject.FindGameObjectWithTag("Base");
        if (spawnObject != null && baseObject != null) {
            GenerateGrid();
            AddSpawnAndBaseAsWaypoints();
            CalculatePathCubes();
        } else {
            Debug.LogError("Spawn or Base object not found.");
        }
    }

    // generates the grid
    void GenerateGrid() {
        for (float x = 0.5f; x < gridSize.x; x++) {
            // Create row parent
            GameObject rowParent = new GameObject("Row " + (x + 0.5f));
            rowParent.transform.parent = transform; 

            for (float z = 0.5f; z < gridSize.y; z++) {
                GameObject cube = Instantiate(cubePrefab, new Vector3(x, 1f + (height / 2), z), Quaternion.identity);
                cube.name = CoordinatesToStringOffset(x,z);
                cube.AddComponent<BoxCollider>().isTrigger = true;
                
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null) renderer.material = transparentMaterial;
                cube.tag = "Ground";
                cube.transform.parent = rowParent.transform;
            }
        }
        Debug.Log("Cubes successfully generated");
    }
	
	private string CoordinatesToStringOffset(float x, float z) {
		return "Cube " + (x + 0.5f).ToString("00") + "-" + (z + 0.5f).ToString("00");
	}
	
	private string CoordinatesToString(float x, float z) {
		return "Cube " + x.ToString("00") + "-" + z.ToString("00");
	}

    // adds spawn and base to waypoints
    void AddSpawnAndBaseAsWaypoints() {
        List<Transform> updatedWaypoints = new List<Transform>(waypoints);
        updatedWaypoints.Insert(0, spawnObject.transform);
        updatedWaypoints.Add(baseObject.transform);
        waypoints = updatedWaypoints.ToArray();
    }

    // calculates the whole path between spawn and base
    public void CalculatePathCubes() {
        for (int i = 0; i < waypoints.Length - 1; i++) {
            Transform currentWaypoint = waypoints[i]; // get current and nect waypoint
            Transform nextWaypoint = waypoints[i + 1];

            Vector3 roundedPosition = RoundPosition(currentWaypoint.position);
            ColorCubeAtPosition(roundedPosition); // color the current cube

            Vector3 direction = nextWaypoint.position - currentWaypoint.position; //calculate the direction to next waypoint
            float step = 1.0f / Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.z)); // step size

            for (float t = 0; t <= 1; t += step) { // color every cube on the line between the waypoints
                Vector3 positionOnLine = Vector3.Lerp(currentWaypoint.position, nextWaypoint.position, t);
                Vector3 roundedPositionOnLine = RoundPosition(positionOnLine);
                ColorCubeAtPosition(roundedPositionOnLine);
            }
        }
    }

     private Vector3 RoundPosition(Vector3 position) {
        return new Vector3(Mathf.Round(position.x + 0.5f), position.y, Mathf.Round(position.z + 0.5f));
    }

    // colors the current cube at the given position in the path
    private void ColorCubeAtPosition(Vector3 position) {
        // Check if the position is within the grid bounds
        if (position.x >= 0 && position.x < gridSize.x && position.z >= 0 && position.z < gridSize.y) {
			string cubeName = CoordinatesToString(position.x, position.z);
            GameObject cube = GameObject.Find(cubeName);
            if (cube != null) {
                cube.tag = "Path";
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null) renderer.material = pathMaterial;
            }
        }
    }
	
    // Only can place on ground
	public bool TryBlockGroundAtPosition(float x, float z) {
		
		string cubeName = CoordinatesToStringOffset(x, z);
		GameObject cube = GameObject.Find(cubeName);
		
		if (cube != null) {
			if (cube.tag == "Ground") {
				Renderer renderer = cube.GetComponent<Renderer>();
				if (renderer != null) renderer.material = blockedMaterial;
				cube.tag = "Blocked";
				return true;
			}
		}
		return false;
	}
	
}