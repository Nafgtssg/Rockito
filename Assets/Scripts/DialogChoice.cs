using UnityEngine;

[CreateAssetMenu(fileName = "DialogChoice", menuName = "Game/Dialog Choice")]
public class DialogChoice : ScriptableObject {
    public string choiceText;
    public DialogNode nextNode;
}