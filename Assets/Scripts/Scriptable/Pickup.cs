using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Recogible", menuName = "Geodisea/Interactuable/Recogible")]
public class Pickup : Interactable
{
    [Header("Configuraciones del Recogible")]
    [Tooltip("Tipo de inventario que este objeto debería utilizar cuando se recoge/reciba.")]
    public PickupType type;
    [Tooltip("Ícono que este objeto tiene en le inventario.")]
    public Sprite icon;
    [Tooltip("Descripción del objeto dentro del inventario.")]
    [TextArea] public string description;
    public override void Interact() => GameManager.manager.inventory.Add(this);
}

public enum PickupType
{
    item = 0,
    mineral = 1,
    gift = 2,
}