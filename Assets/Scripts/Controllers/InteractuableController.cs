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
        {
            Interact();   
            GameManager.manager.text.text = "";
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && interactable != null && !GameManager.manager.inDialog) {
            isPlayerInRange = true;
            interactable.onPlayerEnterRange.Invoke();
            if (interactable.type != InteractableType.camera)
                GameManager.manager.text.text = $"Pulsa E o Enter para {interactable.action}";
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            GameManager.manager.text.text = "";
            interactable.onPlayerExitRange.Invoke();
        }
    }

    public virtual void Interact() {
        interactable.onInteract.Invoke();
        Debug.Log($"Interacted with {interactable.displayName}");

        // If pickup, handle destruction
        if (interactable != null && interactable.type == InteractableType.pickup) {
            Pickup pickup = (Pickup)interactable;
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