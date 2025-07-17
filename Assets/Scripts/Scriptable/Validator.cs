using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Validador Básico", menuName = "Geodisea/Validadores/Validador")]
public class Validator : ScriptableObject
{
    public bool negation;
    public virtual bool Validate()
    {
        return !negation;
    }
}

public enum LogicOperator
{
    And = 0,
    Or = 1,
}