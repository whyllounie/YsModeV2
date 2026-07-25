using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController controller;
    public float speed = 10.5f;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        
        Vector3 vh = new Vector3(h, 0, v);
        
        controller.Move(vh * speed * Time.deltaTime);
    }
}
