using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : EnemyController
{
    public LayerMask towerLayer;
    public LayerMask playerLayer;  // Layer mask for the player
    protected float lastAttackTime = 0f;
    protected int damage = 1;
    protected float attackCooldown = 10f;

    private Transform targetEnemy;
    private Transform targetPlayer;  // Reference to the player
    private float attackRadius = 5f;
    public GameObject bulletPrefab;
    private Vector3 attackStartPosition;

    private Animator animator;

    protected override void Start() {
        base.Start();
        enemyHealth = 3;
        enemyValue = 3;
        enemySpeed = 1f;
        attackCooldown = 4f;
        damage = 1;
        attackStartPosition = gameObject.transform.position;
        animator = GetComponent<Animator>();
    }

    protected override void Update() {
        base.Update();
        // Attack if not on cooldown and if target is nearby
        if (TargetDetected() && AttackReady()) {
            Attack();
            lastAttackTime = Time.time;
        }
        attackStartPosition = gameObject.transform.position;
    }

    private Transform FindNearestTarget(Collider[] targetColliders) {
        if (targetColliders == null) return null;

        Transform nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider collider in targetColliders) {
            float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
            if (distanceToTarget < shortestDistance) {
                shortestDistance = distanceToTarget;
                nearestTarget = collider.transform;
            }
        }
        return nearestTarget;
    }

    protected virtual bool TargetDetected() {
        // Check for player in range
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        if (playersInRange.Length > 0) {
            targetPlayer = playersInRange[0].transform;  // Assuming there's only one player
            //Debug.Log("Player detected!");
        } else {
            targetPlayer = null;
            //Debug.Log("No player detected.");
        }
        if (targetPlayer != null) {
            //Debug.Log("Targeting player");
            return true;
        }

        // Check for other enemies in range
        Collider[] enemiesInRange = EnemiesInRange();
        // Check if target is still in range, else change targetEnemy
        if (targetEnemy == null || TargetOutOfRange(enemiesInRange)) {
            targetEnemy = FindNearestTarget(enemiesInRange);
        }

        if (targetEnemy != null) {
            //Debug.Log("Targeting enemy");
            return true;
        }
        return false;
    }

    protected virtual void Attack() {
        GameObject enemyBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);

        EnemyShootingBullet bullet = enemyBullet.GetComponent<EnemyShootingBullet>();

        if (bullet != null) {
            if (targetPlayer != null) {
                bullet.Initialize(targetPlayer, damage);

                if (animator != null) animator.SetTrigger("Shoot");
                
            } else if (targetEnemy != null) {
                bullet.Initialize(targetEnemy, damage);
                
                if (animator != null) animator.SetTrigger("Shoot");
                
            }
        }
    }

    protected virtual bool AttackReady() { return (Time.time - lastAttackTime >= attackCooldown); }

    // Get enemy colliders in sphere with attackRadius
    private Collider[] EnemiesInRange() { return Physics.OverlapSphere(transform.position, attackRadius, towerLayer); }

    private bool TargetOutOfRange(Collider[] enemyColliders) {
        foreach (Collider collider in enemyColliders) {
            if (targetEnemy != null && targetEnemy.position.Equals(collider.transform.position)) return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
