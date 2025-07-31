using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Popup", menuName = "Geodisea/Efectos/Popup")]
public class Popup : Effect
{
    [Header("Datos del Popup")]
    [Tooltip("Tipo de popup.")]
    public PopupType type;
    [Tooltip("Si es que el popup tiene título superior. Esto es opcional.")]
    public string title;
    [Tooltip("Si es que el popup tiene descripción inferior. Esto es opcional.")]
    [TextArea(0, 300)] public string description;
    [Tooltip("Sprite para mostrar en el popup.")]
    public Sprite sprite;
    [Tooltip("Tamaño del sprite a mostrar.")]
    public Vector2 size;
    [Tooltip("Efecto que se debe activar luego de mostrar el popup.")]
    public Effect onEnding;
    public override void Execute() => GameManager.manager.TriggerPopup(this);
}

public enum PopupType
{
    none = 0,
    fade = 1,
    bounce = 2
}