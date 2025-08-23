using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Juego de Unir Conceptos", menuName = "Geodisea/Efectos/Juego Unir Conceptos")]
public class ConceptData : Effect
{
    [Header("Datos del Juego de Conceptos")]
    [Tooltip("Conceptos los cuales pueden ser tomados y movidos.")]
    public ConceptPair[] concepts;
    [Tooltip("ID y nombre de cajas donde se depositan los conceptos. Las Id colocadas aquí deben coincidir con las Id de los conceptos.")]
    public string[] boxIDs; // IDs of all available boxes
    [Tooltip("Tamaño de los conceptos, es un tamaño regular que todos estos poseerán.")]
    public Vector2 size = new Vector2(300, 100);
    [Tooltip("Si es que al generarse el juego, los conceptos se colocan en posiciones aleatorias. Si no, se colocan en la posición en la que se definieron.")]
    public bool randomizeConcepts = true;
    [Tooltip("Si es que al generarse el juego, las cajas se colocan en posiciones aleatorias. Si no, se colocan en la posición en la que se definieron.")]
    public bool randomizeBoxes = true;
    [Tooltip("Efecto que se activa una vez temina el juego. Ideal para continuar con diálogo, o activar algún evento.")]
    public Effect onEnding;
    [Tooltip("Efecto que sobreescribe el efecto de fin de juego y que sólo se activa si se tiene puntuación perfecta en el juego. Ideal para continuar con diálogo, o activar algún evento.")]
    public Effect onCorrect;
    public override void Execute()
    {
        if (validator != null) {
            if (validator) GameManager.manager.StartConceptGame(this);
        }
        else GameManager.manager.StartConceptGame(this);
    }
}

[System.Serializable]
public class ConceptPair
{
    public string conceptID;
    public Sprite conceptImage;
    public string conceptText;
    public string description;
    public string correctBoxID;
}