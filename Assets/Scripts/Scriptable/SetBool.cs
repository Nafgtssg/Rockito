using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Establecer Booleano de Estado", menuName = "Geodisea/Efectos/Establecer Booleano de Estado")]
public class SetBool : Effect
{
    public StateBool state;
    public bool value = true;
    public override void Execute()
    {
        if (validator != null)
        {
            if (validator.Validate())
            {
                DoTheThing();
            }
        }
        else
        {
            DoTheThing();
        }
    }
    public void DoTheThing()
    {
        switch (state)
        {
            case StateBool.Flashlight:
                GameManager.manager.canUseFlashlight = value;
                GameManager.manager.flashlightHint.SetActive(value);
                break;
            case StateBool.Credits:
                GameManager.manager.TriggerCredits();
                break;
            case StateBool.Blackout:
                GameManager.manager.blackout.SetActive(value);
                break;
        }
    }
}

public enum StateBool
{
    Flashlight,
    Credits,
    Blackout
}