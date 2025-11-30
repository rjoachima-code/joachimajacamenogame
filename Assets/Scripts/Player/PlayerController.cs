using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6f;
    public float gravity = -9.81f;
    public Transform cameraTransform;
    public float jumpHeight = 1.2f;

    [Header("Jump Enhancement Settings")]
    [Tooltip("Duration in seconds after leaving a platform where the player can still jump.")]
    public float coyoteTime = 0.15f;
    [Tooltip("Duration in seconds before landing where jump input is remembered.")]
    public float jumpBufferTime = 0.2f;

    [Header("Score")]
    [Tooltip("Current collectible count/score.")]
    public int score = 0;

    private CharacterController controller;

    private Vector2 moveInput;
    private bool sprintInput;
    private bool jumpInput;
    private Vector3 velocity;

    // Coyote Time tracking
    private float coyoteTimeCounter;
    private bool wasGroundedLastFrame;

    // Input Buffer tracking
    private float jumpBufferCounter;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        MoveUpdate();
    }

    void MoveUpdate()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 desiredMove = forward * moveInput.y + right * moveInput.x;
        float speed = sprintInput ? sprintSpeed : walkSpeed;
        controller.Move(desiredMove * speed * Time.deltaTime);

        // Update Coyote Time
        if (controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update Jump Buffer
        if (jumpInput)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Check jump with Coyote Time and Input Buffering
        bool canJump = coyoteTimeCounter > 0f;
        bool hasBufferedJump = jumpBufferCounter > 0f;

        if (hasBufferedJump && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // Reset counters to prevent multiple jumps
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;

            // Play jump sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJumpSFX();
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Track grounded state for next frame
        wasGroundedLastFrame = controller.isGrounded;

        // Reset jump input flag
        jumpInput = false;
    }

    /// <summary>
    /// Adds to the player's score/collectible count.
    /// </summary>
    /// <param name="amount">Amount to add to the score.</param>
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"[PlayerController] Score: {score}");

        // Notify HUD if available
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateScore(score);
        }
    }

    /// <summary>
    /// Gets the current score.
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintInput = ctx.ReadValue<float>() > 0.5f;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started) jumpInput = true;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.Interact();
            }
        }
    }
}
