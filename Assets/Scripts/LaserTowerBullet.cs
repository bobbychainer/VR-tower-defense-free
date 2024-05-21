using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTowerBullet : BulletController {
    
	// bullet target position
	private Vector3 target;
	private float distanceToTarget;
	
	// returns true if bullet reached game border (50)
	protected override bool TargetReached() {
		float distance = Vector3.Distance(transform.position, target);
		// compare if distance to target is increasing
		// maybe not the best solution
		if (distance > distanceToTarget) {
			targetReached = true;
			return true;
		}
		distanceToTarget = distance;
		return false;
	}
	
	// move bullet 
	protected override void MoveToTarget() {
		Vector3 direction = target - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;
		
		transform.Translate(direction.normalized * distanceThisFrame, Space.World);
	}
	
	// initialize bullet damage and target
	public void Initialize(Vector3 _target, int _damage) {
        damage = _damage;
		target = _target;
		distanceToTarget = Vector3.Distance(transform.position, target);
    }
	
}
