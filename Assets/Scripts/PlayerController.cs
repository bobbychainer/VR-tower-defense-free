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
    public Transform overviewLocation;
    public InputActionProperty returnTriggerAction;
    public InputActionProperty shootTriggerAction;
    public bool freezePlayer = true;
    public ActionBasedContinuousMoveProvider moveProvider;
    public GameObject bulletPrefab;   // The bullet prefab to instantiate
    public float bulletSpeed = 20f;   // Speed of the bullet
    public float fireRate = 2f;     // Rate of fire in seconds
    private float nextFireTime = 1f;  // Time until the next shot can be fired
    public GameObject bulletSpawnPoint; //Position where Bullet should come out


    private AudioManager audioManager;

    public void Start(){
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        freezePlayer = true;
    }


  
    void Update() { 
        if (freezePlayer && moveProvider.enabled) {
            moveProvider.enabled = false;
        } else if (!freezePlayer && !moveProvider.enabled) {
            moveProvider.enabled = true;
        }
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
        if (freezePlayer == false) {
            MovePlayerToLocation();
        }
    }

        private void OnShootTriggerPressed(InputAction.CallbackContext context) {        
        if (Time.time >= nextFireTime && GameManager.instance.IsAttackGameState() && !freezePlayer) {
            ShootBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void MovePlayerToLocation() {
        if (overviewLocation) {
            // Move the XR Origin to the target location
            transform.position = overviewLocation.position;
            transform.rotation = overviewLocation.rotation;
        } else {
            Debug.LogWarning("Target location is not set.");
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Freeze") {
            freezePlayer = true;
        }
        if (other.gameObject.tag == "Ground") {
            freezePlayer = false;
        }
        //Check Collision EnemyBullet -> Player
        if (other.gameObject.tag == "EnemyBullet") {
            //Debug.Log("Hit EnemyBullet -> Player" + other.gameObject);
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();

            if (bulletController != null) {
                //Debug.Log("bullCon != null");
                float damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
                Destroy(other.gameObject);
            }
        }
        // Killbox Collision
        if (other.gameObject.tag == "Killbox") {
            SpawnPlayer();
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
            audioManager.PlaySFX(audioManager.playerShootBullet);

            //Debug.Log("Bullet velocity set to: " + rb.velocity);
        } else {
            Debug.LogError("Bullet Rigidbody not found!");
        }
    }

    public void TakeDamage(float damage) {
        audioManager.PlaySFX(audioManager.playerGettingHurt);
        GameManager.instance.TakePlayerDamage(damage); 
        
    }

    public void RespawnPlayer() { transform.position = initialLocation.position; }
    public void SpawnPlayer() { transform.position = overviewLocation.position; }

    public void FreezePlayer() {
        Debug.Log("FROZEN");
        transform.position = overviewLocation.position;
        freezePlayer = true;   
    }
}