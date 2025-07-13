using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Node", menuName = "Geodisea/Dialog/Dialog Node")]
public class DialogNode : Effect
{
    [Header("Información del Orador")]
    public string displayName;
    public Sprite leftSpeaker;
    public Sprite rightSpeaker;
    public AudioClip sound;
    public float pitchVariation = 0;

    [Header("Contenido del Dialogo")]
    [TextArea(0, 300)] public string dialogText;
    [Tooltip("El dialogo que viene tras este dialogo.\nEn caso de no haber, siguiente nodo, se acaba la conversación.\n\nSi hay elecciones de dialogo, se ignora este siguiente logo.")]
    public DialogNode nextNode;
    [Tooltip("Efecto que se ejecuta cuando empieza el dialogo.")]
    public Effect effect;
    [Header("Opciones de Elección")]
    [Tooltip("Nodos de dialogo que aparecen como botones en pantalla, para bifurcar la conversación.")]
    public DialogNode[] choices;
    [Tooltip("Texto que aparece cuando este nodo actúe como elección.")]
    public string choiceText = "Opción";
    [Tooltip("Validador para deshabilitar esta opción de ser necesario.")]
    public Validator validator;
}
