using UnityEngine;

public class Inspector : MonoBehaviour
{
    GameObject obj_cube;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obj_cube = GameObject.Find("Cube(Clone)");
    }

}
