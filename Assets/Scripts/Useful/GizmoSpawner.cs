using UnityEngine;

public class GizmoSpawner : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private GameObject gizmoPrefab;

    [SerializeField] private GameObject Camera;
    [SerializeField] private float gizmoScale = 0.15f;
    
    private SandboxObject currentObject;
    private GameObject currentGizmo;

    private Vector3 centralPosition;
    
    private bool isGizmoIn = false;
    void Update()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();
        Camera = GameObject.FindWithTag("CameraEm");
        
        Renderer objectRenderer = selected.GetComponent<Renderer>();
        centralPosition = objectRenderer.bounds.center;
        
        // Selection changed
        if (selected != currentObject)
        {
            // Remove old gizmo
            if (currentGizmo != null)
                Destroy(currentGizmo); isGizmoIn = false;

            currentObject = selected;

            // Create new gizmo
            if (currentObject != null)
            {
                currentGizmo = Instantiate(
                    gizmoPrefab,
                    centralPosition,
                    currentObject.transform.rotation
                );
                
                isGizmoIn = true;
            }
            
        }

        //scale
        if (isGizmoIn)
        {
            float _distance = Vector3.Distance(Camera.transform.position, currentObject.transform.position);

            float calc_distance = _distance * gizmoScale;
            currentGizmo.transform.localScale = new Vector3(calc_distance, calc_distance, calc_distance);
        }
        
        // Keep gizmo following the object
        if (currentGizmo != null && currentObject != null)
        {
            currentGizmo.transform.position = currentObject.GetPosition();
            currentGizmo.transform.rotation = currentObject.transform.rotation;
        }
    }
}