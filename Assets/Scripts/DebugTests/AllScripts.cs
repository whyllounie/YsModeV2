// using System;
// using UnityEngine;
// using TMPro;
//
// public class SandboxObject1 : MonoBehaviour
// {
//     public GameObject obj;
//     public Rigidbody rb;
//     public Vector3 pos;
//     public Quaternion rot;
//     public SwitchMode mode;
//     // public Vector3 scale;
//
//     public void Start()
//     {
//         pos = obj.transform.position;
//         rot = obj.transform.rotation;
//         rb = obj.GetComponent<Rigidbody>();
//         rb.useGravity = false;
//     }
//
//     public void Update()
//     {
//         if (mode.GetMode())
//             rb.useGravity = true;
//         
//         else
//         {
//             rb.useGravity = false;
//             obj.transform.position = pos;
//             obj.transform.rotation = rot;
//
//         }
//     }
//     
//     
//     
//     
//     public SelectionManager selectionManager;
//     private SandboxObject _hitObject;
//     public Vector3 _pos;
//     
//     [SerializeField] private GameObject pannel;
//     [SerializeField] private TMP_Text text_x;
//     [SerializeField] private TMP_Text text_y;
//     [SerializeField] private TMP_Text text_z;
//     
//     private void Start1()
//     {
//         pannel.SetActive(false);
//         
//     }
//
//     private void Update1()
//     {
//         _hitObject = selectionManager.GetSelectedObject();
//         _pos = _hitObject.GetPosition();
//         if (_hitObject != null)
//         {
//             if (_hitObject.isSelected){
//                 pannel.SetActive(true); 
//                 string posX = String.Format("{0:F", _pos.x);
//                 text_x.text = posX;
//                 text_y.text = String.Format("{0:F", _pos.y);
//                 text_z.text = Convert.ToString(_pos.z); // сделать округление до 2х чисел после запятой
//             }
//             else
//             {
//                 pannel.SetActive(false);
//             }
//         }
//         else
//         {
//             pannel.SetActive(false);
//         }
//     }
// }
//
