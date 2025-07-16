using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    [Tooltip("Validador para poder deshabilitar este efecto de ser necesario.")]
    public Validator validator;
    public virtual void Execute()
    {
        return;
    }
}
