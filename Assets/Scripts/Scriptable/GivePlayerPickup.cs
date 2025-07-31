using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Dar Recogible al Jugador", menuName = "Geodisea/Efectos/Dar Recogible al Jugador")]
public class GivePlayerPickup : Effect
{
    [Header("Datos del Efecto")]
    [Tooltip("Recogible a dar al jugador.")]
    public Pickup pickup;
    public override void Execute() => pickup.Interact();
}
