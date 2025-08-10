using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Retraso", menuName = "Geodisea/Efectos/Retraso")]
public class Delay : Effect
{
    [Header("Datos del Retraso")]
    public float delayInSeconds;
    public Effect effect;
    public override void Execute()
    {
        if (validator != null) {
            if (validator.Validat4e()) GameManager.manager.TriggerEffectDelay(delayInSeconds, effect);
        }
        else GameManager.manager.TriggerEffectDelay(delayInSeconds, effect);
    }
}
