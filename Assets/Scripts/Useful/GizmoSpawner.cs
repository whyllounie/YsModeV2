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
        // 1. Находим камеру, если поле пустое
        if (Camera == null)
        {
            Camera = GameObject.FindWithTag("CameraEm");
        }

        // 2. Безопасно получаем выбранный объект
        SandboxObject selected = selectionManager != null ? selectionManager.GetSelectedObject() : null;

        // 3. Логика смены выделения
        if (selected != currentObject)
        {
            // Удаляем старый гизмо
            if (currentGizmo != null)
            {
                Destroy(currentGizmo);
                isGizmoIn = false;
            }
            
            currentObject = selected;
            
            // Создаем новый гизмо ТОЛЬКО если объект реально выбран
            if (currentObject != null)
            {
                // Считаем центр БЕЗОПАСНО
                Renderer objectRenderer = currentObject.GetComponent<Renderer>();
                centralPosition = (objectRenderer != null) ? objectRenderer.bounds.center : currentObject.transform.position;

                currentGizmo = Instantiate(
                    gizmoPrefab,
                    centralPosition,
                    currentObject.transform.rotation
                );
                
                isGizmoIn = true;
            }
        }
        
        // 4. Масштабирование (проверяем, что всё существует)
        if (isGizmoIn && currentGizmo != null && currentObject != null && Camera != null)
        {
            float _distance = Vector3.Distance(Camera.transform.position, currentObject.transform.position);
            float calc_distance = _distance * gizmoScale;
            currentGizmo.transform.localScale = new Vector3(calc_distance, calc_distance, calc_distance);
        }

        // 5. Следование гизмо за объектом
        if (currentGizmo != null && currentObject != null)
        {
            Renderer objectRenderer = currentObject.GetComponent<Renderer>();
            Vector3 targetPos = (objectRenderer != null) ? objectRenderer.bounds.center : currentObject.GetPosition();

            currentGizmo.transform.position = targetPos;
            currentGizmo.transform.rotation = currentObject.transform.rotation;
        }
    }

    Vector3 GetCentralPosition()
    {
        return centralPosition;
    }
}