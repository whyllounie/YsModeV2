using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSelect : MonoBehaviour
{
    // [SerializeField] private GameObject[] gizmoPrefab;
    [SerializeField] private Camera _camera;
    //
    // private Dictionary<String, GameObject> _gizmo;

    // private void Start()
    // {
    //     if (gizmoPrefab != null && gizmoPrefab.Length >= 3)
    //     {
    //         _gizmo = new Dictionary<string, GameObject>()
    //         {
    //             {"X", gizmoPrefab[0]},
    //             {"Y", gizmoPrefab[1]},
    //             {"Z", gizmoPrefab[2]}
    //         };
    //     }
    // }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (_camera == null) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Gizmo"))
                    Debug.Log("Gizmo is hit!");

                if (hit.collider.CompareTag("X"))
                    Debug.Log("X is hit!");

                if (hit.collider.CompareTag("Y"))
                    Debug.Log("Y is hit!");

                if (hit.collider.CompareTag("Z"))
                    Debug.Log("Z is hit!");
            }
        }
    }
}