using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Cambio de Escena", menuName = "Geodisea/Efectos/Cambiar Escena")]
public class ChangeScene : Effect
{
    [Header("Datos del Cambio de Escena")]
    [Tooltip("Nombre de la escena a la que se quiere cambiar.")]
    public string sceneName;
    [Tooltip("Posición que el jugador debería tomar cuando cambie de escena.")]
    public Vector3 playerNewPos;
    public Vector3 playerNewLocalScale = Vector3.one;
    public Vector3 playerNewRot;
    public Effect duringChange;
    public override void Execute()
    {
        if (validator != null)
        {
            if (validator.Validate()) GameManager.manager.TriggerSceneChange(this);
        }
        else GameManager.manager.TriggerSceneChange(this);
    }
}
