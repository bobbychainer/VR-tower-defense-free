using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour {
    public LayerMask towerLayerMask;
	public LayerMask optionLayerMask;
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
	
	private GameObject selectedTarget;
	
	// Set the spawn position of buyed tower and the drag height for placing
    void Start() {
        spawnPosition = new Vector3(0, dragHeight, 15);
        objectHeightPlane = new Plane(Vector3.up * dragHeight, Vector3.up);
    }

	// VR Controller stuff
    void OnEnable() {
        triggerAction.action.started += OnTriggerPressed;
        triggerAction.action.canceled += OnTriggerReleased;
    }
	// VR Controller stuff
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

	// if VR Controller trigger is pressed
	// two different states: allow on click only drag-and-drop or selection
    private void OnTriggerPressed(InputAction.CallbackContext context) {
		if (spawnedTowerButNotPlaced) {
			// drag state
			// find drag target if ray points towards Tower object
			dragTarget = PerformRaycastOnTower();
			if (dragTarget != null) {
				TowerController towerController = dragTarget.gameObject.GetComponent<TowerController>();
				if (towerController) {
					// only not placed towers can be moved
					if (!towerController.hasBeenPlaced()) {
						isDragging = true;
					}
				}
			}
		} else {
			// select state
			// find select target if ray points towards Tower object
			GameObject selectedTargetNew = PerformRaycastOnTower();
			// if Tower already selected or null again do nothing
			if (!GameObject.ReferenceEquals(selectedTargetNew, selectedTarget)) {
				// on option buttons presses dont reset selection even if ray hits no tower
				GameObject optionMenuItem = PerformRaycastOnOption();
				// if no option button pressed
				if (optionMenuItem == null) {
					// toggle the menu of the selected tower off
					ResetInformationMenuOfSelectedTarget();
					selectedTarget = selectedTargetNew;
					if (selectedTarget != null) {
						TowerController towerController = selectedTarget.gameObject.GetComponent<TowerController>();
						if (towerController) {
							// show information panel of selected target
							towerController.ShowInformationMenu();
						}
					}
				}
			}
		}
    }

	// deactivate the dragging
    private void OnTriggerReleased(InputAction.CallbackContext context) {
        // reset drag
        isDragging = false;
        dragTarget = null;
    }
	
	// find and return the Tower object hit by rightRay of VR Controller
    private GameObject PerformRaycastOnTower() {
        if (rightRayInteractor != null) {
            // Perform raycast using XRRayInteractor
            Ray ray = new Ray(rightRayInteractor.transform.position, rightRayInteractor.transform.forward);
            RaycastHit hit;
			// check if ray hits Tower object
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerLayerMask)) {
				return hit.collider.gameObject;
			}
		}
		return null;
    }
	// find and return the Option object hit by rightRay of VR Controller
	private GameObject PerformRaycastOnOption() {
		if (rightRayInteractor != null) {
            // Perform raycast using XRRayInteractor
            Ray ray = new Ray(rightRayInteractor.transform.position, rightRayInteractor.transform.forward);
            RaycastHit hit;
			// check if ray hits Option object
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, optionLayerMask)) {
				return hit.collider.gameObject;
			}
		}
		return null;
	}

    // move target to mouse position
    private void DragTargetToMouse(){
        // get intersection with objectHeightPlane and move target to that point
        float distance;
        Ray ray = new Ray(rightRayInteractor.transform.position, rightRayInteractor.transform.forward);
        if (objectHeightPlane.Raycast(ray, out distance)) {
			// restrict mouse drag from 0 to 50 on x and z axis
            Vector3 point = ray.GetPoint(distance);
			if (point.x < 1) point.x = 1;
			if (point.x > 49) point.x = 49;
			if (point.z < 1) point.z = 1;
			if (point.z > 49) point.z = 49;
			// round x and z coordinates
            dragTarget.transform.position = new Vector3(Mathf.Round(point.x - dragXOffset), dragHeight, Mathf.Round(point.z - dragZOffset)); // Set y to 2
        }
    }
	
	// hide all menu items of the selected tower
	private void ResetInformationMenuOfSelectedTarget() {
		if (selectedTarget != null) {
			TowerController towerController = selectedTarget.gameObject.GetComponent<TowerController>();
			if (towerController) {
				// check for a placed tower
				if (towerController.hasBeenPlaced()) {
					// toggle information panel off
					towerController.CloseInformationMenu();
				}
			}
		}
	}
	
	// instantiate a Tower Object on spawn position
	private void SpawnTower(GameObject towerPrefab, Vector3 position) {
		// if game state is preparation (building) spawn a tower
		if (GameManager.instance.IsPreparationGameState()) {
			// avoid spawning multiple tower
			if (!spawnedTowerButNotPlaced) {
				// hide selected target information panel
				ResetInformationMenuOfSelectedTarget();
				spawnedTower = Instantiate(towerPrefab, position, towerPrefab.transform.rotation, activeTowerListObject.transform);
				spawnedTowerButNotPlaced =  true;
			}
		}
	}
	
	// create a SmallTower at spawnposition
	public void SpawnSmallTower() { 
		SpawnTower(smallTowerPrefab, spawnPosition);
		// get costs of tower
		dragTargetPrice = GameManager.instance.GetTowerCosts("SMALL",1);
	}
	// create a SmallTower at spawnposition
	public void SpawnRapidTower() { 
		SpawnTower(rapidTowerPrefab, spawnPosition);
		// get costs of tower
		dragTargetPrice = GameManager.instance.GetTowerCosts("RAPID",1);
	}
	// create a SmallTower at spawnposition
	public void SpawnLaserTower() { 
		SpawnTower(laserTowerPrefab, spawnPosition);
		// get costs of tower
		dragTargetPrice = GameManager.instance.GetTowerCosts("LASER",1);
	}
	
	// tower placed valid ground
	public void TowerAcceptButtonPressed() {
		// remove coins for paying
        GameManager.instance.RemoveCoins(dragTargetPrice);
		// add costs to tower value
		TowerController towerController = spawnedTower.gameObject.GetComponent<TowerController>();
		if (towerController) towerController.IncreaseTowerPrice(dragTargetPrice);
		// reset drag
		spawnedTowerButNotPlaced = false;
		spawnedTower = null;
	}
	
	// tower placement canceled
    public void TowerCancelButtonPressed() {
		spawnedTowerButNotPlaced = false;
		spawnedTower = null;
	}
	
	// tower upgrade button got pressed
	public bool TowerUpgradeButtonPressed(string towerName, int level) {
		// check if player got enough coins to pay
		float currCoins = GameManager.instance.GetPlayerCoins();
		int towerCosts = GameManager.instance.GetTowerCosts(towerName, level);
		if (currCoins >= towerCosts) {
			// pay tower costs
			GameManager.instance.RemoveCoins(towerCosts);
			// add costs to tower value
			TowerController towerController = selectedTarget.gameObject.GetComponent<TowerController>();
			if (towerController) towerController.IncreaseTowerPrice(towerCosts);
			return true;
		}
		return false;
	}
	
	public void TowerDeleteButtonPressed(int refund) {GameManager.instance.AddCoins(refund);}
	
	// return the damage, attackCooldown, attackRadius, price for a Tower name and a level
	public (int damage, float attackCooldown, float attackRadius, int price) GetAllUpgrades(string name, int level) {
        int damage = GameManager.instance.GetDamageUpgrade(name, level);
        float attackCooldown = GameManager.instance.GetAttackCooldownUpgrade(name, level);
        float attackRadius = GameManager.instance.GetAttackRadiusUpgrade(name, level);
		int price = GameManager.instance.GetTowerCosts(name, level);
        return (damage, attackCooldown, attackRadius, price);
    }
	
	
	// show tower
	public void ToggleTowerActive(bool isVisible) {
		// iterate over child objects of tag "Tower" and toggle active
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
	
	// remove not placed tower
	// if start pressed but tower placement not accepted
	public void DeleteSpawnedTower() {
		if (spawnedTowerButNotPlaced) {
			Destroy(spawnedTower);
			spawnedTowerButNotPlaced = false;
			spawnedTower = null;
		}
	}
	
	// show all tower
	public void ShowTowerPressed() {
		ToggleTowerActive(true);
	}
	
	// hide all tower
	public void HideTowerPressed() {
		if (GameManager.instance.IsPreparationGameState()) {
			ToggleTowerActive(false);
		}
	}
	
}
