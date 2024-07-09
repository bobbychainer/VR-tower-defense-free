using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    protected float damage = 0.8f;

    void Start()
    {
        // Destroy the bullet after a certain time to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet forward each frame
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public float GetDamage() {
		return damage;
	}


}