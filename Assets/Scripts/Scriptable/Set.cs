using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Conjunto", menuName = "Geodisea/Efectos/Conjunto")]
public class Set : Effect
{
    [Header("Datos del Efecto")]
    [Tooltip("Conjunto de efectos que este efecto activará en secuencia cuando se ejecute.")]
    public Effect[] effects;
    public override void Execute()
    {
        foreach (Effect effect in effects) {
            effect.Execute();
        }
    }
}