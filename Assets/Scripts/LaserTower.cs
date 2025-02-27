using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerController {
	
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	//target position
	private Vector3 attackEndPosition;
	private Vector3[] attackEndPositions;
	
	// upgrades
	private int upgradeLevelIndex;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		towerName = "LASER";
		damage = 1f;
		attackCooldown = 5f;
		attackRadius = 8f;
		maxLevel = 5;
		// RapidTower initialization
		attackStartPosition = transform.Find("Canons/AttackStart").position;
		attackEndPositions = GetAxisAlignedEndPositions();
		// RapidTower upgrades
		upgradeLevelIndex = level - 1;
	}
	
	// return true if raycast on enemyLayer detects enemy 
	protected override bool TargetDetected() {
		foreach (Vector3 attackEndPosition in attackEndPositions) {
			Vector3 attackDirection = attackEndPosition - attackStartPosition;
			if (Physics.Raycast(attackStartPosition, attackDirection, attackRadius, enemyLayer)) {
				return true;
			}
		}
		return false;
	}
	
	// shot LaserTowerBullet towards attackEndPosition
	protected override void Attack() {
		CreateBullet(attackEndPositions[0], Quaternion.Euler(-90, 0, 0));
		CreateBullet(attackEndPositions[1], Quaternion.Euler(90, 0, 0));
		CreateBullet(attackEndPositions[2], Quaternion.Euler(0, 0, 90));
		CreateBullet(attackEndPositions[3], Quaternion.Euler(0, 0, -90));
	}
	
	// upgrade tower stats and design
	protected override void UpgradeTower() {
		base.UpgradeTower();
		upgradeLevelIndex += 1;
		UpgradeStates();
		UpdateRadiusRenderer();
		UpgradeDesign();
	}

	// create bullet object
	private void CreateBullet(Vector3 endPosition, Quaternion additionalRotation) {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		// apply the base rotation and the additional rotation
		towerBullet.transform.rotation = bulletPrefab.transform.rotation * additionalRotation;
		// initialize bullet with target and damage
		LaserTowerBullet bullet = towerBullet.GetComponent<LaserTowerBullet>();
		if (bullet != null) bullet.Initialize(endPosition, damage);
	}
	
	// get all 4 axis aligned end positions from a center position
	private Vector3[] GetAxisAlignedEndPositions() {
		Vector3 frontPosition1 = transform.Find("Canons/Canon1/Ring").position;
		Vector3 frontPosition2 = transform.Find("Canons/Canon2/Ring").position;
		Vector3 frontPosition3 = transform.Find("Canons/Canon3/Ring").position;
		Vector3 frontPosition4 = transform.Find("Canons/Canon4/Ring").position;
		// restrict end position x and z between 0 and 50
		Vector3 endPosition1 = new Vector3(frontPosition1.x, frontPosition1.y, 0f);
		Vector3 endPosition2 = new Vector3(frontPosition2.x, frontPosition2.y, 50f);
		Vector3 endPosition3 = new Vector3(0f, frontPosition3.y, frontPosition3.z);
		Vector3 endPosition4 = new Vector3(50f, frontPosition4.y, frontPosition4.z);
		return new Vector3[] { endPosition1, endPosition2, endPosition3, endPosition4 };
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
		if (upgradeLevelIndex == 2) {
			// deactivate level1 object
			string level1Name = "Level1";
			GameObject level1Object = transform.Find(level1Name).gameObject;
			if (level1Object != null) level1Object.SetActive(false);
		} else if (upgradeLevelIndex == 4) {
			// deactivate level2 object
			string level2Name = "Level2";
			GameObject level2Object = transform.Find(level2Name).gameObject;
			if (level2Object != null) level2Object.SetActive(false);
		}
		// show level tower object
		GameObject levelObject = transform.Find(levelObjectName).gameObject;
		if (levelObject != null) levelObject.SetActive(true);
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		foreach (Vector3 attackEndPosition in attackEndPositions) {
			Vector3 attackDirection = attackEndPosition - attackStartPosition;
			Gizmos.DrawRay(attackStartPosition, attackDirection);
		}
	}
}
