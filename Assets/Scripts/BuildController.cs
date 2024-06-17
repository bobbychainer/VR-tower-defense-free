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
    private GameManager gameManager;
    public Camera cam;
    private GameObject dragTarget;
    private Vector3 spawnPosition;
    private bool spawnedTowerButNotPlaced = false;
    private float dragHeight = 1f;
    private float dragXOffset = 0.5f;
    private float dragZOffset = 0.5f;
    private Plane objectHeightPlane;
    public XRRayInteractor leftRayInteractor;
    public LineRenderer lineRenderer;
    public InputActionProperty triggerAction; // New input action property for the trigger

    protected bool isDragging = false;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        spawnPosition = new Vector3(0, dragHeight, 5);
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

    private GameObject PerformRaycast() {
        if (leftRayInteractor != null) {
            // Perform raycast using XRRayInteractor
            Ray ray = new Ray(leftRayInteractor.transform.position, leftRayInteractor.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    // move target to mouse position
    private void DragTargetToMouse(){
        // get intersection with objectHeightPlane and move target to that point
        float distance;
        Ray ray = new Ray(leftRayInteractor.transform.position, leftRayInteractor.transform.forward);
        if (objectHeightPlane.Raycast(ray, out distance)) {
            Vector3 point = ray.GetPoint(distance);
            dragTarget.transform.position = new Vector3(Mathf.Round(point.x - dragXOffset), Mathf.Round(dragHeight), Mathf.Round(point.z - dragZOffset)); // Set y to 2
        }
    }

    private void SpawnTower(GameObject towerPrefab, Vector3 position) {
        // if game state is preparation (building) spawn a tower
        if (gameManager.IsPreparationGameState()) {
            // avoid spawning multiple towers
            if (!spawnedTowerButNotPlaced) {
                Instantiate(towerPrefab, position, towerPrefab.transform.rotation);
                spawnedTowerButNotPlaced = true;
            }
        }
    }

    // create a SmallTower at spawn position
    public void SpawnSmallTower() { SpawnTower(smallTowerPrefab, spawnPosition); }
    // create a RapidTower at spawn position
    public void SpawnRapidTower() { SpawnTower(rapidTowerPrefab, spawnPosition); }
    // create a LaserTower at spawn position
    public void SpawnLaserTower() { SpawnTower(laserTowerPrefab, spawnPosition); }
}
