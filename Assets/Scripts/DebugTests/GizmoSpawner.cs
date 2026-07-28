using System;
using UnityEngine;

public class GizmoSpawner : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private GameObject gizmoPrefab;

    private SandboxObject obj;
    private GameObject gizmo;

    private void Update()
    {
        SandboxObject selected = selectionManager.GetSelectedObject();

        if (selected != obj)
        {
            if (gizmo != null)
                Destroy(gizmo);
            
            obj = selected;

            if (obj != null)
            {
                gizmo = Instantiate(gizmoPrefab, obj.GetPosition(), Quaternion.identity);
            }
        }
        
        if (gizmo != null && obj != null)
        {
            gizmo.transform.position = obj.GetPosition();
        }
    }
}
