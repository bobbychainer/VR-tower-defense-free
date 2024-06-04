using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingBullet : BulletController
{

    // bullet target
    private Transform target;
    private Vector3 towerTargetOffset = new Vector3(0,1f,0);


    protected override void Start()
    {
        bulletSpeed = 3f;
        damage = 1;
    }

    // return true if target destroyed or hit
    protected override bool TargetReached()
    {
        return (targetReached || target == null);
    }

    // move bullet 
    protected override void MoveToTarget()
    {
        Vector3 direction = target.transform.position - transform.position;
        direction += towerTargetOffset;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    // initialize bullet damage and target
    public void Initialize(Transform _target, int _damage)
    {

        damage = _damage;
        target = _target;
    }


}
