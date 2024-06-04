using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;


public class EnemyShooting : EnemyController
{
    public LayerMask towerLayer;
    protected float lastAttackTime = 0f;
    protected int damage = 1;
    protected float attackCooldown = 10f;

    private Transform targetEnemy;
    private float attackRadius = 8f;
    public GameObject bulletPrefab;
    private Vector3 attackStartPosition;

    protected override void Start()
    {
        base.Start();
        enemyHealth = 3;
        enemyValue = 3;
        enemySpeed = 1f;

        attackCooldown = 4f;
        damage = 1;

        attackStartPosition = gameObject.transform.position;
    }

    protected override void Update()
    {
        base.Update();
        // attack if not on cooldown and if target near by
        if (TargetDetected() && AttackReady())
        {
            Attack();
            lastAttackTime = Time.time;
        }
        attackStartPosition = gameObject.transform.position;
    }

    private Transform FindNearestEnemy(Collider[] enemyColliders)
    {
        if (enemyColliders == null) return null;

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider collider in enemyColliders)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = collider.transform;
            }
        }
        return nearestEnemy;
    }

    protected virtual bool TargetDetected()
    {
        Collider[] enemies = EnemiesInRange();
        // check if target is still in range else change targetEnemy
        if (targetEnemy == null || TargetOutOfRange(enemies))
        {
            targetEnemy = FindNearestEnemy(enemies);
        }
        return targetEnemy != null;
    }

    protected virtual void Attack()
    {
        GameObject enemyBullet = Instantiate(bulletPrefab, attackStartPosition, Quaternion.identity);

        EnemyShootingBullet bullet = enemyBullet.GetComponent<EnemyShootingBullet>();

        if (bullet != null)  bullet.Initialize(targetEnemy, damage);
    }

    protected virtual bool AttackReady()
    {
        return (Time.time - lastAttackTime >= attackCooldown);
    }

    // get enemy collider in sphere with attackradius
    private Collider[] EnemiesInRange()
    {
        return Physics.OverlapSphere(transform.position, attackRadius, towerLayer);
    }

    private bool TargetOutOfRange(Collider[] enemyColliders)
    {
        foreach (Collider collider in enemyColliders)
        {
            if (targetEnemy.position.Equals(collider.transform.position)) return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
