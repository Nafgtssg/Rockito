using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Interactable interactable;
    public Animator animator;
    private bool isPlayerInRange = false;
    private Collider interactionCollider;
    private bool canInteract = true;

    void Awake() {
        interactionCollider = GetComponent<Collider>();
        interactionCollider.isTrigger = true;
    }

    void Update() {
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && !GameManager.manager.inDialog && canInteract)
        {
            Interact();
            GameManager.manager.text.text = "";
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && interactable != null && !GameManager.manager.inDialog) {
            isPlayerInRange = true;
            if (interactable.onPlayerEnterRange != null) interactable.onPlayerEnterRange.Execute();
            if (interactable is not CameraModifier)
                GameManager.manager.text.text = $"Pulsa E o Enter para {interactable.action}";
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            GameManager.manager.text.text = "";
            if (interactable.onPlayerExitRange != null) interactable.onPlayerExitRange.Execute();
            canInteract = true;
        }
    }

    public void Interact() {
        if (interactable.onInteract != null) interactable.onInteract.Execute();
        interactable.Interact();

        Debug.Log($"Interacted with {interactable.displayName}");

        canInteract = false;

        // If pickup, handle destruction
        if (interactable != null && interactable is Pickup)
        {
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