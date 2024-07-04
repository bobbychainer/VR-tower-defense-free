using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections;
using System.Numerics;

// TODO: Freeze Interaction/Teleport -> in prep muss wieder alles funktionieren
public class PlayerController : MonoBehaviour
{
    public Transform initialLocation;
    public InputActionProperty returnTriggerAction;
    public InputActionProperty shootTriggerAction;
    public bool freezePlayer = false;
    public ActionBasedContinuousMoveProvider moveProvider;
    public GameObject bulletPrefab;   // The bullet prefab to instantiate
    public float bulletSpeed = 20f;   // Speed of the bullet
    public float fireRate = 2f;     // Rate of fire in seconds
    private float nextFireTime = 1f;  // Time until the next shot can be fired
    public GameObject bulletSpawnPoint; //Position where Bullet should come out


    private AudioSource damageAS;
    private AudioSource shootAS;

    public void Start(){

        AudioSource[] audioSources = GetComponents<AudioSource>();
        damageAS = audioSources[0];
        shootAS = audioSources[1];
    }


  
    void Update() { 
        if (freezePlayer && moveProvider.enabled) {
            moveProvider.enabled = false;
        } else if (!freezePlayer && !moveProvider.enabled) {
            moveProvider.enabled = true;
        }
        //TODO: Implement VR Controller Button as Input
    }

    private void OnEnable() {
        returnTriggerAction.action.performed += OnReturnTriggerPressed;
        shootTriggerAction.action.performed += OnShootTriggerPressed;
    }

    private void OnDisable() {
        returnTriggerAction.action.performed -= OnReturnTriggerPressed;
        shootTriggerAction.action.performed -= OnShootTriggerPressed;
    }

    private void OnReturnTriggerPressed(InputAction.CallbackContext context) {
        MovePlayerToLocation();
    }

        private void OnShootTriggerPressed(InputAction.CallbackContext context) {        
        if (Time.time >= nextFireTime && GameManager.instance.IsAttackGameState() && !freezePlayer) {
            ShootBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void MovePlayerToLocation() {
        if (initialLocation) {
            // Move the XR Origin to the target location
            transform.position = initialLocation.position;
            transform.rotation = initialLocation.rotation;
        } else {
            Debug.LogWarning("Target location is not set.");
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Check Collision EnemyBullet -> Player
        if (other.gameObject.tag == "EnemyBullet") {
            //Debug.Log("Hit EnemyBullet -> Player" + other.gameObject);
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();

            if (bulletController != null) {
                //Debug.Log("bullCon != null");
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
                Destroy(other.gameObject);
            }
        }
        // Killbox Collision
        if (other.gameObject.tag == "Killbox") {
            RespawnPlayer();
            Debug.Log("Player hit Killbox");
        }
    }


    void ShootBullet() {
        if (bulletPrefab == null) {
            Debug.LogError("Bullet prefab is not assigned!");
            return;
        }

        // Create a bullet instance at the right controller's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        if (bullet == null) {
            Debug.LogError("Bullet instantiation failed!");
            return;
        }

        // Get the Rigidbody component of the bullet and set its velocity
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = bulletSpawnPoint.transform.forward * bulletSpeed;

            shootAS.Play();

            //Debug.Log("Bullet velocity set to: " + rb.velocity);
        } else {
            Debug.LogError("Bullet Rigidbody not found!");
        }
    }

    public void TakeDamage(int damage) {
        GameManager.instance.TakePlayerDamage(damage); 
        damageAS.Play();
    }

    public void RespawnPlayer() { transform.position = initialLocation.position; }
}