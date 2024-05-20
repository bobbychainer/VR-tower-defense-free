using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTower : TowerController {
	
	private float attackRadius = 10f;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	
    protected override void Start() {
		base.Start();
		attackStartPosition = gameObject.transform.Find("TopSpheres/CenterSphere").position;
		Debug.Log(attackStartPosition);
	}
	
	protected override Collider[] EnemiesInRange() {
		return Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);
	}
	
	protected override void AttackEnemy(Transform enemy) {
		Debug.Log("Target " + enemy.name);
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		
		SmallTowerBullet bullet = towerBullet.GetComponent<SmallTowerBullet>();
		if (bullet != null) bullet.Initialize(targetEnemy, damage);
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}
