using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseMover))]
public class PlayerMovementInput : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private BaseMover mover;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isCrouching;

    void Start()
    {
        mover = GetComponent<CharacterControllerMover>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        mover.SetJumpInput(context.ReadValueAsButton());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Toggle crouching only when the button is fully pressed (performed)
        if (context.performed)
        {
            isCrouching = !isCrouching;
            mover.SetCrouchInput(isCrouching);
        }
    }

    private void Update()
    {
        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);
            mover.SetLookDirection(cameraForward);
        }
        else
        {
            moveDirection.Set(moveInput.x, 0, moveInput.y);
            mover.SetLookDirection(transform.forward);
        }

        mover.Move(moveDirection);
    }
}