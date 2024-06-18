using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPanelController : MonoBehaviour {
	
	private Camera cam;
    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        // face camera
		Vector3 directionToCamera = transform.position - cam.transform.position;
        Vector3 lookPosition = transform.position + directionToCamera;

        transform.LookAt(lookPosition);
    }
}
