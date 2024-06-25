using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour {
    public LayerMask layerMask;
    public GameObject smallTowerPrefab;
    public GameObject rapidTowerPrefab;
    public GameObject laserTowerPrefab;
    public Camera cam;
    private GameObject dragTarget;
    private Vector3 spawnPosition;
    private bool spawnedTowerButNotPlaced = false;
    private float dragHeight = 1.1f;
    private float dragXOffset = 0.5f;
    private float dragZOffset = 0.5f;
    private Plane objectHeightPlane;
    public XRRayInteractor rightRayInteractor;
    public InputActionProperty triggerAction; // New input action property for the trigger
    private int dragTargetPrice;
    protected bool isDragging = false;
	public GameObject activeTowerListObject;
	private GameObject spawnedTower;

    void Start() {
        spawnPosition = new Vector3(0, dragHeight, 15);
        objectHeightPlane = new Plane(Vector3.up * dragHeight, Vector3.up);
    }

    void OnEnable() {
        triggerAction.action.started += OnTriggerPressed;
        triggerAction.action.canceled += OnTriggerReleased;
    }

    void OnDisable() {
        triggerAction.action.started -= OnTriggerPressed;
        triggerAction.action.canceled -= OnTriggerReleased;
    }

    void Update() {
        // if dragTarget valid move to mouse position
        if (isDragging && dragTarget != null) {
            DragTargetToMouse();
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context) {
        dragTarget = PerformRaycast();
        if (dragTarget != null) {
            isDragging = true;
            Debug.Log("Drag Started");
            Debug.Log("Dragging " + dragTarget);
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext context) {
        // reset drag
        isDragging = false;
        dragTarget = null;
    }

    private GameObject PerformRaycast() {
        if (rightRayInteractor != null) {
            // Perform raycast using XRRayInteractor
            Ray ray = new Ray(rightRayInteractor.transform.position, rightRayInteractor.transform.forward);
            RaycastHit hit;

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
		}
		return null;
    }

    // move target to mouse position
    private void DragTargetToMouse(){
        // get intersection with objectHeightPlane and move target to that point
        float distance;
        Ray ray = new Ray(rightRayInteractor.transform.position, rightRayInteractor.transform.forward);
        if (objectHeightPlane.Raycast(ray, out distance)) { // TODO: Fix posis
            Vector3 point = ray.GetPoint(distance);
			if (point.x < 1) point.x = 1;
			if (point.x > 49) point.x = 49;
			if (point.z < 1) point.z = 1;
			if (point.z > 49) point.z = 49;
            dragTarget.transform.position = new Vector3(Mathf.Round(point.x - dragXOffset), dragHeight, Mathf.Round(point.z - dragZOffset)); // Set y to 2
        }
    }
	
	private void SpawnTower(GameObject towerPrefab, Vector3 position) {
		// if game state is preparation (building) spawn a tower
		if (GameManager.instance.IsPreparationGameState()) {
			// avoid spawning multiple tower
			if (!spawnedTowerButNotPlaced) {
				spawnedTower = Instantiate(towerPrefab, position, towerPrefab.transform.rotation, activeTowerListObject.transform);
				spawnedTowerButNotPlaced =  true;
			}
		}
	}
	
	// create a SmallTower at spawnposition
	public void SpawnSmallTower() { 
		SpawnTower(smallTowerPrefab, spawnPosition); 
		dragTargetPrice = GameManager.instance.towerPrices["SMALL"];
	}
	// create a SmallTower at spawnposition
	public void SpawnRapidTower() { 
		SpawnTower(rapidTowerPrefab, spawnPosition); 
		dragTargetPrice = GameManager.instance.towerPrices["RAPID"];
	}
	// create a SmallTower at spawnposition
	public void SpawnLaserTower() { 
		SpawnTower(laserTowerPrefab, spawnPosition); 
		dragTargetPrice = GameManager.instance.towerPrices["LASER"];
	}
	
	public void TowerAcceptButtonPressed() {
		Debug.Log("Accept Button Pressed");
		spawnedTowerButNotPlaced = false;
		spawnedTower = null;
        GameManager.instance.RemoveCoins(dragTargetPrice);
	}
    public void TowerCancelButtonPressed() {
		Debug.Log("Cancel Button Pressed");
		spawnedTowerButNotPlaced = false;
		spawnedTower = null;
	}
	
	// show tower
	public void ToggleTowerActive(bool isVisible) {
		for (int i = 0; i < activeTowerListObject.transform.childCount; i++) {
			Transform child = activeTowerListObject.transform.GetChild(i);
			if (child.tag == "Tower"){
				TowerController towerController = child.gameObject.GetComponent<TowerController>();
				if (towerController) {
					if (towerController.hasBeenPlaced()) {
						child.gameObject.SetActive(isVisible);
					}
				}
			}
		}
	}
	
	public void DeleteSpawnedTower() {
		if (spawnedTowerButNotPlaced) {
			Destroy(spawnedTower);
			spawnedTowerButNotPlaced = false;
			spawnedTower = null;
		}
	}
	
	public void ShowTowerPressed() {
		ToggleTowerActive(true);
	}
	
	public void HideTowerPressed() {
		if (GameManager.instance.IsPreparationGameState()) {
			ToggleTowerActive(false);
		}
	}
	
}
