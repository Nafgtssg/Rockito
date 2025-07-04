using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Validator", menuName = "Geodisea/Validator/Combinator")]
public class Combinator : Validator
{
    [Header("Validator Properties")]
    public Validator validatorA;
    public Validator validatorB;
    public LogicOperator logicOperator;
    public override bool Validate()
    {
        if (validatorA != null && validatorB != null)
        {
            bool valor = logicOperator switch
            {
                LogicOperator.And => validatorA.Validate() && validatorB.Validate(),
                LogicOperator.Or => validatorA.Validate() || validatorB.Validate(),
                _ => false,
            };
            return negation ? !valor : valor;
        }
        else return false;
    }
}
