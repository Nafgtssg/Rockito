using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interactable Pickup", menuName = "Geodisea/Interactable/Pickup")]
public class Pickup : Interactable
{
    [Header("Pickup Settings")]
    public Sprite icon;
    [TextArea] public string description;
    public int maxInteractions = 1; // 0 for infinite
}
