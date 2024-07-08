using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingBullet : BulletController
{
    // Bullet target
    private Vector3 target;
    private Vector3 towerTargetOffset = new Vector3(0, 0.5f, 0); // Offset to target the lower body

    protected override void Start()
    {
        bulletSpeed = 3f;
        damage = 1;
    }

    // Return true if target destroyed or hit
    protected override bool TargetReached()
    {
        return targetReached;
    }

    // Move bullet towards the target
    protected override void MoveToTarget()
    {
        Vector3 targetPosition = target + towerTargetOffset;
        Vector3 direction = targetPosition - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            // Bullet reached the target
            targetReached = true;
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    // Initialize bullet damage and target
    public void Initialize(Vector3 _target, int _damage)
    {
        damage = _damage;
        target = _target;
    }
}
