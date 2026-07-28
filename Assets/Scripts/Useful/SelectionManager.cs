using System;
using UnityEngine;
using Object = System.Object;

public class SelectionManager : MonoBehaviour
{
    public Camera cam;

    private SandboxObject currentSelectedObject;

    public SandboxObject hitObject;
    
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Selectable"))
                {
                    hitObject = hit.collider.GetComponent<SandboxObject>();

                    if (!hitObject.isSelected)
                    {
                        
                        if (hitObject != null)
                        {
                            DeselectCurrent();
                            
                            currentSelectedObject = hitObject;
                            
                            currentSelectedObject.SetSelected(true);
                        }
                    }

                    else
                    {
                        DeselectCurrent();
                    }
                }
                
                else
                {
                    DeselectCurrent();
                }
            }

            else
            {
                DeselectCurrent();
            }
        }
    }

    private void DeselectCurrent()
    {
        if (currentSelectedObject != null)
        {
            currentSelectedObject.SetSelected(false);
            
            currentSelectedObject = null;
        }
    }

    public SandboxObject GetSelectedObject()
    {
        return hitObject;
    }
    
    public Vector3 GetPosition()
    {
        return hitObject.transform.position;
    }
    
}
