using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Interactable interactable;
    public Animator animator;
    private bool isPlayerInRange = false;
    private Collider interactionCollider;

    void Awake() {
        interactionCollider = GetComponent<Collider>();
        interactionCollider.isTrigger = true;
    }

    void Update() {
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && !GameManager.manager.inDialog)
            Interact();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && interactable != null) {
            isPlayerInRange = true;
            interactable.onPlayerEnterRange.Invoke();
            GameManager.manager.text.text = $"Pulsa E o Enter para {interactable.action}";
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            interactable.onPlayerExitRange.Invoke();
            GameManager.manager.text.text = "";
        }
    }

    public virtual void Interact() {
        interactable.onInteract.Invoke();
        Debug.Log($"Interacted with {interactable.displayName}");

        // If pickup, handle destruction
        if (interactable != null && interactable.type == InteractableType.pickup) {
            Pickup pickup = (Pickup)interactable;
            GameManager.manager.text.text = "";
            GameManager.manager.inventory.Add(pickup);
            if (animator == null)
                Destroy(gameObject);
            else {
                animator.gameObject.transform.SetParent(null, true);
                animator.SetTrigger("Destroy");
                Destroy(gameObject);
            }
        }
    }
}