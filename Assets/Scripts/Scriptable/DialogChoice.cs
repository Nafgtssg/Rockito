using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Choice", menuName = "Geodisea/Dialog/Dialog Choice")]
public class DialogChoice : ScriptableObject
{
    public string choiceText;
    public DialogNode nextNode;
    public Validator validator;
}