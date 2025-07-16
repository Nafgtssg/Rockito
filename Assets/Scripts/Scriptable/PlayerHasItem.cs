using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Validador de Objeto", menuName = "Geodisea/Validadores/Jugador Tiene Objecto")]
public class PlayerHasItem : Validator
{
    [Header("Propiedades del Validador")]
    [Tooltip("Qué objeto el jugador debe poseer en el inventario.")]
    public Pickup item;
    public override bool Validate()
    {
        bool valor = GameManager.manager.inventory.Contains(item);
        return negation ? !valor : valor;
    }
}
