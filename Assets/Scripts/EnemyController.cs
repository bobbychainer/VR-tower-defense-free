using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// parent class for all enemies
public class EnemyController : MonoBehaviour {
    protected GenerateCubes generateCubes; // Reference to the GenerateCubes script
    protected GameManager gameManager; //Reference to the GameManager script
    protected int currentWaypointIndex = 0;
    protected NavMeshAgent agent;
    protected int enemyHealth = 1; // health to destroy
    protected int enemyValue = 1; // score for player and damage to base

    protected int enemySpeed = 8;

    protected virtual void Start() {
        generateCubes = FindObjectOfType<GenerateCubes>(); // Find the GenerateCubes script
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // Find the GenerateCubes script

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
                gameManager.TakeBaseDamage(enemyValue);
                Destroy(gameObject);
            }
        }
        //In Update Methode, damit später slows oder sogar speedups implementiert werden können
        agent.speed = enemySpeed;

    }

    public int GetEnemyValue() { return enemyValue; }
    /*
    // Check collision enemies with bullets
    private void OnCollisionEnter(Collision collision) {
		
		if (collision.gameObject.tag == "Bullet") {
			//Debug.Log("Hit "+collision.gameObject);
			BulletController bulletController = collision.gameObject.GetComponent<BulletController>();
			
			if (bulletController != null) {
				int damage = bulletController.GetDamage();
				bulletController.TargetHit();
				TakeDamage(damage);
			}
		}

	}
    */
    // Check collision enemies with bullets
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            // Debug.Log("Hit " + other.gameObject);
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();

            if (bulletController != null)
            {
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
            }
        }
    }

    // Check if there are waypoints remaining
    protected virtual void SetDestinationToNextWaypoint() {
        if (currentWaypointIndex < generateCubes.waypoints.Length) agent.SetDestination(generateCubes.waypoints[currentWaypointIndex].position);
    }
	
    // enemy takes damage from bullet and player gets score
    public void TakeDamage(int damage) {
        enemyHealth -= damage;
        if (enemyHealth <= 0) {
            Destroy(gameObject);
            Debug.Log("Enemy destroyed.");
            GameManager.instance.UpdatePlayerScore(enemyValue);
        }
    }
}
