using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Node", menuName = "Geodisea/Dialog/Dialog Node")]
public class DialogNode : Effect {
    [Header("Speaker Info")]
    public string displayName;
    public Sprite leftSpeaker;
    public Sprite rightSpeaker;
    public AudioClip sound;
    public float pitchVariation = 0;

    [Header("Dialog Content")]
    [TextArea(0, 300)] public string dialogText;
    public DialogNode nextNode;
    public Effect effect;
    
    [Header("Dialog Options")]
    public bool hasChoices;
    public DialogChoice[] choices;
}
