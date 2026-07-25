using UnityEngine;
using UnityEngine.UI;

public class SwitchMode : MonoBehaviour
{
    public GameObject player_PM;
    public GameObject camera_EM;
    public GameObject canvas;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_PM = GameObject.FindWithTag("Player"); 
        camera_EM = GameObject.FindWithTag("CameraEm");    
        canvas = GameObject.FindWithTag("InteractableUI");
        player_PM.SetActive(false);
    }

    public void Switch()
    {
        bool active = player_PM.activeSelf;
        
        player_PM.SetActive(!active);
        camera_EM.SetActive(active);
        canvas.SetActive(active);
        
    }
    
    public bool GetMode()
    {
        return player_PM.activeSelf;
    }
}
