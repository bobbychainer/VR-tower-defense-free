using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// parent class for all enemies
public class EnemyController : MonoBehaviour {
    protected static float healthIncrease = 0.5f;
    protected static float valueIncrease = 0.5f;
    protected static float speedIncrease = 0.1f;
    protected GenerateCubes generateCubes; // Reference to the GenerateCubes script
    protected GameManager gameManager; //Reference to the GameManager script
    protected int currentWaypointIndex = 0;
    protected NavMeshAgent agent;
    protected float enemyHealth = 1; // health to destroy
    protected float enemyValue = 1; // score for player and damage to base
    protected Transform[] waypoints;
    protected float enemySpeed = 4f;
    
    private AudioManager audioManager;


    protected virtual void Start() {
        generateCubes = FindObjectOfType<GenerateCubes>(); // Find the GenerateCubes script
        waypoints = generateCubes.GetWaypoints(); // Initialize waypoints
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // Find the GenerateCubes script

        SetDestinationToNextWaypoint();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    protected virtual void Update() {
        // Check if the agent has reached the current waypoint
        if (agent.remainingDistance <= agent.stoppingDistance + 0.2) {
            // Move to the next waypoint
            currentWaypointIndex++;
            SetDestinationToNextWaypoint();
        }
        //In Update Methode, damit später slows oder sogar speedups implementiert werden können
        agent.speed = enemySpeed;
    }

    // increase enemy stats for spawner
    public static void IncreaseEnemyStats() {
        healthIncrease++;
        valueIncrease++;
        speedIncrease += 0.1f;
    }

    public float GetEnemyValue() { return enemyValue; }

    // Check collision
    private void OnTriggerEnter(Collider other) {
        //Collision Towerbullet -> Enemy
        if (other.gameObject.tag == "Bullet") {
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();
            if (bulletController != null) {
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
            }
        }
        //Collision PlayerBullet -> Enemy
        if (other.gameObject.tag == "PlayerBullet") {
            int damage = other.gameObject.GetComponent<PlayerBullet>().GetDamage();
            TakeDamage(damage);
            Destroy(other.gameObject);
        }
		//Coll PlayerBullet -> Base
		if (other.gameObject.tag == "Base") {
			// base takes damage
			gameManager.TakeBaseDamage(enemyValue);
			Destroy(gameObject);
		}
    }

    // Check if there are waypoints remaining
    protected virtual void SetDestinationToNextWaypoint() { if (currentWaypointIndex < waypoints.Length) agent.SetDestination(waypoints[currentWaypointIndex].position);  }
	
    // enemy takes damage from bullet and player gets score
    public void TakeDamage(int damage) {
        enemyHealth -= damage;
        if (enemyHealth <= 0) {
            Destroy(gameObject);
            audioManager.PlaySFX(audioManager.enemyDeadSound);
            //Debug.Log("Enemy destroyed.");
            GameManager.instance.UpdatePlayerScore(enemyValue);
        }
    }
}
