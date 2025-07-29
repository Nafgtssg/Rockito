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

    void Start()
    {
        interactionCollider = GetComponent<Collider>();
        interactionCollider.isTrigger = true;
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
        RegisterInteractable();
    }

    void RegisterInteractable()
    {
        // Los únicos interactuables que hay que registrar, pues su estado es importante
        // son los NPCs y los recolectables
        if (interactable is DialogTrigger || interactable is Pickup)
        {
            bool available = GameManager.manager.RegisterInteractable(internalId, gameObject, interactable);
            if (!available)
            {
                if (isPlayerInRange) OnTriggerExit(PlayerController.player.GetComponent<CapsuleCollider>());
                gameObject.SetActive(false);
            }
            GameManager.manager.LoadInteractableState(internalId, this);
        }
    }

    void Update() {
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            && !GameManager.manager.inDialog && !GameManager.manager.inPopup && !GameManager.manager.isBookOpen) {
            Interact();
            GameManager.manager.text.text = "";
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && interactable != null && !GameManager.manager.inDialog && !GameManager.manager.inPopup) {
            isPlayerInRange = true;
            if (interactable.onPlayerEnterRange != null) interactable.onPlayerEnterRange.Execute();
            if (interactable.showDisplay)
                GameManager.manager.text.text = $"Pulsa E o Enter para {interactable.action}";
        }
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

        Debug.Log($"Interacted with {interactable.displayName}");

        //canInteract = false;

        // If pickup, handle destruction
        if (interactable != null && interactable is Pickup)
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
    }
}