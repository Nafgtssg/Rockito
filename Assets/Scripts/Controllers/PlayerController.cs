using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;

    [Header("Movement Settings")]
    public bool canMove = true;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    public float rotationSpeed = 10f;
    
    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    
    [SerializeField] private bool isGrounded;
    private Vector3 movement;
    private float targetRotation;
    [Header("Flashlight Settings")]
    public GameObject flashlight;
    public GameObject topLight;
    public bool flashlightOn = false;

    void Awake() {
        if (player != null && player != this) Destroy(gameObject);
        else
        {
            player = this;
            DontDestroyOnLoad(gameObject);
        }
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (GameManager.manager.CanMove() && canMove)
        {
            // Calculate movement direction relative to camera rotation
            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
            
            if (inputDirection.magnitude >= 0.1f)
            {
                // Get camera's Y rotation only (ignore tilt)
                float cameraAngle = CameraController.controller.rotation;
                
                // Rotate input direction by camera angle
                movement = Quaternion.Euler(0f, cameraAngle + 180, 0f) * inputDirection;
                
                // Calculate target rotation for player
                targetRotation = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            }
            else
            {
                movement = Vector3.zero;
            }

            // Jump input
            if (Input.GetButtonDown("Jump") && isGrounded)
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (Input.GetKeyDown(KeyCode.F) && GameManager.manager.canUseFlashlight) {
                flashlightOn = !flashlightOn;
                flashlight.SetActive(flashlightOn);
                topLight.SetActive(flashlightOn);
            }
        } else {
            movement = Vector3.zero;
        }
    }

    void FixedUpdate() {
        if (GameManager.manager.CanMove() && canMove)
        {
            // Apply movement
            Vector3 moveVelocity = movement * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

            // Smoothly rotate player to face movement direction
            if (movement.magnitude > 0.1f)
            {
                Quaternion targetQuaternion = Quaternion.Euler(0f, targetRotation + 90, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else rb.velocity = new Vector3(0, rb.velocity.y, 0);
        
        // Update animator
        animator.SetFloat("speed", movement.magnitude);
    }
}