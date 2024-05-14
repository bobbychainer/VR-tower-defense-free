using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// parent class for all enemies
public class EnemyController : MonoBehaviour {
    protected GenerateCubes generateCubes; // Reference to the GenerateCubes script
    protected int currentWaypointIndex = 0;
    protected NavMeshAgent agent;
    protected int enemyHealth = 1; // health to destroy
    protected int enemyValue = 1; // score for player and damage to base

    protected virtual void Start() {
        generateCubes = FindObjectOfType<GenerateCubes>(); // Find the GenerateCubes script
        agent = GetComponent<NavMeshAgent>();
        SetDestinationToNextWaypoint();
    }

    protected virtual void Update() {
        // Check if the agent has reached the current waypoint
        if (agent.remainingDistance <= agent.stoppingDistance) {
            // Move to the next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex < generateCubes.waypoints.Length) {
                SetDestinationToNextWaypoint();
            } else {
                // Reached the last waypoint (Base), destroy the enemy
                Debug.Log("Enemy reached Base.");
                // rufe GM auf für base dmg 
                Destroy(gameObject);
            }
        }
    }
    protected virtual void SetDestinationToNextWaypoint() {
        // Check if there are waypoints remaining
        if (currentWaypointIndex < generateCubes.waypoints.Length) agent.SetDestination(generateCubes.waypoints[currentWaypointIndex].position);
    }

    public void TakeDamage(int damage) {
        enemyHealth -= damage;
        if (enemyHealth <= 0) {
            Destroy(gameObject);
            Debug.Log("Enemy destroyed.");
            GameManager.instance.UpdatePlayerScore(enemyValue);
        }
    }

}
