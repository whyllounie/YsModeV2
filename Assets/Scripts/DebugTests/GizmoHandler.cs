using UnityEngine;

public class GizmoHandler : MonoBehaviour
{
    public GameObject gizmo;
    public Vector3 pos;
    public Quaternion rot;
    public SwitchMode mode;
    
    public bool showGizmo;
    [SerializeField] private Renderer gizmoRender;
    [SerializeField] private Color gizmoColorSelected;
    [SerializeField] private Color gizmoColorUnselected;
    [SerializeField] private SelectionManager selectionManager;
    
    public void Start()
    {
        mode = GameObject.FindWithTag("modeManager").GetComponent<SwitchMode>();

        if (!mode.GetMode())
        {
            if (selectionManager != null)
            {
                pos = selectionManager.GetCentralPosition();
                rot = selectionManager.GetRotation();
                
                gizmoRender = gizmo.GetComponent<Renderer>();
                gizmoColorSelected = new Color(1f, 0.84f, 0f, 1f);
                gizmoColorUnselected = gizmoRender.material.color;
            }
        }
    }
    
    public void Update()
    {
        if (showGizmo)
        {
            gizmoRender.material.color = gizmoColorSelected;
        }

        else
        {
            gizmoRender.material.color = gizmoColorUnselected;
        }
    }

    public void SetSelected(bool selected)
    {
        showGizmo = selected;
        
        if (showGizmo)
            Debug.Log("Selected: " + selected);
        else
            Debug.Log("Unselected: " + selected);
    }
}
