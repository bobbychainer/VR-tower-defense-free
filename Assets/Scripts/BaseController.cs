using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            Debug.Log("Coll");
            Destroy(other.gameObject);
        }
    }
}
