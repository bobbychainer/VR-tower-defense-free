using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerController {
	
	private float attackRadius = 10f;
	public GameObject bulletPrefab;
	private Vector3 attackStartPosition;
	private Vector3 attackEndPosition;
	
    // Start is called before the first frame update
    protected override void Start() {
		base.Start();
        attackStartPosition = transform.Find("Canon").position;
		Vector3 frontPosition = transform.Find("Front").position;
		
		attackEndPosition = GetEndPosition(frontPosition);
    }
	
	private Vector3 GetEndPosition(Vector3 frontPosition) {
		Vector3 attackVector = (frontPosition - attackStartPosition).normalized;
		
		if (attackVector.x == 0 && attackVector.z != 0) {
			return new Vector3(attackStartPosition.x, attackStartPosition.y, (attackVector.z + 1f) * 25f);
		} else if (attackVector.x != 0 && attackVector.z == 0) {
			return new Vector3((attackVector.x + 1f) * 25f, attackStartPosition.y, attackStartPosition.z);
		}
		return frontPosition;
	}
	
	protected override Collider[] EnemiesInRange() {
		RaycastHit hit;
		Vector3 attackDirection = attackEndPosition - attackStartPosition;
		if (Physics.Raycast(attackStartPosition, attackDirection, out hit, 20, enemyLayer)) {
			Debug.DrawRay(transform.position, attackDirection, Color.green);
			return new Collider[] {hit.collider};
		}
		return null;
	}
	
	protected override void AttackEnemy(Transform enemy) {
		GameObject towerBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);
		towerBullet.transform.rotation = transform.rotation * bulletPrefab.transform.rotation;
		
		LaserTowerBullet bullet = towerBullet.GetComponent<LaserTowerBullet>();
		if (bullet != null) bullet.Initialize(attackEndPosition, damage);
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}

}
