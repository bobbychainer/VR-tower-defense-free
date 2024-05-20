using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController: MonoBehaviour {
	protected Transform targetEnemy;
	public LayerMask enemyLayer;
	protected float lastAttackTime = 0f;
	protected int damage = 1;
	protected int towerHealth = 20;
	protected float attackCooldown = 1f;
	
	protected virtual void Start() {
		
	}

	protected virtual void Update() {
		// find all enemies in radius
		Collider[] enemyColliders = EnemiesInRange();
		
		// choose nearest enemy
		targetEnemy = FindNearestEnemy(enemyColliders);

		// attack nearest enemy
		if (targetEnemy != null && Time.time - lastAttackTime >= attackCooldown) {
			AttackEnemy(targetEnemy);
			lastAttackTime = Time.time;
		}
	}
	
	protected virtual Collider[] EnemiesInRange() {
		return null;
	}

	protected virtual void AttackEnemy(Transform enemy) {
		return;
	}

	// searches for nearest enemy in attackradius
	private Transform FindNearestEnemy(Collider[] enemyColliders) {
		
		if (enemyColliders == null) return null;
		
		Transform nearestEnemy = null;
		float shortestDistance = Mathf.Infinity;

		foreach (Collider collider in enemyColliders) {
			float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
			if (distanceToEnemy < shortestDistance) {
				shortestDistance = distanceToEnemy;
				nearestEnemy = collider.transform;
			}
		}
		return nearestEnemy;
	}
}
