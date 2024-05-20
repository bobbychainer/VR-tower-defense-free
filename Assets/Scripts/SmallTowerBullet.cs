using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTowerBullet : BulletController {
	
	private Transform target;
	
	protected override bool TargetReached() {
		return (targetReached || target == null);
	}
	
	protected override void MoveToTarget() {
		Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;
		
		transform.Translate(direction.normalized * distanceThisFrame, Space.World);
	}
	
	public void Initialize(Transform _target, int _damage) {
        damage = _damage;
		target = _target;
    }
	
}
