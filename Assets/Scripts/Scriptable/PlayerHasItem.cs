using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Validator", menuName = "Geodisea/Validator/Player Has Item")]
public class PlayerHasItem : Validator
{
    [Header("Validator Properties")]
    public Pickup item;
    public override bool Validate()
    {
        bool valor = GameManager.manager.inventory.Contains(item);
        return negation ? !valor : valor;
    }
}
