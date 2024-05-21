using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController: MonoBehaviour {
	public LayerMask enemyLayer;
	protected float lastAttackTime = 0f;
	protected int damage = 1;
	protected int towerHealth = 20;
	protected float attackCooldown = 1f;
	
	protected virtual void Start() {
		
	}

	protected virtual void Update() {
		// attack if not on cooldown and if target near by
		if (TargetDetected() && AttackReady()) {
			Attack();
			lastAttackTime = Time.time;
		}
	}
	
	// target detected = true ; no target near by = false;
	protected virtual bool TargetDetected() {
		return false;
	}

	// attack target (depends on tower type)
	protected virtual void Attack() {
		return;
	}
	
	// attack cooldown
	// returns true if attack ready
	protected virtual bool AttackReady() {
		return (Time.time - lastAttackTime >= attackCooldown);
	}
}
