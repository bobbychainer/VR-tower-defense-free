using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidTowerBullet : BulletController {
	
    // bullet target
	private Transform target;
	
	// return true if target destroyed or hit
	protected override bool TargetReached() {
		return (targetReached || target == null);
	}
	
	// move bullet 
	protected override void MoveToTarget() {
		Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;
		
		transform.Translate(direction.normalized * distanceThisFrame, Space.World);
	}
	
	// initialize bullet damage and target
	public void Initialize(Transform _target, float _damage) {
        damage = _damage;
		target = _target;
    }
}
