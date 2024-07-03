using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScaleController : MonoBehaviour {
	
	public Camera mainCamera; // The other GameObject to measure the distance to
    public float minScale = 0.1f; // Minimum scale value
    public float maxScale = 2.0f; // Maximum scale value
	public float minDistance = 2.0f;
    public float maxDistance = 20.0f; // The distance at which the object will be at its maximum scale
	
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
		if (mainCamera != null) {
			float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
			distance = Mathf.Clamp(distance, minDistance, maxDistance);
			
			float scaleFactor = Mathf.Lerp(minScale, maxScale, (distance - minDistance) / (maxDistance - minDistance));
			
			transform.localScale = initialScale * scaleFactor;
		}
    }
}
