using UnityEngine;

/// <summary>
/// Put this on each of the 3 arrow objects INSIDE your gizmoPrefab
/// (the X arrow, the Y arrow, the Z arrow).
/// Set the "Axis" field to match which axis that arrow represents.
/// Make sure each arrow also has a Collider (Box/Capsule/Mesh) so it can be clicked.
/// </summary>
public class GizmoAxisHandle : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis axis;
}
