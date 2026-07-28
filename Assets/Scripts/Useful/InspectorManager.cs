using System;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using static System.Text.StringBuilder;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public SelectionManager selectionManager;
    private SandboxObject _hitObject;
    public Vector3 _pos;
    
    [SerializeField] private GameObject pannel; //включает инспектор
    [SerializeField] private List<GameObject> pannelImages; //хранит все картинки
    [SerializeField] private Dictionary<string, GameObject> ImagesPannel = new Dictionary<string, GameObject>(); // берет имя и картинку
    
    [SerializeField] private TMP_Text text_x;
    [SerializeField] private TMP_Text text_y;
    [SerializeField] private TMP_Text text_z;

    private void Awake()
    {
        foreach (var data in pannelImages)
        {
            ImagesPannel.Add(data.name, data); 
        }
    }
    
    private void Start()
    {
        pannel.SetActive(false);
        
        for (int i = 0; i < pannelImages.Count; i++)
            pannelImages[i].SetActive(false);
        
        Debug.LogError(String.Join(",", ImagesPannel.Keys)); // Проверка, список создался или нет
    }

    private void Update()
    {
        _hitObject = selectionManager.GetSelectedObject();
        
        if (_hitObject != null)
            _pos = _hitObject.GetPosition();

        if (_hitObject != null)
        {
            if (_hitObject.isSelected){
                pannel.SetActive(true);
                
                text_x.text = String.Format("{0:F}", _pos.x);
                text_y.text = String.Format("{0:F}", _pos.y);
                text_z.text = String.Format("{0:F}", _pos.z);

                Inspect(_hitObject, pannelImages);
                
            }
            
            else
            {
                pannel.SetActive(false);
            }
        }
        
        else
        {
            pannel.SetActive(false);
        }
    }
    
    private void Inspect(SandboxObject obj, List<GameObject> list)
    {
        
        foreach (GameObject image in list)
        {
            image.SetActive(false);
        }
        
        if (obj.GetName() == "Cube(Clone)")
        {
                    
            GameObject thisObj = ImagesPannel["ImageOfCube"];
            thisObj.SetActive(true);
        }
                
        if (obj.GetName() == "Sphere(Clone)")
        {
            GameObject thisObj = ImagesPannel["ImageOfSphere"];
            thisObj.SetActive(true);
        }

        if (obj.GetName() == "Cylinder(Clone)")
        {
            GameObject thisObj = ImagesPannel["ImageOfCylinder"];
            thisObj.SetActive(true);
        }
    }
}
