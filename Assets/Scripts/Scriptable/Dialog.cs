using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Interactable", menuName = "Geodisea/Interactable/Dialog")]
public class Dialog : Interactable
{
    [Header("Dialog Settings")]
    public string id;
    public DialogNode[] dialog;
    public void Display() {
        int currentState = GameManager.manager.GetDialogState(id);

        if (currentState >= dialog.Length)
        {
            currentState = 0;
            Debug.LogError($"{id} tried to load a dialog non existent from a state.");
        }
        
        GameManager.manager.StartDialog(dialog[currentState]);
    }
}
