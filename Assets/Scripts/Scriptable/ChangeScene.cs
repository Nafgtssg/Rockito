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
    public override void Execute() => GameManager.manager.TriggerSceneChange(this);
}
