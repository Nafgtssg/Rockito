using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractableData", menuName = "Game/Interactable Data")]
public class InteractableData : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName = "Object";
    public Sprite icon;

    [Header("Interaction Settings")]
    public bool isPickup = false;

    [Header("Audio")]
    public AudioClip interactionSound;

    [Header("Advanced Settings")]
    [TextArea] public string description;
    public int maxInteractions = 1; // 0 for infinite
}