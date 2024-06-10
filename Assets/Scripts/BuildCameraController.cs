using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCameraController : MonoBehaviour {
    public LayerMask layerMask;
	public GameObject smallTowerPrefab;
	public GameObject rapidTowerPrefab;
	public GameObject laserTowerPrefab;
	private GameManager gameManager;
	private Camera cam;
	private GameObject dragTarget;
	private Vector3 spawnPosition;
	private bool spawnedTowerButNotPlaced = false;
	
	private float dragHeight = 1f;
	private float dragXOffset = 0.5f;
	private float dragZOffset = 0.5f;
	private Plane objectHeightPlane;
	
	protected bool isDragging = false;

	void Start() {
		cam = GetComponent<Camera>();
		gameManager = FindObjectOfType<GameManager>();
		spawnPosition = new Vector3(0,dragHeight,25);
		objectHeightPlane = new Plane(Vector3.up * dragHeight, Vector3.up);
	}

    void Update(){
		// only run while in build state
		if (gameManager.IsPreparationGameState()) {
			// mouse pressed. Select dragTarget if layerMask is valid
			if (Input.GetMouseButtonDown(0)) {
				dragTarget = PointerOnObject();
				if (dragTarget != null) {
					isDragging = true;
					Debug.Log("Drag Started");
					Debug.Log("Dragging " + dragTarget);
				}
			}
			
			// mouse realeased. Place tower and reset dragTarget
			if (Input.GetMouseButtonUp(0)) {
				// place dragTarget
				if (dragTarget != null) {
					TowerController towerController = dragTarget.GetComponent<TowerController>();
					if (towerController != null) {
						towerController.PlaceTower();
						spawnedTowerButNotPlaced = false;
					}
					Debug.Log("Drag Released");
				}
				// reset drag
				isDragging = false;
				dragTarget = null;
			}
			
			// if dragTarget valid move to mouse position
			if (isDragging && dragTarget != null) {
				DragTargetToMouse();
			}
		}
    }
	
	// move target to mouse positon
	private void DragTargetToMouse(){
		// ray from camera through mouse position on screen
		Vector3 adjustedMousePosition = AdjustMousePosition(Input.mousePosition);
        Ray ray = cam.ScreenPointToRay(adjustedMousePosition);
		// get intersection with objectHeightPlane and move target to that point
		float distance;
		if (objectHeightPlane.Raycast(ray, out distance)) {
			Vector3 point = ray.GetPoint(distance);
			dragTarget.transform.position = new Vector3(Mathf.Round(point.x - dragXOffset), Mathf.Round(dragHeight), Mathf.Round(point.z - dragZOffset)); // Set y to 2
		}
    }
	
	private GameObject PointerOnObject() {
		// ray from camera through mouse position on screen
		RaycastHit hit;
		Vector3 adjustedMousePosition = AdjustMousePosition(Input.mousePosition);
		Ray ray = cam.ScreenPointToRay(adjustedMousePosition);
		// if ray hits a tower object and tower not placed return tower object
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
			TowerController towerController = hit.collider.gameObject.GetComponent<TowerController>();
			if (towerController) {
				if (!towerController.hasBeenPlaced()) {
					return hit.collider.gameObject;
				} else {
					Debug.Log("Tower already placed.");
				}
			}
		}
		return null;
	}

	// adjust mouseposition to avoid wrong target position while dragging and for drag selection
    private Vector3 AdjustMousePosition(Vector3 originalMousePosition) {
        // adjust the mouse position to account for any offsets due to the XR setup
        Vector3 screenPoint = originalMousePosition;
        // convert screen point to viewport point, taking into account the XR origin's offset and scaling
        Vector3 viewportPoint = cam.ScreenToViewportPoint(screenPoint);
        // adjust viewport point back to screen space, correcting for the XR setup's viewport
        Vector3 adjustedScreenPoint = cam.ViewportToScreenPoint(viewportPoint);
        return adjustedScreenPoint;
    }
	
	private void SpawnTower(GameObject towerPrefab, Vector3 position) {
		// if game state is preparation (building) spawn a tower
		if (gameManager.IsPreparationGameState()) {
			// avoid spawning multiple tower
			if (!spawnedTowerButNotPlaced) {
				Instantiate(towerPrefab, position, towerPrefab.transform.rotation);
				spawnedTowerButNotPlaced =  true;
			}
		}
	}
	
	// create a SmallTower at spawnposition
	public void SpawnSmallTower() { SpawnTower(smallTowerPrefab, spawnPosition); }
	// create a SmallTower at spawnposition
	public void SpawnRapidTower() { SpawnTower(rapidTowerPrefab, spawnPosition); }
	// create a SmallTower at spawnposition
	public void SpawnLaserTower() { SpawnTower(laserTowerPrefab, spawnPosition); }
}
