using UnityEngine;

/// <summary>
/// Put this on a NEW empty GameObject called "GizmoMover".
/// It reads whichever gizmo GizmoSpawner already created, detects mouse drags
/// on the arrow handles (tagged with GizmoAxisHandle), and moves the selected object.
///
/// SETUP:
/// 1. Create empty GameObject "GizmoMover", add this script.
/// 2. Assign "Spawner" = your GizmoSpawner object.
/// 3. Assign "Cam" = your scene camera.
/// 4. On EACH arrow inside gizmoPrefab, add the GizmoAxisHandle script and set its Axis (X/Y/Z),
///    and make sure each arrow has a Collider.
/// </summary>
public class GizmoMover : MonoBehaviour
{
    [SerializeField] private GizmoSpawner spawner;
    [SerializeField] private Camera cam;

    private GizmoAxisHandle draggedHandle;
    private Vector3 dragAxisWorld;
    private Vector3 dragStartObjectPos;
    private Vector3 dragStartHitPoint;

    void Update()
    {
        Debug.Log("tick");

        if (spawner == null || cam == null)
        {
            Debug.Log("EXIT: spawner or cam not assigned");
            return;
        }

        GameObject gizmo = spawner.CurrentGizmo;
        SandboxObject target = spawner.CurrentObject;

        if (gizmo == null || target == null)
        {
            Debug.Log("EXIT: gizmo=" + gizmo + " target=" + target);
            draggedHandle = null;
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Start drag
        if (draggedHandle == null && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse down, casting ray...");
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log("Hit: " + hit.collider.name);
                GizmoAxisHandle handle = hit.collider.GetComponentInParent<GizmoAxisHandle>();
                if (handle != null)
                {
                    Debug.Log("Found GizmoAxisHandle, axis=" + handle.axis);
                    draggedHandle = handle;
                    dragAxisWorld = AxisToWorldDirection(handle.axis, gizmo.transform);
                    dragStartObjectPos = target.transform.position;

                    if (ClosestPointOnAxis(ray, dragStartObjectPos, dragAxisWorld, out Vector3 hitPoint))
                        dragStartHitPoint = hitPoint;
                }
                else
                {
                    Debug.Log("EXIT: hit collider has no GizmoAxisHandle on it or its parents");
                }
            }
            else
            {
                Debug.Log("Missed");
            }
        }

        // Continue drag
        if (draggedHandle != null && Input.GetMouseButton(0))
        {
            if (ClosestPointOnAxis(ray, dragStartObjectPos, dragAxisWorld, out Vector3 currentPoint))
            {
                Vector3 delta = currentPoint - dragStartHitPoint;
                target.transform.position = dragStartObjectPos + delta;
                // NOTE: if SandboxObject has its own SetPosition(Vector3) method
                // (it clearly has a GetPosition() one), use that instead of
                // target.transform.position so any internal state stays in sync:
                // target.SetPosition(dragStartObjectPos + delta);
            }
        }

        // End drag
        if (Input.GetMouseButtonUp(0))
        {
            draggedHandle = null;
        }
    }

    Vector3 AxisToWorldDirection(GizmoAxisHandle.Axis axis, Transform gizmoTransform)
    {
        switch (axis)
        {
            case GizmoAxisHandle.Axis.X: return gizmoTransform.right;
            case GizmoAxisHandle.Axis.Y: return gizmoTransform.up;
            default: return gizmoTransform.forward;
        }
    }

    /// <summary>
    /// Finds the closest point on the world-space line (origin + t*axisDir) to the camera ray.
    /// Standard trick for dragging along a 3D axis with a 2D mouse.
    /// </summary>
    bool ClosestPointOnAxis(Ray ray, Vector3 lineOrigin, Vector3 axisDir, out Vector3 result)
    {
        axisDir.Normalize();

        Vector3 cross = Vector3.Cross(axisDir, ray.direction);
        float denom = cross.sqrMagnitude;

        if (denom < 1e-6f)
        {
            result = lineOrigin;
            return false;
        }

        Vector3 diff = ray.origin - lineOrigin;
        float t = Vector3.Dot(Vector3.Cross(diff, ray.direction), cross) / denom;

        result = lineOrigin + axisDir * t;
        return true;
    }
}