using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public InteractableData interactableData;
    public Vector3 interactionOffset = Vector3.zero;
    public Animator animator;

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent onPlayerEnterRange;
    public UnityEvent onPlayerExitRange;

    private bool isPlayerInRange = false;
    private Collider interactionCollider;

    void Awake()
    {
        interactionCollider = GetComponent<Collider>();
        interactionCollider.isTrigger = true;
    }

    void Update()
    {
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
        {
            Interact();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactableData != null)
        {
            isPlayerInRange = true;
            onPlayerEnterRange.Invoke();
            GameManager.manager.text.text = $"Pulsa E o Enter para {(interactableData.isPickup ? "recoger" : "interactuar")}";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            onPlayerExitRange.Invoke();
            GameManager.manager.text.text = "";
        }
    }

    public virtual void Interact()
    {
        onInteract.Invoke();
        Debug.Log($"Interacted with {interactableData.displayName}");

        // If consumable, handle destruction
        if (interactableData != null && interactableData.isPickup)
        {
            GameManager.manager.text.text = "";
            if (animator == null)
                Destroy(gameObject);
            else
                animator.SetTrigger("Destroy");
        }
    }
}