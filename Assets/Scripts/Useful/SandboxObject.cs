using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SandboxObject : MonoBehaviour
{
    public GameObject obj;
    public Rigidbody rb;
    public Vector3 pos;
    public Quaternion rot;
    public SwitchMode mode;
    // public Vector3 scale;
    
    [SerializeField] public bool isSelected;
    [SerializeField] private Renderer objRender;
    [SerializeField] private Color objColorSelected;
    [SerializeField] private Color objColorUnselected;
    
    public void Start()
    {
        pos = obj.transform.position;
        rot = obj.transform.rotation;
        rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        objRender = obj.GetComponent<Renderer>();
        objColorSelected = new Color(145f / 255f, 5 / 255f, 5 / 255f);
        objColorUnselected = objRender.material.color;
    }

    public void Update()
    {
        if (mode.GetMode())
            rb.useGravity = true;
        
        else
        {
            rb.useGravity = false;
            obj.transform.position = pos;
            obj.transform.rotation = rot;
        }

        if (isSelected)
        {
            Debug.Log("[SBO] Object selected");
            objRender.material.color = objColorSelected; //has to be an outline
        }
        else if (!isSelected)
        {
            Debug.Log("[SBO] Object unselected");
            objRender.material.color = objColorUnselected; //too
        }
    }

    public void SetSelected(bool selectStatus)
    {
        isSelected = selectStatus;
        
        if (isSelected)
            Debug.Log("Object selected");
        else
            Debug.Log("Object undelected");
    }

    public Vector3 GetPosition()
    {
        return pos;
    }

    public String GetName()
    {
        return obj.name;
    }
}

