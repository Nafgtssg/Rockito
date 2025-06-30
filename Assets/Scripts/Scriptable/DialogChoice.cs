using UnityEngine;

[CreateAssetMenu(fileName = "DialogChoice", menuName = "Geodisea/Dialog/Dialog Choice")]
public class DialogChoice : ScriptableObject {
    public string choiceText;
    public DialogNode nextNode;
}