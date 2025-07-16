using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Activador de Diálogo", menuName = "Geodisea/Interactuable/Activador de Diálogo")]
public class DialogTrigger : Interactable
{
    [Header("Configuraciones del Diálogo")]
    [Tooltip("Id de este activador de diálogo, es necesario para que el estado actual de la conversación se guarde correctamente.")]
    public string id;
    [Tooltip("Nodos de diálogos que se activan dependiendo del estado relacionado a la ID de este activador. Por defecto, el estado es 0, lo que corresponde al primer nodo del arreglo.")]
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
