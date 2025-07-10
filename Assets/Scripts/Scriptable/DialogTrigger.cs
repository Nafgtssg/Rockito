using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Trigger", menuName = "Geodisea/Interactable/Dialog Trigger")]
public class DialogTrigger : Interactable
{
    [Header("Dialog Settings")]
    public string id;
    public DialogNode[] dialog;
    public override void Interact() {
        int currentState = GameManager.manager.GetDialogState(id);

        if (currentState >= dialog.Length)
        {
            currentState = 0;
            Debug.LogError($"{id} tried to load a dialog non existent from a state.");
        }
        
        GameManager.manager.StartDialog(dialog[currentState]);
    }
}
