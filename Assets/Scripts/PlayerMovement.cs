using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching = false;
    private PlayerControls controls; // Your auto-generated Input Manager
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;

    void Awake()
    {
        controls = new PlayerControls();

        // Movement Input
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Jump Input (Instant Response)
        controls.Player.Jump.performed += ctx => jumpPressed = true;

        // Sprint & Crouch Input
        controls.Player.Sprint.performed += ctx => walkSpeed = sprintSpeed;
        controls.Player.Sprint.canceled += ctx => walkSpeed = 5f;
        controls.Player.Crouch.performed += ctx => isCrouching = !isCrouching;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        // Raycast to check if player is on the ground
        isGrounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    private void HandleMovement()
    {
        float speed = isCrouching ? crouchSpeed : walkSpeed;
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        controller.Move(move * speed * Time.fixedDeltaTime);
        controller.height = isCrouching ? 1f : 2f;
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false; // Reset jump press immediately
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keeps player grounded properly
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }

        controller.Move(velocity * Time.fixedDeltaTime);
    }
}
