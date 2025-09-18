using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public ChangeScene returnToMainMenu;
    public void ExitCredits()
    {
        returnToMainMenu.Execute();
        GameManager.manager.creditsAnimator.gameObject.SetActive(false);
        GameManager.manager.blackout.SetActive(false);
        PlayerController.player.canMove = false;
        PlayerController.player.flashlight.SetActive(false);
        PlayerController.player.topLight.SetActive(false);
    }
}
