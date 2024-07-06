using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidTower : TowerController {
    // target transform object
	private Transform targetEnemy;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	
	private int upgradeLevelIndex;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		towerName = "RAPID";
		damage = 1f;
		attackCooldown = 0.7f;
		attackRadius = 8f;
		maxLevel = 6;
		// RapidTower initialization
		// get top position of tower (CenterSphere)
		attackStartPosition = gameObject.transform.Find("Canon").position;
		// RapidTower upgrades
		upgradeLevelIndex = level - 1;
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
		// initialize bullet with target and damage
		RapidTowerBullet bullet = towerBullet.GetComponent<RapidTowerBullet>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}
	
	// upgrade tower stats and design
	protected override void UpgradeTower() {
		base.UpgradeTower();
		upgradeLevelIndex += 1;
		UpgradeStates();
		UpdateRadiusRenderer();
		UpgradeDesign();
	}
	
	// get enemie collider in sphere with attackradius
	private Collider[] EnemiesInRange() {
		return Physics.OverlapSphere(baseObject.transform.position, attackRadius, enemyLayer);
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
			float distanceToEnemy = Vector3.Distance(baseObject.transform.position, collider.transform.position);
			if (distanceToEnemy < shortestDistance) {
				shortestDistance = distanceToEnemy;
				nearestEnemy = collider.transform;
			}
		}
		return nearestEnemy;
	}
	
	// upgrade tower stats if upgrade avaible for a stat
	private void UpgradeStates() {
		var upgrades = buildController.GetAllUpgrades(towerName,upgradeLevelIndex);
		if (upgrades.damage != 0) damage = upgrades.damage;
		if (upgrades.attackCooldown != 0f) attackCooldown = upgrades.attackCooldown;
		if (upgrades.attackRadius != 0f) attackRadius = upgrades.attackRadius;
	}
	
	// upgrade tower design if design is given
	private void UpgradeDesign() {
		// get level tower object
		string levelObjectName = "Level"+upgradeLevelIndex.ToString();
		// show level tower object
		GameObject levelObject = transform.Find(levelObjectName).gameObject;
		if (levelObject != null) levelObject.SetActive(true);
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(baseObject.transform.position, attackRadius);
	}
}