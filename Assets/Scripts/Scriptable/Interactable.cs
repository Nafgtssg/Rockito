using UnityEngine;
using UnityEngine.Events;

public class Interactable : ScriptableObject
{
    [Header("Basic Info")]
    public InteractableType type;
    public string displayName = "Object";

    [Header("Audio")]
    public AudioClip interactionSound;

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent onPlayerEnterRange;
    public UnityEvent onPlayerExitRange;
}

public enum InteractableType {
    pickup = 0,
    dialog = 1,
}