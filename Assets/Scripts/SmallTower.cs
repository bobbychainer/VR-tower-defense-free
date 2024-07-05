using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTower : TowerController {
	// target transform object
	private Transform targetEnemy;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	
	private int upgradeLevelIndex;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		towerName = "SMALL";
		damage = 1;
		attackCooldown = 1f;
		attackRadius = 10f;
		maxLevel = 7;
		// RapidTower initialization
		// get top position of tower (CenterSphere)
		attackStartPosition = transform.Find("CanonTopSphere").position;
		// RapidTower upgrades
		upgradeLevelIndex = level - 1;
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
		// initialize bullet with target and damage
		SmallTowerBullet bullet = towerBullet.GetComponent<SmallTowerBullet>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}
	
	// upgrade tower stats and design
	protected override void UpgradeTower() {
		base.UpgradeTower();
		upgradeLevelIndex += 1;
		UpgradeStates();
		UpgradeDesign();
	}
	
	// get enemie collider in sphere with attackradius
	private Collider[] EnemiesInRange() {
		return Physics.OverlapSphere(baseObject.transform.position, attackRadius, enemyLayer);
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