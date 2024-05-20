using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTowerBullet : BulletController {
    
	private Vector3 target;
	private float distanceToTarget;
		
	protected override bool TargetReached() {
		float distance = Vector3.Distance(transform.position, target);
		if (distance > distanceToTarget) {
			targetReached = true;
			return true;
		}
		distanceToTarget = distance;
		return false;
	}
	
	protected override void MoveToTarget() {
		Vector3 direction = target - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;
		
		transform.Translate(direction.normalized * distanceThisFrame, Space.World);
	}
	
	public void Initialize(Vector3 _target, int _damage) {
        damage = _damage;
		target = _target;
		distanceToTarget = Vector3.Distance(transform.position, target);
    }
	
}
