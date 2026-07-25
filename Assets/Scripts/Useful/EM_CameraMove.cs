using UnityEngine;

public class EM_CameraMove : MonoBehaviour
{
    [Header("Old")]
    GameObject camera_EM;
    [SerializeField] private Vector3 forward;
    [SerializeField] private Vector3 upward;
    [SerializeField] private Vector3 downward;
    [SerializeField] private Vector3 backward;
    [SerializeField] private Vector3 right;
    [SerializeField] private Vector3 left;
    public float speed;

    [Header("Настройки мыши")]
    public float sensitivity = 2f; // Скорость чувствительности мыши
    private float rotationX = 0f;  // Поворот вверх/вниз
    private float rotationY = 0f;  // Поворот влево/вправо
    
    void Start()
    {
        camera_EM = GameObject.FindWithTag("CameraEm");
        speed = 10; 
        
        forward = new Vector3 (0, 0, 1);
        upward = new Vector3 (0, 1, 0);
        downward = new Vector3 (0, -1, 0);
        backward = new Vector3 (0, 0, -1);
        right = new Vector3 (1, 0, 0);
        left = new Vector3 (-1, 0, 0);
    }

    void Update()
    {
        // === 1. ПРОВЕРКА НАЖАТИЯ ALT ===
        // Если зажат левый или правый Alt
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            Cursor.lockState = CursorLockMode.None; // Освобождаем мышку
            Cursor.visible = true;                  // Показываем её
            
            return; // Стоп! Дальше код в этом кадре не выполняется. Камера замирает.
        }
        else
        {
            // Если Alt НЕ зажат — прячем мышку, чтобы она не мешала летать
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // === 2. ПОВОРОТ КАМЕРЫ МЫШКОЙ ===
        // Получаем простые числа: насколько сдвинулась мышь в этом кадре
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationY += mouseX; // Крутим влево-вправо
        rotationX -= mouseY; // Крутим вверх-вниз
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Чтобы голова не оторвалась назад

        // Поворачиваем камеру в пространстве
        camera_EM.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);


        // === 3. ТВОЁ ДВИЖЕНИЕ (ПОЛНОСТЬЮ ТВОЙ КОД) ===
        // Но с одним изменением: мы берем направления ОТНОСИТЕЛЬНО КАМЕРЫ
        if (Input.GetKey(KeyCode.W))
        {
            // camera_EM.transform.forward — это "вперед" туда, куда смотрит камера
            camera_EM.transform.position += camera_EM.transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            // Назад — это противоположность переднего направления камеры (минус transform.forward)
            camera_EM.transform.position += -camera_EM.transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            // Вправо относительно камеры
            camera_EM.transform.position += camera_EM.transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            // Влево относительно камеры
            camera_EM.transform.position += -camera_EM.transform.right * speed * Time.deltaTime;
        }

        // Q и E оставляем строго как у тебя — они двигают мир чисто вверх и вниз
        if (Input.GetKey(KeyCode.Q))
        {
            camera_EM.transform.position += downward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            camera_EM.transform.position += upward * speed * Time.deltaTime;
        }
    }
}