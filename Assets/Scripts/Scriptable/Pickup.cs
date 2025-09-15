using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Recogible", menuName = "Geodisea/Interactuable/Recogible")]
public class Pickup : Interactable
{
    [Header("Configuraciones del Recogible")]
    [Tooltip("Tipo de inventario que este objeto debería utilizar cuando se recoge/reciba.")]
    public PickupType type;
    [Tooltip("Ícono que este objeto tiene en le inventario.")]
    public Sprite icon;
    [Tooltip("Descripción del objeto dentro del inventario.")]
    [TextArea] public string description;
    [Tooltip("Efecto que se activa cuando este objeto es fabricado.")]
    public Effect onCraft;
    public override void Interact()
    {
        GameManager.manager.recievedAnimator.SetTrigger("appear");
        GameManager.manager.recievedImage.sprite = icon;
        GameManager.manager.recievedText.text = $"Conseguido {displayName}";
        switch (type)
        {
            case PickupType.item:
                var item1 = GameManager.manager.inventory.Find(x => x.displayName == displayName);
                if (item1 == null) GameManager.manager.inventory.Add(this);
                GameManager.manager.recievedText.text += "\nAñadido a Inventario";
                break;
            case PickupType.key:
                var item2 = GameManager.manager.keyItems.Find(x => x.displayName == displayName);
                if (item2 == null) GameManager.manager.keyItems.Add(this);
                GameManager.manager.recievedText.text += "\nAñadido a Obj. Llave";
                break;
            case PickupType.rock:
                var item3 = GameManager.manager.rock.Find(x => x.displayName == displayName);
                if (item3 == null) GameManager.manager.rock.Add(this);
                GameManager.manager.recievedText.text += "\nAñadido a Minerales";
                break;
        }
    }
}

public enum PickupType
{
    item = 0,
    key = 1,
    rock = 2,
}