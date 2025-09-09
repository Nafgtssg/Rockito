using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Establecer Booleano de Estado", menuName = "Geodisea/Efectos/Establecer Booleano de Estado")]
public class SetBool : Effect
{
    public StateBool state;
    public override void Execute()
    {
        if (validator != null)
        {
            if (validator.Validate())
            {
                PlayerController.player.canMove = false;
            }
        }
        else
        {
            PlayerController.player.canMove = false;
        }
    }
}

public enum StateBool
{
    
}