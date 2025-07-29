using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nueva Receta", menuName = "Geodisea/Receta")]
public class Recipe : ScriptableObject
{
    [Tooltip("Primer reactante que se utilizará en el crafteo.")]
    public Reactant first;
    [Tooltip("Segundo reactante que se utilizará en el crafteo.")]
    public Reactant second;
    [Tooltip("Objeto resultante del crafteo..")]
    public Pickup result;
}

[System.Serializable]
public class Reactant
{
    public Pickup pickup;
    public int amount;
}