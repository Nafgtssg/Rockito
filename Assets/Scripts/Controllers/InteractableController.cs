using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour
{
    public int internalId;
    [Header("Interaction Settings")]
    public Interactable interactable;
    public Animator animator;
    private bool isPlayerInRange = false;
    private Collider interactionCollider;
    //private bool canInteract = true;
    private bool loaded = false;
    void Start()
    {
        interactionCollider = GetComponent<Collider>();
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        RegisterInteractable();
    }

    void RegisterInteractable()
    {
        bool available = GameManager.manager.RegisterInteractable(internalId, gameObject, interactable);
        if (!available)
        {
            if (isPlayerInRange) OnTriggerExit(PlayerController.player.GetComponent<BoxCollider>());
            gameObject.SetActive(false);
        }
        GameManager.manager.LoadInteractableState(internalId, this);
        loaded = true;
    }

    void Update() {
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
                    && !GameManager.manager.inDialog
                    && !GameManager.manager.inPopup
                    && !GameManager.manager.isPlaying
                    && !GameManager.manager.isBookOpen
                    && !GameManager.manager.isCamera) {
            Interact();
            GameManager.manager.text.text = "";
        }
    }

    void OnTriggerEnter(Collider other) {
        StartCoroutine(DelayEnterUntilLoaded(other));
    }
    IEnumerator DelayEnterUntilLoaded(Collider other)
    {
        for ( ; !loaded ; ) yield return new WaitForSeconds(.1f);
        if (other.CompareTag("Player") && interactable != null && !GameManager.manager.inDialog && !GameManager.manager.inPopup)
        {
            isPlayerInRange = true;
            if (interactable.onPlayerEnterRange != null) interactable.onPlayerEnterRange.Execute();
            if (interactable.showDisplay)
                GameManager.manager.text.text = $"Pulsa E o Enter para {interactable.action.ToLower()}";
        }
        yield return new WaitForSeconds(0);
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            GameManager.manager.text.text = "";
            if (interactable.onPlayerExitRange != null) interactable.onPlayerExitRange.Execute();
            //canInteract = true;
        }
    }

    public void Interact() {
        if (interactable.onInteract != null) interactable.onInteract.Execute();
        interactable.Interact();
        if (interactable.interactionSound != null)
            GameManager.manager.audioSource.PlayOneShot(interactable.interactionSound);

        //canInteract = false;

        // If pickup, handle destruction
        if (interactable != null)
        {
            if (interactable is Pickup)
            {
                GameManager.manager.ToggleInteractableState(internalId);
                if (animator == null)
                    Destroy(gameObject);
                else
                {
                    animator.gameObject.transform.SetParent(null, true);
                    animator.SetTrigger("Destroy");
                    Destroy(gameObject);
                }
            }
            /*
            else if (interactable is DialogTrigger)
            {
                transform.LookAt(PlayerController.player.transform.position, Vector3.up);
                transform.Rotate(0f, 180f, 0f, Space.Self);
            }
            */
        }
    }
}