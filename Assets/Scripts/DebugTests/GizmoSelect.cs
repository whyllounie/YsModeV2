using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSelect : MonoBehaviour
{
    public Camera cam;

    private GizmoHandler currentGizmo;

    private GizmoHandler hitArrow;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("X"))
                {
                    Debug.Log("X");
                }
                else if (hit.collider.CompareTag("Y"))
                {
                    Debug.Log("Y");
                }
                else if (hit.collider.CompareTag("Z"))
                {
                    Debug.Log("Z");
                }

                else
                {
                    DeselectCurrent();
                }
            }
        }
    }

    private void DeselectCurrent()
    {
        if (currentGizmo != null)
        {
            currentGizmo.SetSelected(false);
        }
    }
}