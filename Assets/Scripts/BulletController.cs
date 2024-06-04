using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    protected float bulletSpeed = 10f;
    protected int damage;
	protected bool targetReached = false;

    protected virtual void Start() {

	}
	
	protected virtual void Update() {
        // if target is reached destroy bullet object
		if (TargetReached()) {
			Destroy(gameObject);
            return;
		} 
		// calculate bullet direction
		MoveToTarget();
    }
	
	// return true if target is reached (or target destroyed or attack used)
	protected virtual bool TargetReached() {
		return false;
	}
	
	// move bullet towards target direction
	protected virtual void MoveToTarget() {
		return;
	}
	
	// set targetReached if bullet hit target
	public void TargetHit() {
		targetReached = true;
	}
	
	// return damage for enemy calculations
	public int GetDamage() {
		return damage;
	}
}
