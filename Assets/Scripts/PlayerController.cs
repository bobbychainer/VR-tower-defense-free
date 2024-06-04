using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public Transform targetLocation; // The target location to move to
    public InputActionProperty triggerAction; // The input action for the trigger

    protected GameManager gameManager;

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        MovePlayerToLocation();
    }

    private void MovePlayerToLocation()
    {
        if (targetLocation)
        {
            // Move the XR Origin to the target location
            transform.position = targetLocation.position;
            transform.rotation = targetLocation.rotation;
        }
        else
        {
            Debug.LogWarning("Target location is not set.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check Collision EnemyBullet -> Player
        if (other.gameObject.tag == "EnemyBullet")
        {
            Debug.Log("Hit Player" + other.gameObject);
            BulletController bulletController = other.gameObject.GetComponent<BulletController>();

            if (bulletController != null)
            {
                int damage = bulletController.GetDamage();
                bulletController.TargetHit();
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        GameManager.instance.TakePlayerDamage(damage);
    }

}
