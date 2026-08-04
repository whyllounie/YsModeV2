using UnityEngine;

public class GizmoSpawner : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private GameObject gizmoPrefab;

    [SerializeField] private GameObject Camera;
    [SerializeField] private float gizmoScale = 0.15f;

    private SandboxObject currentObject;
    private GameObject currentGizmo;

    private bool isGizmoIn = false;

    // Public getters so other scripts (like GizmoMover) can read state without duplicating logic
    public GameObject CurrentGizmo => currentGizmo;
    public SandboxObject CurrentObject => currentObject;

    void Start()
    {
        // Find the camera once instead of every frame
        if (Camera == null)
            Camera = GameObject.FindWithTag("CameraEm");
    }

    void Update()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();

        // FIX: bail out early if nothing is selected, instead of crashing on GetComponent<Renderer>()
        if (selected == null)
        {
            if (currentGizmo != null)
            {
                Destroy(currentGizmo);
                isGizmoIn = false;
            }
            currentObject = null;
            return;
        }

        // Selection changed
        if (selected != currentObject)
        {
            // Remove old gizmo
            if (currentGizmo != null)
            {
                Destroy(currentGizmo);
                isGizmoIn = false;
            }

            currentObject = selected;

            // Create new gizmo
            Renderer objectRenderer = currentObject.GetComponent<Renderer>();
            Vector3 spawnPos = objectRenderer != null
                ? objectRenderer.bounds.center
                : currentObject.transform.position;

            currentGizmo = Instantiate(
                gizmoPrefab,
                spawnPos,
                currentObject.transform.rotation
            );

            isGizmoIn = true;
        }

        // Scale gizmo based on camera distance
        if (isGizmoIn && Camera != null)
        {
            float distance = Vector3.Distance(Camera.transform.position, currentObject.transform.position);
            float calcDistance = distance * gizmoScale;
            currentGizmo.transform.localScale = new Vector3(calcDistance, calcDistance, calcDistance);
        }

        // Keep gizmo following the object
        if (currentGizmo != null && currentObject != null)
        {
            currentGizmo.transform.position = currentObject.GetPosition();
            currentGizmo.transform.rotation = currentObject.transform.rotation;
        }
    }
}
