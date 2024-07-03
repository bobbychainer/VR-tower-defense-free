using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour {
	
	public bool fixedRotationSpeed = true;
	
	public float xRotationSpeedLimit;
	public float yRotationSpeedLimit;
	public float zRotationSpeedLimit;
	
	private float xRotationSpeed;
	private float yRotationSpeed;
	private float zRotationSpeed;
	
    // Start is called before the first frame update
    void Start() {
		xRotationSpeed = xRotationSpeedLimit;
		yRotationSpeed = yRotationSpeedLimit;
		zRotationSpeed = zRotationSpeedLimit;
    }

    // Update is called once per frame
    void Update() {
		if (! fixedRotationSpeed) {
			xRotationSpeed = Random.Range(0.0f, xRotationSpeedLimit);
			yRotationSpeed = Random.Range(0.0f, yRotationSpeedLimit);
			zRotationSpeed = Random.Range(0.0f, zRotationSpeedLimit);
		}
        transform.Rotate(xRotationSpeed, yRotationSpeed, zRotationSpeed);
    }
}
