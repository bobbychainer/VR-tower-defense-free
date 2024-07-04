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
    protected Transform[] waypoints;
    protected float enemySpeed = 5f;

    protected virtual void Start() {
        generateCubes = FindObjectOfType<GenerateCubes>(); // Find the GenerateCubes script
        waypoints = generateCubes.GetWaypoints(); // Initialize waypoints
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // Find the GenerateCubes script

        SetDestinationToNextWaypoint();
    }

    protected virtual void Update() {
        // Check if the agent has reached the current waypoint
        if (agent.remainingDistance <= agent.stoppingDistance + 0.2) {
            // Move to the next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex < waypoints.Length) {
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

    // Check collision
    private void OnTriggerEnter(Collider other)
    {
        //Collision Towerbullet -> Enemy
        if (other.gameObject.tag == "Bullet")
        {
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();
            if (bulletController != null)
            {
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
            }
        }
        //Collision PlayerBullet -> Enemy
        if (other.gameObject.tag == "PlayerBullet")
        {
            int damage = other.gameObject.GetComponent<PlayerBullet>().GetDamage();
            TakeDamage(damage);
            Destroy(other.gameObject);
            
        }
    }

    // Check if there are waypoints remaining
    protected virtual void SetDestinationToNextWaypoint() {
        if (currentWaypointIndex < waypoints.Length) agent.SetDestination(waypoints[currentWaypointIndex].position);
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
