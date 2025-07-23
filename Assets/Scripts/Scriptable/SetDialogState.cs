using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Establecer Estado a NPC", menuName = "Geodisea/Efectos/Establecer Estado a NPC")]
public class SetNPCState : Effect
{
    [Header("Datos del Efecto")]
    [Tooltip("Id (presente en el activador de diálogo) del NPC al que se le cambiará el estado.")]
    public string id;
    [Tooltip("Nuevo estado para este NPC, recordando que el número de estado, activa el diálogo con el que abre el NPC, siendo 0, el primer diálogo disponible en el arreglo.")]
    [Min(0)] public int state = 0;
    public override void Execute() => GameManager.manager.SetDialogState(id, state);
}
