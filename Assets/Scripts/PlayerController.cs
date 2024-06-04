using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public Transform initialLocation;
    public InputActionProperty triggerAction;

    public bool disableMovement = false;
    public ActionBasedContinuousMoveProvider moveProvider;

    void Update()
    { 
        if (disableMovement && moveProvider.enabled)
        {
            moveProvider.enabled = false;
        }
        else if (!disableMovement && !moveProvider.enabled)
        {
            moveProvider.enabled = true;
        }
    }

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
        if (initialLocation)
        {
            // Move the XR Origin to the target location
            transform.position = initialLocation.position;
            transform.rotation = initialLocation.rotation;
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
