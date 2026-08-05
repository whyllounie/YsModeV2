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
    
    void Update()
    {
        // 1. Безопасный поиск камеры
        if (Camera == null)
        {
            Camera = GameObject.FindWithTag("CameraEm");
        }

        // 2. Получаем выделенный объект
        SandboxObject selected = selectionManager != null ? selectionManager.GetSelectedObject() : null;

        // 3. Смена выделения
        if (selected != currentObject)
        {
            if (currentGizmo != null)
            {
                Destroy(currentGizmo);
            }

            currentObject = selected;

            if (currentObject != null)
            {
                // Проверяем Renderer безопасно!
                Renderer objectRenderer = currentObject.GetComponent<Renderer>();
                centralPosition = objectRenderer != null ? objectRenderer.bounds.center : currentObject.transform.position;

                currentGizmo = Instantiate(
                    gizmoPrefab,
                    centralPosition,
                    currentObject.transform.rotation
                );
            }
        }

        // 4. Логика следования и масштабирования (только если Гизмо существует!)
        if (currentGizmo != null && currentObject != null)
        {
            // Обновляем позицию
            Renderer objectRenderer = currentObject.GetComponent<Renderer>();
            Vector3 targetPos = objectRenderer != null ? objectRenderer.bounds.center : currentObject.transform.position;
            
            currentGizmo.transform.position = targetPos;
            currentGizmo.transform.rotation = currentObject.transform.rotation;

            // Масштаб от дистанции
            if (Camera != null)
            {
                float _distance = Vector3.Distance(Camera.transform.position, currentGizmo.transform.position);
                float calc_distance = _distance * gizmoScale;
                currentGizmo.transform.localScale = new Vector3(calc_distance, calc_distance, calc_distance);
            }
        }
    }
}