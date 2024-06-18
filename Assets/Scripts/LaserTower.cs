using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerController {
	
	private float attackDistance;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	//target position
	private Vector3 attackEndPosition;
	private Vector3[] attackEndPositions;
	public int laserTowerPrice = 100;
	
	// initialize tower
	protected override void Initialize() {
		// TowerController initialization
		attackCooldown = 5f;
		damage = 1;
		// RapidTower initialization
		attackDistance = 10f;
		attackStartPosition = transform.Find("Canons/Center").position;
		
		attackEndPositions = GetAxisAlignedEndPositions();
	}
	
	// return true if raycast on enemyLayer detects enemy 
	protected override bool TargetDetected() {
		foreach (Vector3 attackEndPosition in attackEndPositions) {
			Vector3 attackDirection = attackEndPosition - attackStartPosition;
			if (Physics.Raycast(attackStartPosition, attackDirection, attackDistance, enemyLayer)) {
				return true;
			}
		}
		return false;
	}
	
	// shot LaserTowerBullet towards attackEndPosition
	protected override void Attack() {
		CreateBullet(attackEndPositions[0], Quaternion.identity);
		CreateBullet(attackEndPositions[1], Quaternion.identity);
		CreateBullet(attackEndPositions[2], Quaternion.Euler(0, 0, 90));
		CreateBullet(attackEndPositions[3], Quaternion.Euler(0, 0, 90));
	}

	private void CreateBullet(Vector3 endPosition, Quaternion additionalRotation) {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		
		// Apply the base rotation and the additional rotation
		towerBullet.transform.rotation = bulletPrefab.transform.rotation * additionalRotation;
		
		LaserTowerBullet bullet = towerBullet.GetComponent<LaserTowerBullet>();
		if (bullet != null) bullet.Initialize(endPosition, damage);
	}
	
	private Vector3[] GetAxisAlignedEndPositions() {
		Vector3 frontPosition1 = transform.Find("Canons/Front1").position;
		Vector3 frontPosition2 = transform.Find("Canons/Front2").position;
		Vector3 frontPosition3 = transform.Find("Canons/Front3").position;
		Vector3 frontPosition4 = transform.Find("Canons/Front4").position;
				
		Vector3 endPosition1 = new Vector3(frontPosition1.x, frontPosition1.y, 0f);
		Vector3 endPosition2 = new Vector3(frontPosition2.x, frontPosition2.y, 50f);
		Vector3 endPosition3 = new Vector3(0f, frontPosition3.y, frontPosition3.z);
		Vector3 endPosition4 = new Vector3(50f, frontPosition4.y, frontPosition4.z);

		return new Vector3[] { endPosition1, endPosition2, endPosition3, endPosition4 };
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		foreach (Vector3 attackEndPosition in attackEndPositions) {
			Vector3 attackDirection = attackEndPosition - attackStartPosition;
			Gizmos.DrawRay(attackStartPosition, attackDirection);
		}
	}

}