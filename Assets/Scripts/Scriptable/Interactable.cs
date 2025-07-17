using UnityEngine;
[CreateAssetMenu(fileName = "Nuevo Interactuable Simple", menuName = "Geodisea/Interactuable/Interactuable Simple")]
public class Interactable : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName = "Object";
    public string action = "interactuar";
    public bool showDisplay = true;

    [Header("Audio")]
    public AudioClip interactionSound;

    [Header("Effects")]
    public Effect onInteract;
    public Effect onPlayerEnterRange;
    public Effect onPlayerExitRange;
    public virtual void Interact()
    {
        return;
    }
}