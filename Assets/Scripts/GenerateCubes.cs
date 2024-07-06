using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for implement the game field with ground an path
public class GenerateCubes : MonoBehaviour {
    private Vector2 gridSize = new Vector2(50, 50); // grid size
    [SerializeField] private  GameObject cubePrefab; // prefab
    [SerializeField] private Material pathMaterial; // Material for cubes on the path
    [SerializeField] private  Material transparentMaterial; // Material for cubes not on the path
	[SerializeField] private  Material blockedMaterial;
	[SerializeField] private  Material buildMaterial;
    [SerializeField] private Transform waypointsParent; // Parent GameObject containing waypoints
    private Transform[] waypoints; // Waypoints for the path, included in scene
    private GameObject spawnObject;
    private GameObject baseObject;
    private float height = 0.1f;

    void Start() {
        spawnObject = GameObject.FindGameObjectWithTag("Spawn");
        baseObject = GameObject.FindGameObjectWithTag("Base");
        if (spawnObject != null && baseObject != null) {
            InitializeWaypoints();
            GenerateGrid();
            CalculatePathCubes();
        } else {
            Debug.LogError("Spawn or Base object not found.");
        }		
    }

    // get wayoints from parentand initialize spawn and base
    void InitializeWaypoints() {
        if (waypointsParent != null) {
            waypoints = new Transform[waypointsParent.childCount + 1]; // Increase the size by 2 to accommodate spawn and base objects
            waypoints[0] = spawnObject.transform; // set first waypoint as spawn

            // Fill in the waypoints from the parent
            for (int i = 0; i < waypointsParent.childCount; i++) waypoints[i + 1] = waypointsParent.GetChild(i);
            
            // move baseObject to first waypoint after spawn
			Vector3 newPosition = waypoints[2].position;
			newPosition.y = 1.05f;
			baseObject.transform.position = newPosition;
        } else {
            Debug.LogError("Waypoints parent not assigned.");
        }
    }

    // extend the path of all existing waypoints
    public void ExtendPath(int roundNumber) {
        // if waypoints length > 0 and base not already on position of last waypoint
        if (waypoints != null && waypoints.Length > 0 && roundNumber < waypoints.Length - 1) {
			// get next waypoint in waypoints and adjust base position
			int newWayPointBaseIndex = roundNumber + 1;
			Vector3 newPosition = waypoints[newWayPointBaseIndex].position;
            newPosition.y = 1.05f; // Ensure the base's y coordinate is always 1.05f
            baseObject.transform.position = newPosition;
        } else {
            //Debug.Log("Last Waypoint reached, base stays.");
            int lastWayPointBaseIndex = waypoints.Length - 1;
			Vector3 newPosition = waypoints[lastWayPointBaseIndex].position;
            newPosition.y = 1.05f;
            baseObject.transform.position = newPosition;
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
	
	// convert x and z coordinates to string with 0.5f offset
	private string CoordinatesToStringOffset(float x, float z) { return "Cube " + (x + 0.5f).ToString("00") + "-" + (z + 0.5f).ToString("00"); }
	
	// convert x and z coordinates to string
	private string CoordinatesToString(float x, float z) { return "Cube " + x.ToString("00") + "-" + z.ToString("00"); }

	// return waypoints Transform array
    public Transform[] GetWaypoints() { return waypoints; }

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
	
    // check if cube at position x,z is ground and if so changes to blocked material
	public bool TryCubeGroundAtPosition(float x, float z) {
		// get cube object by name
		string cubeName = CoordinatesToStringOffset(x, z);
		GameObject cube = GameObject.Find(cubeName);
		// change material if valid tag
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
	
	// reset cube at position x,z
	public void ResetCubeGroundAtPosition(float x, float z) {
		// get cube object by name
		string cubeName = CoordinatesToStringOffset(x, z);
		GameObject cube = GameObject.Find(cubeName);
		// change material
		if (cube != null) {
			Renderer renderer = cube.GetComponent<Renderer>();
			if (renderer != null) renderer.material = transparentMaterial;
			cube.tag = "Ground";
		}
	}
	
}