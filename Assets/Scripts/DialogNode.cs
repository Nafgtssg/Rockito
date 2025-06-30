using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Game/Dialog Node")]
public class DialogNode : ScriptableObject {
    [Header("Speaker Info")]
    public string displayName;
    public Sprite portrait;

    [Header("Dialog Content")]
    [TextArea(0, 300)] public string dialogText;
    public DialogNode nextNode;
    
    [Header("Dialog Options")]
    public bool hasChoices;
    public DialogChoice[] choices;
}
