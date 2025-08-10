using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public void ChangeScene() => GameManager.manager.PassSceneChange();
    public void EndChangeScene() => GameManager.manager.EndSceneChange();
    public void StartCinematicMode() => GameManager.manager.StartCinematicMode();
    public void EndCinematicMode() => GameManager.manager.EndCinematicMode();
    public void DeactivateSceneTransition() => gameObject.SetActive(false);
    public void CinematicFlash() => GameManager.manager.sceneChangeAnimator.SetTrigger("flash");
}
