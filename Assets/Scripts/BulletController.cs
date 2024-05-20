using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    protected float bulletSpeed = 10f;
    protected int damage;
	protected bool targetReached = false;
	
	protected virtual void Update() {
        // if target does not exist, destroy bullet (from other tower)
		if (TargetReached()) {
			Destroy(gameObject);
            return;
		} 
		
		// calculate direction
		MoveToTarget();
    }
	
	protected virtual bool TargetReached() {
		return false;
	}
	
	protected virtual void MoveToTarget() {
		return;
	}
	
	public void TargetHit() {
		targetReached = true;
	}
	
	public int GetDamage() {
		return damage;
	}
}
