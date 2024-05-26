using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform targetLocation; // The target location to move to
    public InputActionProperty triggerAction; // The input action for the trigger

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
}
