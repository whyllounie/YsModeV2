using UnityEngine;

public class GizmoAxisHandle_my : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Axis Configuration")]
    [SerializeField] private Axis handleAxis;

    [Header("Target & References")]
    [SerializeField] private SelectionManager selectionManager;

    private Camera mainCam;
    private Plane dragPlane;
    private Vector3 planeNormal;
    private Vector3 initialTargetPos;
    private Vector3 initialHitPoint;

    private void Start()
    {
        mainCam = Camera.main;
        if (selectionManager == null)
        {
            selectionManager = FindFirstObjectByType<SelectionManager>();
        }
    }

    private void OnMouseDown()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();
        if (selected == null || mainCam == null) return;

        initialTargetPos = selected.GetPosition();

        // 1. Choose a plane normal perpendicular to the chosen axis for smooth dragging
        Vector3 axisDir = GetAxisVector();
        
        // Pick a cross vector based on camera view direction to keep the drag plane facing the camera
        Vector3 camDir = mainCam.transform.forward;
        planeNormal = Vector3.Cross(axisDir, Vector3.Cross(camDir, axisDir)).normalized;

        // 2. Create an invisible drag plane passing through the object's current position
        dragPlane = new Plane(planeNormal, initialTargetPos);

        // 3. Find where the mouse ray initially hits this plane
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            initialHitPoint = ray.GetPoint(enter);
        }
    }

    private void OnMouseDrag()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();
        if (selected == null || mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        // Calculate current mouse hit on the drag plane
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 currentHitPoint = ray.GetPoint(enter);
            Vector3 offset = currentHitPoint - initialHitPoint;

            // Project the offset onto the selected axis direction ONLY
            Vector3 axisDir = GetAxisVector();
            float dragDistance = Vector3.Dot(offset, axisDir);

            // Update target object position along the single axis
            Vector3 newPosition = initialTargetPos + (axisDir * dragDistance);
            
            // Move object
            selected.transform.position = newPosition;
        }
    }

    private Vector3 GetAxisVector()
    {
        switch (handleAxis)
        {
            case Axis.X: return transform.right;   // Red arrow axis
            case Axis.Y: return transform.up;      // Green arrow axis
            case Axis.Z: return transform.forward; // Blue arrow axis
            default: return Vector3.right;
        }
    }
}