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
    public override void Interact()
    {
        switch (type)
        {
            case PickupType.item: GameManager.manager.inventory.Add(this); break;
            case PickupType.key: GameManager.manager.keyItems.Add(this); break;
            case PickupType.rock: GameManager.manager.rock.Add(this); break;
        }
    }
}

public enum PickupType
{
    item = 0,
    key = 1,
    rock = 2,
}