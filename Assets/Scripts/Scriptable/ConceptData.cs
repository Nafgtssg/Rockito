using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Juego de Unir Conceptos", menuName = "Geodisea/Efectos/Juego Unir Conceptos")]
public class ConceptData : Effect
{
    [Header("Datos del Juego de Conceptos")]
    public ConceptPair[] concepts;
    public string[] boxIDs; // IDs of all available boxes
    public int columns = 1;
    public float horizontalOffset = 316f;
    public override void Execute() => GameManager.manager.StartConceptGame(this);
}

[System.Serializable]
public class ConceptPair
{
    public string conceptID;
    public Sprite conceptImage;
    public string conceptText;
    public string correctBoxID;
}