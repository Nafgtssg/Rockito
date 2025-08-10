using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Validador de Objeto", menuName = "Geodisea/Validadores/Jugador Tiene Objecto")]
public class PlayerHasItem : Validator
{
    [Header("Propiedades del Validador")]
    [Tooltip("Tipo de inventario que este objeto se debe encontrar.")]
    public PickupType type;
    [Tooltip("Qué objeto el jugador debe poseer en el inventario.")]
    public Pickup item;
    public override bool Validate()
    {
        switch (type)
        {
            case PickupType.item:
                {
                    bool valor = GameManager.manager.inventory.Contains(item);
                    return negation ? !valor : valor;
                }
            case PickupType.key:
                {
                    bool valor = GameManager.manager.keyItems.Contains(item);
                    return negation ? !valor : valor;
                }
            case PickupType.rock:
                {
                    bool valor = GameManager.manager.rock.Contains(item);
                    return negation ? !valor : valor;
                }
            default: return false;
        }
    }
}
