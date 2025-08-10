using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Modificador de Cinemática", menuName = "Geodisea/Efectos/Modificador de Cinemática")]
public class CinematicModifier : Effect
{
    [Header("Datos de la cinemática")]
    public CinematicModifierType type;
    public Vector3 camPos;
    public Vector3 camRot;
    public float camFOV = 80f;
    public override void Execute()
    {
        if (validator != null)
        {
            if (validator.Validate())
            {
                switch (type)
                {
                    case CinematicModifierType.end:
                        GameManager.manager.FinishCinematicMode();
                        break;
                    case CinematicModifierType.fade:
                        GameManager.manager.TriggerCinematicMode(this);
                        break;
                    case CinematicModifierType.instant:
                        GameManager.manager.isCamera = true;
                        PlayerController.player.rb.useGravity = false;
                        GameManager.manager.sceneChangeAnimator.gameObject.SetActive(true);
                        GameManager.manager.sceneChangeAnimator.SetTrigger("instantCinema");
                        GameManager.manager.sceneChangeAnimator.SetBool("finishCinematic", false);
                        CCController.controller.transform.position = camPos;
                        CCController.controller.transform.rotation = Quaternion.Euler(camRot);
                        CCController.controller.cinematic.fieldOfView = camFOV;
                        break;
                    case CinematicModifierType.flash:
                        GameManager.manager.sceneChangeAnimator.SetTrigger("flash");
                        break;
                }
            }
        }
        else
        {
            switch (type)
            {
                case CinematicModifierType.end:
                    GameManager.manager.FinishCinematicMode();
                    break;
                case CinematicModifierType.fade:
                    GameManager.manager.TriggerCinematicMode(this);
                    break;
                case CinematicModifierType.instant:
                    GameManager.manager.isCamera = true;
                    PlayerController.player.rb.useGravity = false;
                    GameManager.manager.sceneChangeAnimator.gameObject.SetActive(true);
                    GameManager.manager.sceneChangeAnimator.SetTrigger("instantCinema");
                    GameManager.manager.sceneChangeAnimator.SetBool("finishCinematic", false);
                    CCController.controller.transform.position = camPos;
                    CCController.controller.transform.rotation = Quaternion.Euler(camRot);
                    CCController.controller.cinematic.fieldOfView = camFOV;
                    break;
                case CinematicModifierType.flash:
                    GameManager.manager.sceneChangeAnimator.SetTrigger("flash");
                    break;
            }
        }
    }
}

public enum CinematicModifierType
{
    instant = 0,
    fade,
    flash,
    end
}