using UnityEngine;

public class MouseHoverDetector : MonoBehaviour {
    private Camera cam;
    private Ray currentRay;

    public LayerMask layerMask;

    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        TrackMouseHover();
    }

    private void TrackMouseHover() {
        RaycastHit hit;
        Vector3 adjustedMousePosition = AdjustMousePosition(Input.mousePosition);

        currentRay = cam.ScreenPointToRay(adjustedMousePosition);

        if (Physics.Raycast(currentRay, out hit, Mathf.Infinity, layerMask)) {
            //Debug.Log("Hovering over: " + hit.collider.gameObject.name);
        } else {
            //Debug.Log("Hovering over: Nothing");
        }

        Debug.DrawRay(currentRay.origin, currentRay.direction * 1000, Color.green);
    }

    private Vector3 AdjustMousePosition(Vector3 originalMousePosition) {
        // Adjust the mouse position to account for any offsets due to the XR setup
        Vector3 screenPoint = originalMousePosition;
        
        // Convert screen point to viewport point, taking into account the XR origin's offset and scaling
        Vector3 viewportPoint = cam.ScreenToViewportPoint(screenPoint);

        // Adjust viewport point back to screen space, correcting for the XR setup's viewport
        Vector3 adjustedScreenPoint = cam.ViewportToScreenPoint(viewportPoint);

        return adjustedScreenPoint;
    }
}
