using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

// parent class for all towers
public class TowerController : MonoBehaviour {
	private Transform targetEnemy;
	public LayerMask enemyLayer;
	private float lastAttackTime = 0f;
	public GameObject bulletPrefab;
	private int damage = 1;
	private int towerHealth = 20;
	private float attackRadius = 10f;
	private float attackCooldown = 1f;

	void Update() {
		// find all enemies in radius
		Collider[] enemyColliders = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

		// choose nearest enemy
		targetEnemy = FindNearestEnemy(enemyColliders);

		// attack nearest enemy
		if (targetEnemy != null && Time.time - lastAttackTime >= attackCooldown) {
			AttackEnemy(targetEnemy);
			lastAttackTime = Time.time;
		}
	}

	void AttackEnemy(Transform enemy) {
		// initialize bullet
		Debug.Log("Target " + enemy.name);
		GameObject towerBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
		TowerBulletController bullet = towerBullet.GetComponent<TowerBulletController>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}

	// searches for nearest enemy in attackradius
	Transform FindNearestEnemy(Collider[] enemyColliders) {
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
	
	// draw attackradius from tower
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}
