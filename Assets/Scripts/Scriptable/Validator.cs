using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Validator", menuName = "Geodisea/Validator/Validator")]
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