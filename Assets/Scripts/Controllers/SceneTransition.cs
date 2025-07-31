using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public void ChangeScene() => GameManager.manager.PassSceneChange();
    public void EndChangeScene() => GameManager.manager.EndSceneChange();
}
