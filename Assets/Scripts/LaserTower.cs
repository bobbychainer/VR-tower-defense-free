using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerController {
	
	private float attackDistance;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	//target position
	private Vector3 attackEndPosition;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		attackCooldown = 5f;
		damage = 1;
		// RapidTower initialization
		attackDistance = 10f;
		attackStartPosition = transform.Find("Canon").position;
		// set board edge position by extending the attackStartPosition and frontPosition vector
		Vector3 frontPosition = transform.Find("Front").position;
		attackEndPosition = GetEndPosition(frontPosition);
	}
	
	// return true if raycast on enemyLayer detects enemy 
	protected override bool TargetDetected() {
		Vector3 attackDirection = attackEndPosition - attackStartPosition;
		return Physics.Raycast(attackStartPosition, attackDirection, attackDistance, enemyLayer);
	}
	
	// shot LaserTowerBullet towards attackEndPosition
	protected override void Attack() {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		towerBullet.transform.rotation = transform.rotation;// * bulletPrefab.transform.rotation;
		
		LaserTowerBullet bullet = towerBullet.GetComponent<LaserTowerBullet>();
		if (bullet != null) bullet.Initialize(attackEndPosition, damage);
	}
	
	private Vector3 GetEndPosition(Vector3 frontPosition) {
		// get normalized direction
		Vector3 attackVector = (frontPosition - attackStartPosition).normalized;
		// set x or z to {50,-50} depending on the direction
		if (attackVector.x == 0 && attackVector.z != 0) {
			return new Vector3(attackStartPosition.x, attackStartPosition.y, (attackVector.z + 1f) * 25f);
		} else if (attackVector.x != 0 && attackVector.z == 0) {
			return new Vector3((attackVector.x + 1f) * 25f, attackStartPosition.y, attackStartPosition.z);
		}
		return frontPosition;
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Vector3 attackDirection = attackEndPosition - attackStartPosition;
		Gizmos.DrawRay(attackStartPosition, attackDirection);
	}

}