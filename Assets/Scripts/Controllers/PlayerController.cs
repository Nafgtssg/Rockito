using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    public Rigidbody rb;
    public Animator animator;
    [SerializeField] private bool isGrounded;
    private Vector3 movement;

    void Awake() {
        if (player != null && player != this) Destroy(gameObject);
        else
        {
            player = this;
            DontDestroyOnLoad(gameObject);
        }
        rb = GetComponent<Rigidbody>();
        animator= GetComponent<Animator>();
    }

    void Update() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (!GameManager.manager.inDialog && !GameManager.manager.inPopup && !GameManager.manager.isPlaying && !GameManager.manager.isBookOpen)
        {
            movement = CameraController.controller.transform.forward * vertical + CameraController.controller.transform.right * horizontal;
            if (Input.GetButtonDown("Jump") && isGrounded && !GameManager.manager.inDialog && !GameManager.manager.inPopup && !GameManager.manager.isPlaying && !GameManager.manager.isBookOpen)
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        Vector3 moveVelocity = movement.normalized * moveSpeed;
        if (!GameManager.manager.inDialog && !GameManager.manager.inPopup && !GameManager.manager.isPlaying && !GameManager.manager.isBookOpen)
        {
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
            if (rb.velocity.magnitude > 0.1)
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + 90, 0);
        }
        animator.SetFloat("speed", moveVelocity.magnitude);
    }
}