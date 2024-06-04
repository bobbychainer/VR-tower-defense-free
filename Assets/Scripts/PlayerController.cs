using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

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
}
