using UnityEngine;

public class MenuFolder : MonoBehaviour
{
    public GameObject subButtonContainer;

    public void ToggleMenu()
    {
        bool isCurrentActive = subButtonContainer.activeSelf;
        subButtonContainer.SetActive(!isCurrentActive);
    } 
}
