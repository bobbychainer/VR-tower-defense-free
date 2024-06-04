using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationRaySnap : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public LayerMask teleportationAnchorLayer;
    public float snappingRange = 1.0f; // Adjust this to your preference
    public LineRenderer lineRenderer;

    private void Update()
    {
        // Get the current ray hit information
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Find the nearest teleportation anchor
            TeleportationAnchor nearestAnchor = FindNearestAnchor(hit.point);

            if (nearestAnchor != null)
            {
                // Snap the ray to the nearest anchor
                Vector3 anchorPosition = nearestAnchor.transform.position;
                float distanceToAnchor = Vector3.Distance(hit.point, anchorPosition);

                if (distanceToAnchor <= snappingRange)
                {
                    // Set the ray end point to the anchor position
                    Vector3[] positions = new Vector3[2];
                    positions[0] = rayInteractor.transform.position;
                    positions[1] = anchorPosition;

                    lineRenderer.SetPositions(positions);
                }
                else
                {
                    ResetLineRenderer(hit.point);
                }
            }
            else
            {
                ResetLineRenderer(hit.point);
            }
        }
        else
        {
            ResetLineRenderer(rayInteractor.transform.position + rayInteractor.transform.forward * rayInteractor.maxRaycastDistance);
        }
    }

    private void ResetLineRenderer(Vector3 endPoint)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = rayInteractor.transform.position;
        positions[1] = endPoint;

        lineRenderer.SetPositions(positions);
    }

    private TeleportationAnchor FindNearestAnchor(Vector3 hitPoint)
    {
        float closestDistance = float.MaxValue;
        TeleportationAnchor nearestAnchor = null;

        // Find all teleportation anchors in the scene
        TeleportationAnchor[] anchors = FindObjectsOfType<TeleportationAnchor>();

        foreach (var anchor in anchors)
        {
            float distance = Vector3.Distance(hitPoint, anchor.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestAnchor = anchor;
            }
        }

        return nearestAnchor;
    }
}
