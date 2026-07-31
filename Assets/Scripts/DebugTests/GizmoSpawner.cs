using UnityEngine;

public class GizmoSpawner : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private GameObject gizmoPrefab;

    private SandboxObject currentObject;
    private GameObject currentGizmo;

    void Update()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();

        // Selection changed
        if (selected != currentObject)
        {
            // Remove old gizmo
            if (currentGizmo != null)
                Destroy(currentGizmo);

            currentObject = selected;

            // Create new gizmo
            if (currentObject != null)
            {
                currentGizmo = Instantiate(
                    gizmoPrefab,
                    currentObject.GetPosition(),
                    Quaternion.identity
                );
            }
        }

        // Keep gizmo following the object
        if (currentGizmo != null && currentObject != null)
        {
            currentGizmo.transform.position = currentObject.GetPosition();
        }
    }
}