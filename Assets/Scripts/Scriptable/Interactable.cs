using UnityEngine;

public class Interactable : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName = "Object";
    public string action = "interactuar";

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