using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Modificador de Cámara", menuName = "Geodisea/Efectos/Modificador de Cámara")]
public class CameraModifier : Effect
{
    [Header("Configuración de Cámara")]
    [Range(-180.0f, 180.0f)] public float addRotation;
    public float addSize;
    public float addClipingPlane;
    //public Vector3 tempOffset;
    //public float tempRotation;
}
