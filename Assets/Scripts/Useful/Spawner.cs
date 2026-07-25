using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public SwitchMode mode;
    public GameObject player;
    public GameObject camera_Em;
    public GameObject[] objectsForSpawn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Spawn(int which)
    {
        if (!mode)
        {
            camera_Em = GameObject.FindWithTag("CameraEm");
            Instantiate(objectsForSpawn[which], 
                camera_Em.transform.position + new Vector3(0, 0, 3), 
                camera_Em.transform.rotation)
                .tag = "Selectable";
        }
        
    } 
    
}
