using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBulletController : MonoBehaviour {
    private float bulletSpeed = 10f;
    private Transform target;
    private int damage;

    void Update() {
        // calculate direction
        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = bulletSpeed * Time.deltaTime;

        // hit enemy
        if (direction.magnitude <= distanceThisFrame) HitTarget();
            
        // move target
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget() {
        EnemyController enemyController = target.GetComponent<EnemyController>();
        if (enemyController != null) enemyController.TakeDamage(damage);
        Destroy(gameObject);
    }

    public void Initialize(Transform _target, int _damage) {
        target = _target;
        damage = _damage;
    }
}
