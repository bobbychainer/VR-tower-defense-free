using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTower : TowerController {
	// target transform object
	private Transform targetEnemy;
	private float attackRadius;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		damage = 1;
		// RapidTower initialization
		// get top position of tower (CenterSphere)
		attackStartPosition = gameObject.transform.Find("TopSpheres/CenterSphere").position;
		attackRadius = 10f;
	}
	
	// return true if target got set
	protected override bool TargetDetected() {
		Collider[] enemies = EnemiesInRange();
		targetEnemy = FindNearestEnemy(enemies);
		return targetEnemy != null;
	}
	
	// attack nearest enemie with SmallTowerBullet
	protected override void Attack() {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		
		SmallTowerBullet bullet = towerBullet.GetComponent<SmallTowerBullet>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}
	
	// get enemie collider in sphere with attackradius
	private Collider[] EnemiesInRange() {
		return Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);
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