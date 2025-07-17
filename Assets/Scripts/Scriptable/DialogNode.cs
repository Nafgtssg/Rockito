using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Nodo de Diálogo", menuName = "Geodisea/Efectos/Nodo de Diálogo")]
public class DialogNode : Effect
{
    [Header("Información del Orador")]
    [Tooltip("Nombre de quién está hablando.")]
    public string displayName;
    [Tooltip("Sprite que aparece del lado izquierdo del diálogo. Es opcional.")]
    public Sprite leftSpeaker;
    [Tooltip("Sprite que aparece del lado derecho del diálogo. Es opcional.")]
    public Sprite rightSpeaker;
    [Tooltip("Sonido (idealmente muy corto) que se reproduce cada vez que aparece una letra de diálogo, para emular diálogo de Undertale o Animal Crossing.")]
    public AudioClip sound;
    [Tooltip("Variación del pitch del sonido elegido para el diálogo.")]
    public float pitchVariation = 0;
    [Header("Contenido del Dialogo")]
    [TextArea(0, 300)] public string dialogText;
    [Tooltip("El diálogo que viene tras este diálogo.\nEn caso de no haber, siguiente nodo, se acaba la conversación.\n\nSi hay elecciones de diálogo, se ignora este siguiente logo.")]
    public DialogNode nextNode;
    [Tooltip("Efecto que se ejecuta cuando empieza el diálogo.")]
    public Effect effect;
    [Header("Opciones de Elección")]
    [Tooltip("Nodos de diálogo que aparecen como botones en pantalla, para bifurcar la conversación.")]
    public DialogNode[] choices;
    [Tooltip("Texto que aparece cuando este nodo actúe como elección.")]
    public string choiceText = "Opción";
    public override void Execute() => GameManager.manager.StartDialog(this);
}
