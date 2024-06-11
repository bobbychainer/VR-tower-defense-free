using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidTower : TowerController {
    // target transform object
	private Transform targetEnemy;
	
	private float attackRadius;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		attackCooldown = 0.5f;
		damage = 1;
		// RapidTower initialization
		// get top position of tower (CenterSphere)
		attackStartPosition = gameObject.transform.Find("TopSpheres/CenterSphere").position;
		attackRadius = 8f;
	}
	
	// return true if target got set
	protected override bool TargetDetected() {
		Collider[] enemies = EnemiesInRange();
		// check if target is still in range else change targetEnemy
		if (targetEnemy == null || TargetOutOfRange(enemies)) {
			targetEnemy = FindNearestEnemy(enemies);
		}
		return targetEnemy != null;
	}
	
	// attack nearest enemie with SmallTowerBullet
	protected override void Attack() {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		
		RapidTowerBullet bullet = towerBullet.GetComponent<RapidTowerBullet>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}
	
	// get enemie collider in sphere with attackradius
	private Collider[] EnemiesInRange() {
		return Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);
	}
	
	// return true if targetEnemy is out of range
	private bool TargetOutOfRange(Collider[] enemyColliders) {
		foreach (Collider collider in enemyColliders) {
			if (targetEnemy.position.Equals(collider.transform.position) ) return false;
		}
		return true;
	}
	
	// find nearest enemy
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
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}