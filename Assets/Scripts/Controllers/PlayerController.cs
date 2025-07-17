using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    [SerializeField] private bool isGrounded;
    private Vector3 movement;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (!GameManager.manager.inDialog && !GameManager.manager.inPopup)
        {
            movement = CameraController.controller.transform.forward * vertical + CameraController.controller.transform.right * horizontal;
            if (Input.GetButtonDown("Jump") && isGrounded && !GameManager.manager.inDialog && !GameManager.manager.isPlaying)
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        if (!GameManager.manager.inDialog && !GameManager.manager.inPopup && !GameManager.manager.isPlaying)
        {
            Vector3 moveVelocity = movement.normalized * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }
    }
}