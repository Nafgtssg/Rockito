using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Set NPC State Effect", menuName = "Geodisea/Effect/Set NPC State")]
public class SetNPCState : Effect
{
    public string id;
    public int state;
    public override void Execute() => GameManager.manager.SetDialogState(id, state);
}
