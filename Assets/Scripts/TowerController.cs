using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class TowerController: MonoBehaviour {
	public LayerMask enemyLayer;
	protected float lastAttackTime = 0f;
	protected int damage = 1;
	protected int totalTowerHealth = 100;
	protected int currentTowerHealth = 100;
	protected float attackCooldown = 1f;
	
	protected bool placed = false;
	
	protected GameObject healthObject;
	
	protected virtual void Start() {
		healthObject = gameObject.transform.Find("HealthBar/Background/Anchor/Health").gameObject;
		Debug.Log(healthObject);
	}

	protected virtual void Update() {
		// no target detection or attack without being placed
		if (placed) {
			Debug.Log("works");
			// attack if not on cooldown and if target near by
			if (TargetDetected() && AttackReady()) {
				Attack();
				lastAttackTime = Time.time;
			}
		}
	}
	
	// initialize tower
	protected virtual void Initialize() {
		return;
	}
	
	// target detected = true ; no target near by = false;
	protected virtual bool TargetDetected() {
		return false;
	}
	
	// attack cooldown
	// returns true if attack ready
	protected virtual bool AttackReady() {
		return (Time.time - lastAttackTime >= attackCooldown);
	}

	// attack target (depends on tower type)
	protected virtual void Attack() {
		return;
	}
	
	// activate tower
	public void PlaceTower() {
		Initialize();
		placed = true;
	}

    private void OnTriggerEnter(Collider other) { //TODO: Add LifeBar
		//Collision EnemyBullet -> Tower
        if (other.gameObject.tag == "EnemyBullet")
        {
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();
            if (bulletController != null)
            {
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
				Destroy(other.gameObject);
            }
        }
    }
	
	private void UpdateHealthBar() {
		// Calculate the new scale for the health object based on the current health percentage
		Vector3 newScale = healthObject.transform.localScale;
		float healthPercentage = (float)currentTowerHealth / (float)totalTowerHealth;
		newScale.x = healthPercentage;

		// Apply the new scale
		healthObject.transform.localScale = newScale;

		// Adjust the position to keep the left side anchored
		Vector3 newPos = healthObject.transform.localPosition;
		float originalWidth = 1f; // Assuming the original width of the health bar is 1 unit
		newPos.x = (healthPercentage - 1) * (originalWidth / 2);

		// Apply the new position
		healthObject.transform.localPosition = newPos;
	}

    public void TakeDamage(int damage)
    {
        currentTowerHealth -= damage;
		// update healthObject
		UpdateHealthBar();
        if (currentTowerHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Tower destroyed.");
            //GameManager.instance.UpdatePlayerScore(enemyValue); Minuspunkte hinzuf�gen?
			
        }
    }
	
	public bool hasBeenPlaced() {
		return placed;
	}


}
