using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    private GenerateCubes generateCubes; // Reference to the GenerateCubes script

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    void Start() {
        generateCubes = GameObject.FindObjectOfType<GenerateCubes>(); // Find the GenerateCubes script
        agent = GetComponent<NavMeshAgent>();
        SetDestinationToNextWaypoint();
    }
    void Update() {
        // Check if the agent has reached the current waypoint
        if (agent.remainingDistance <= agent.stoppingDistance) {
            // Move to the next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex < generateCubes.waypoints.Length) {
                SetDestinationToNextWaypoint();
            } else {
                // Reached the last waypoint (Base), destroy the enemy
                Debug.Log("Enemy reached Base.");
                Destroy(gameObject);
            }
        }
    }
    void SetDestinationToNextWaypoint() {
        // Check if there are waypoints remaining
        if (currentWaypointIndex < generateCubes.waypoints.Length) agent.SetDestination(generateCubes.waypoints[currentWaypointIndex].position);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Base")) {
            Debug.Log("Collision Enemy with Base.");
            Destroy(other.gameObject);
        }
    }

}
