using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Establecer Estado a Interactuable", menuName = "Geodisea/Efectos/Establecer Estado a Interactuable")]
public class SetInteractableState : Effect
{
    [Header("Datos del Efecto")]
    [Tooltip("Nuevo estado para este interactuable.")]
    public InteractableRecord state;
    public override void Execute() => GameManager.manager.SetInteractableState(state);
}
