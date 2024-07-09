using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCloudController : MonoBehaviour {
	
	// The target camera that the cloud will follow
    private Transform mainCamera;

    // Speed at which the cloud will follow the camera
    private float followSpeed = 0.5f;
	private float minY = 1f;
	private Vector3 spawnPosition = new Vector3(8.5f, 1.1f, 15f);

    // Offset position relative to the camera
    private Vector3 offset = new Vector3(5.0f, -4.0f, 11.0f);
	private Vector3 waitOffset = new Vector3(7.0f, 3, 12.0f);
	private BuildController buildController;
	
	private bool menuVisible;

    void Start() {
		mainCamera = Camera.main.transform;
		buildController = FindObjectOfType<BuildController>();
		menuVisible = true;
		transform.position = spawnPosition;
    }

    void Update() {
		if (GameManager.instance.IsStarted()) {
			Vector3 desiredPosition = spawnPosition;
			if (GameManager.instance.IsPreparationGameState()) {
				if (mainCamera != null ){
					if (buildController.IsTowerSpawnedButNotPlaced()) {
						if (menuVisible) ToggleMenu(false);
						desiredPosition = mainCamera.position + mainCamera.right * waitOffset.x + mainCamera.up * waitOffset.y + mainCamera.forward * waitOffset.z;
						desiredPosition.y = mainCamera.position.y + waitOffset.y;
					} else {
						if (!menuVisible) ToggleMenu(true);
						desiredPosition = mainCamera.position + mainCamera.right * offset.x + mainCamera.up * offset.y + mainCamera.forward * offset.z;
						desiredPosition.y = mainCamera.position.y + offset.y;
						if (desiredPosition.y < minY) desiredPosition.y = minY;
						
						transform.LookAt(mainCamera);
						transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
					}
				}
			} else {
				if (menuVisible) ToggleMenu(false);
			}
			transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
		}
    }
	
	
	private void ToggleMenu(bool isVisible) {
		menuVisible = isVisible;
		GameObject buildUIObject = transform.Find("BuildUI").gameObject;
		if (buildUIObject != null) buildUIObject.SetActive(isVisible);
		GameObject informationObject = transform.Find("Information").gameObject;
		if (informationObject != null) informationObject.SetActive(isVisible);
	}
}
