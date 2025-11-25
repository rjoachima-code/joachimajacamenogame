using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6f;
    public float gravity = -9.81f;
    public Transform cameraTransform;
    public float jumpHeight = 1.2f;
    private CharacterController controller;

    private Vector2 moveInput;
    private bool sprintInput;
    private bool jumpInput;
    private Vector3 velocity;

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

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (jumpInput && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // reset jump
        jumpInput = false;
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

