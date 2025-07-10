using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interactable Modifier", menuName = "Geodisea/Interactable/Camera Modifier")]
public class CameraModifier : Interactable
{
    [Header("Camera Settings")]
    [Range(-180.0f, 180.0f)] public float addRotation;
    public float addSize;
    public float addClipingPlane;
    public Vector3 tempOffset;
    public float tempRotation;
}
