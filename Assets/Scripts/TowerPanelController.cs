using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPanelController : MonoBehaviour {
	
	private Camera mainCamera; // The other GameObject to measure the distance to
    private float minScale = 0.8f; // Minimum scale value
    private float maxScale = 2.0f; // Maximum scale value
	private float distanceScale = 0.1f;
	
	private Vector3 initialScale;
	
    // Start is called before the first frame update
    void Start() {		
		mainCamera = Camera.main;
		
		if (mainCamera == null) {
			Debug.LogError("Target object not set.");
			return;
		}
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {
        // face camera
		LookToCamera();
		ScaleToCamera();
    }
	
	private void LookToCamera() {
		if (mainCamera != null) {			
			Vector3 directionToCamera = transform.position - mainCamera.transform.position;
			Vector3 lookPosition = transform.position + directionToCamera;

			transform.LookAt(lookPosition);
		}
	}
	
	private void ScaleToCamera() {
		if (mainCamera != null) {
			float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
			
			float scaleFactor = distance * distanceScale;
			
			if (scaleFactor < minScale) scaleFactor = minScale;
			if (scaleFactor > maxScale) scaleFactor = maxScale;			
			
			transform.localScale = initialScale * scaleFactor;
		}
	}
}
