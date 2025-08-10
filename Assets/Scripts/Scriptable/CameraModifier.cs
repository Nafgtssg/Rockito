using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Modificador de Cámara", menuName = "Geodisea/Efectos/Modificador de Cámara")]
public class CameraModifier : Effect
{
    [Header("Configuración de Cámara")]
    public bool set = false;
    public bool orthographic = true;
    public bool followTarget = true;
    public float addRotation;
    public float addTilt;
    public float addSize;
    public float addClipingPlane;
    public Vector3 addOffset;
    public Vector3 setPosition;
    public Vector3 setRotation;
    public override void Execute()
    {
        if (validator != null)
        {
            if (validator.Validate())
            {
                if (set) CameraController.controller.SetCameraData(this);
                else CameraController.controller.ModifyCameraData(this);
            }
        }
        else
        {
            if (set) CameraController.controller.SetCameraData(this);
            else CameraController.controller.ModifyCameraData(this);
        }
    }
}
