using UnityEngine;

/// <summary>
/// Runtime "move" gizmo — draws three colored arrows (X/Y/Z) attached to a target object.
/// Click-drag an arrow to move the target along that axis, like the editor's Move tool.
///
/// SETUP:
/// 1. Create an empty GameObject in your scene called "MoveGizmo".
/// 2. Add this script to it.
/// 3. Assign "Target" to the object you want to move (or leave empty and call SetTarget() at runtime).
/// 4. Make sure your scene has a Camera tagged "MainCamera" (or assign camera manually).
/// 5. Requires Physics raycasting, so the generated arrow handles have colliders — this works fine
///    alongside your existing objects since the handles are on a dedicated layer (auto-created).
///
/// URP NOTE: Uses Unlit/Color-style materials generated at runtime via Shader.Find("Universal Render Pipeline/Unlit"),
/// with a fallback to "Sprites/Default" / "Unlit/Color" for non-URP projects, so it won't come out pink.
/// </summary>
[ExecuteAlways]
public class RuntimeMoveGizmo : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The object this gizmo will move. Can be set at runtime via SetTarget().")]
    public Transform target;

    [Header("Appearance")]
    public float axisLength = 1.5f;
    public float axisThickness = 0.06f;
    public float arrowHeadSize = 0.18f;
    [Tooltip("Gizmo scales with distance from camera so it stays a usable on-screen size.")]
    public bool constantScreenSize = true;
    public float screenSizeMultiplier = 0.15f;

    [Header("Colors")]
    public Color xColor = new Color(0.85f, 0.2f, 0.2f);
    public Color yColor = new Color(0.2f, 0.85f, 0.2f);
    public Color zColor = new Color(0.2f, 0.45f, 0.95f);
    public Color hoverTint = Color.yellow;

    [Header("Behaviour")]
    public Camera cam;
    [Tooltip("Layer used for the invisible drag-handle colliders. Auto-created if it doesn't exist.")]
    public string handleLayerName = "MoveGizmoHandle";
    public LayerMask blockingLayers = ~0; // layers that can occlude / receive normal clicks; used to avoid stealing clicks when nothing is hit

    // internal
    private Transform xHandle, yHandle, zHandle;
    private MeshRenderer xRend, yRend, zRend;
    private Transform draggedAxis;
    private Vector3 dragAxisWorld;
    private Vector3 dragStartTargetPos;
    private Vector3 dragStartHitPoint;
    private int handleLayer = -1;

    private SandboxObject currentObject;
    private SelectionManager _selectionManager;
    
    void OnEnable()
    {
        if (cam == null) cam = Camera.main;
        EnsureHandleLayer();
        BuildHandles();
    }

    void OnDisable()
    {
        // Clean up generated handles when disabled/destroyed
        if (Application.isPlaying)
        {
            if (xHandle) Destroy(xHandle.gameObject);
            if (yHandle) Destroy(yHandle.gameObject);
            if (zHandle) Destroy(zHandle.gameObject);
        }
    }

    void EnsureHandleLayer()
    {
        handleLayer = LayerMask.NameToLayer(handleLayerName);
        if (handleLayer == -1)
        {
            Debug.LogWarning($"[RuntimeMoveGizmo] Layer '{handleLayerName}' not found. " +
                              $"Create it in Project Settings > Tags and Layers for clean raycasting. " +
                              $"Falling back to Default layer.");
            handleLayer = 0;
        }
    }

    Material MakeUnlitMaterial(Color c)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material mat = new Material(shader);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
        // Make sure it renders on top-ish and isn't affected by fog/lighting
        mat.renderQueue = 4000;
        return mat;
    }

    void BuildHandles()
    {
        if (xHandle != null) return; // already built

        xHandle = CreateArrow("GizmoAxis_X", xColor, Vector3.right);
        yHandle = CreateArrow("GizmoAxis_Y", yColor, Vector3.up);
        zHandle = CreateArrow("GizmoAxis_Z", zColor, Vector3.forward);

        xRend = xHandle.GetComponentInChildren<MeshRenderer>();
        yRend = yHandle.GetComponentInChildren<MeshRenderer>();
        zRend = zHandle.GetComponentInChildren<MeshRenderer>();
    }

    Transform CreateArrow(string name, Color color, Vector3 dir)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(transform, false);
        root.layer = handleLayer;

        // Shaft (cylinder)
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.name = "Shaft";
        Destroy(shaft.GetComponent<Collider>()); // replace with a thin capsule collider sized to the arrow
        shaft.transform.SetParent(root.transform, false);
        shaft.transform.localScale = new Vector3(axisThickness, axisLength * 0.5f, axisThickness);
        shaft.transform.localPosition = dir * (axisLength * 0.5f);
        shaft.transform.up = dir;
        shaft.GetComponent<MeshRenderer>().sharedMaterial = MakeUnlitMaterial(color);
        shaft.layer = handleLayer;

        // Arrowhead (cone-ish via scaled cube... Unity has no built-in cone, use a scaled sphere for simplicity/robustness)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        Destroy(head.GetComponent<Collider>());
        head.transform.SetParent(root.transform, false);
        head.transform.localScale = Vector3.one * arrowHeadSize;
        head.transform.localPosition = dir * axisLength;
        head.GetComponent<MeshRenderer>().sharedMaterial = MakeUnlitMaterial(color);
        head.layer = handleLayer;

        // Single capsule collider covering the whole arrow for easy clicking/dragging
        CapsuleCollider col = root.AddComponent<CapsuleCollider>();
        col.direction = dir == Vector3.right ? 0 : (dir == Vector3.up ? 1 : 2);
        col.radius = Mathf.Max(axisThickness, arrowHeadSize) * 1.2f;
        col.height = axisLength + arrowHeadSize;
        col.center = dir * (axisLength * 0.5f);

        return root.transform;
    }

    void Update()
    {
        currentObject = _selectionManager.GetSelectedObject();
        target = currentObject.transform;
        
        if (!Application.isPlaying) return;
        if (target == null || cam == null) return;

        // Keep gizmo positioned on target
        transform.position = target.position;

        if (constantScreenSize)
        {
            float dist = Vector3.Distance(cam.transform.position, transform.position);
            float scale = dist * screenSizeMultiplier;
            transform.localScale = Vector3.one * scale;
        }

        HandleHoverAndDrag();
    }

    void HandleHoverAndDrag()
    {
        // Reset hover tint each frame, re-apply below if hovered
        SetTint(xRend, xColor);
        SetTint(yRend, yColor);
        SetTint(zRend, zColor);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (draggedAxis == null)
        {
            // Hover detection (only handle layer)
            if (Physics.Raycast(ray, out RaycastHit hoverHit, 1000f, 1 << handleLayer))
            {
                Transform hitRoot = hoverHit.collider.transform;
                TintForAxis(hitRoot, hoverTint);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, 1 << handleLayer))
                {
                    BeginDrag(hit.collider.transform, ray);
                }
            }
        }
        else
        {
            TintForAxis(draggedAxis, hoverTint);

            if (Input.GetMouseButton(0))
            {
                ContinueDrag(ray);
            }
            if (Input.GetMouseButtonUp(0))
            {
                draggedAxis = null;
            }
        }
    }

    void TintForAxis(Transform axisRoot, Color c)
    {
        if (axisRoot == xHandle) SetTint(xRend, c);
        else if (axisRoot == yHandle) SetTint(yRend, c);
        else if (axisRoot == zHandle) SetTint(zRend, c);
    }

    void SetTint(MeshRenderer r, Color c)
    {
        if (r == null) return;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
        if (r.sharedMaterial.HasProperty("_BaseColor")) block.SetColor("_BaseColor", c);
        if (r.sharedMaterial.HasProperty("_Color")) block.SetColor("_Color", c);
        r.SetPropertyBlock(block);

        // also tint the head sibling
        var head = r.transform.parent.Find("Head")?.GetComponent<MeshRenderer>();
        if (head != null)
        {
            head.SetPropertyBlock(block);
        }
    }

    void BeginDrag(Transform axisRoot, Ray ray)
    {
        draggedAxis = axisRoot;
        dragAxisWorld = GetAxisDirectionWorld(axisRoot);
        dragStartTargetPos = target.position;

        if (ClosestPointOnAxisFromRay(ray, target.position, dragAxisWorld, out Vector3 hitPoint))
        {
            dragStartHitPoint = hitPoint;
        }
    }

    void ContinueDrag(Ray ray)
    {
        if (ClosestPointOnAxisFromRay(ray, dragStartTargetPos, dragAxisWorld, out Vector3 currentPoint))
        {
            Vector3 delta = currentPoint - dragStartHitPoint;
            target.position = dragStartTargetPos + delta;
        }
    }

    Vector3 GetAxisDirectionWorld(Transform axisRoot)
    {
        if (axisRoot == xHandle) return transform.right;
        if (axisRoot == yHandle) return transform.up;
        if (axisRoot == zHandle) return transform.forward;
        return Vector3.right;
    }

    /// <summary>
    /// Finds the point on the world-space line (origin + t*axisDir) that is closest to the given camera ray.
    /// This is the standard technique for dragging along a 3D axis with a 2D mouse.
    /// </summary>
    bool ClosestPointOnAxisFromRay(Ray ray, Vector3 lineOrigin, Vector3 axisDir, out Vector3 result)
    {
        axisDir.Normalize();

        Vector3 rayOrigin = ray.origin;
        Vector3 rayDir = ray.direction;

        Vector3 crossAxisRay = Vector3.Cross(axisDir, rayDir);
        float denom = crossAxisRay.sqrMagnitude;

        if (denom < 1e-6f)
        {
            // Ray is parallel to axis; can't solve reliably
            result = lineOrigin;
            return false;
        }

        Vector3 diff = rayOrigin - lineOrigin;
        Vector3 crossDiffRay = Vector3.Cross(diff, rayDir);
        float t = Vector3.Dot(crossDiffRay, crossAxisRay) / denom;

        result = lineOrigin + axisDir * t;
        return true;
    }

    /// <summary>Call this to attach the gizmo to a new object at runtime.</summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null) transform.position = target.position;
    }
}
