using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractableData", menuName = "Geodisea/Interactable/Dialog")]
public class Dialog : Interactable
{
    [Header("Dialog Settings")]
    public DialogNode dialog;
    public void Display() {
        GameManager.manager.StartDialog(dialog);
    }
}
