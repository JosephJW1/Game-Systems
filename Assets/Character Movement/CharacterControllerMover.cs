using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : BaseMover
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;
    public float acceleration = 10f;

    [Header("Physics")]
    public float gravity = -20f;
    public float jumpSpeed = 8f;

    [Header("Moving Platforms")]
    public LayerMask groundLayer = ~0; // Default to everything
    public float platformRayDistance = 0.5f; // Extra length for raycast
    public bool rotateWithPlatform = true;

    [Header("Jump Buffering")]
    public float jumpBufferTime = 0.2f;

    [Header("Charged Leap")]
    public float maxLeapForce = 20f;
    public float maxChargeTime = 1.5f;
    public float leapUpwardBias = 0.5f;
    [Range(0f, 1f)] public float chargeMoveMultiplier = 0.3f;

    [Header("Events")]
    public UnityEvent<float> OnChargeProgress;
    public UnityEvent OnChargeStopped;
    public UnityEvent OnLeapPerformed;

    private CharacterController controller;

    // Movement State
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection = Vector3.forward;
    private Vector3 currentHorizontalVelocity = Vector3.zero;
    private float verticalVelocity = 0f;
    private float jumpBufferTimer;

    // Input State
    private bool isCrouching;
    private bool isJumpHeld;
    private float jumpChargeTimer;
    private bool isCharging;

    // Platform State
    private Transform activePlatform;
    private Vector3 activeLocalPoint;
    private Vector3 activeGlobalPoint;
    private Quaternion activeGlobalRotation;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        lookDirection = transform.forward;
    }

    public override void Move(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public override void SetLookDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            lookDirection = direction.normalized;
        }
    }

    private void Jump()
    {
        if (!isCrouching)
        {
            if (controller.isGrounded)
            {
                verticalVelocity = jumpSpeed;
                // Important: Clear platform when jumping so we don't snap back to it
                activePlatform = null;
            }
            else
            {
                jumpBufferTimer = jumpBufferTime;
            }
        }
    }

    public override void SetJumpInput(bool held)
    {
        if (isCrouching)
        {
            if (isJumpHeld && !held)
            {
                PerformLeap();
            }
        }
        else if (held && !isJumpHeld)
        {
            Jump();
        }

        isJumpHeld = held;
    }

    public override void SetCrouchInput(bool crouching)
    {
        isCrouching = crouching;
        if (!isCrouching)
        {
            ResetCharge();
        }
    }

    void Update()
    {
        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // --- CHARGE LOGIC ---
        if (isCrouching && isJumpHeld)
        {
            jumpChargeTimer += Time.deltaTime;
            isCharging = true;
            float ratio = Mathf.Clamp01(jumpChargeTimer / maxChargeTime);
            OnChargeProgress?.Invoke(ratio);
        }
        else if (isCharging)
        {
            ResetCharge();
        }
        // --------------------

        HandleGravity();
        HandleHorizontalMovement();
        HandleRotation();

        // 1. Calculate Standard Movement
        Vector3 finalMove = currentHorizontalVelocity + (Vector3.up * verticalVelocity);
        Vector3 frameMove = finalMove * Time.deltaTime;

        // 2. Calculate Platform Movement (The new addition)
        Vector3 platformMove = Vector3.zero;

        // Raycast specifically to find moving platforms
        // We do this before moving so we can apply the platform's motion from the PREVIOUS frame to NOW
        if (controller.isGrounded)
        {
            // Raycast slightly below feet
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, (controller.height / 2f) + platformRayDistance, groundLayer))
            {
                // If we are on the same platform as last frame
                if (activePlatform == hit.transform)
                {
                    // Where is the point we were standing on previously, RIGHT NOW?
                    Vector3 newGlobalPoint = activePlatform.TransformPoint(activeLocalPoint);

                    // The difference is how much the platform moved/rotated
                    platformMove = newGlobalPoint - activeGlobalPoint;

                    // Handle Character Rotation (Orbiting)
                    if (rotateWithPlatform)
                    {
                        Quaternion newGlobalRotation = activePlatform.rotation;
                        Quaternion deltaRotation = newGlobalRotation * Quaternion.Inverse(activeGlobalRotation);

                        // We only want to rotate the character on the Y axis usually
                        Vector3 eulerRot = deltaRotation.eulerAngles;
                        transform.Rotate(0, eulerRot.y, 0);
                    }
                }

                // Update Platform Tracking for next frame
                activePlatform = hit.transform;
                activeLocalPoint = activePlatform.InverseTransformPoint(transform.position);
                activeGlobalPoint = transform.position;
                activeGlobalRotation = activePlatform.rotation;
            }
        }
        else
        {
            // Reset if in air
            activePlatform = null;
        }

        // 3. Apply Total Movement
        controller.Move(frameMove + platformMove);
    }

    private void PerformLeap()
    {
        float chargeRatio = Mathf.Clamp01(jumpChargeTimer / maxChargeTime);
        float multiplier = Mathf.Pow(chargeRatio, 2);

        if (multiplier > 0.1f)
        {
            Vector3 leapDirection = (lookDirection + Vector3.up * leapUpwardBias).normalized;

            currentHorizontalVelocity = leapDirection * maxLeapForce * multiplier;
            currentHorizontalVelocity.y = 0;
            verticalVelocity = leapDirection.y * maxLeapForce * multiplier;

            transform.rotation = Quaternion.LookRotation(lookDirection);

            // Detach from platform on leap
            activePlatform = null;

            OnLeapPerformed?.Invoke();
        }

        ResetCharge();
    }

    private void ResetCharge()
    {
        if (isCharging)
        {
            jumpChargeTimer = 0f;
            isCharging = false;
            OnChargeStopped?.Invoke();
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                // Small push down to ensure isGrounded stays true on slopes/moving platforms
                verticalVelocity = -5f;
            }

            if (!isCrouching && jumpBufferTimer > 0f)
            {
                verticalVelocity = jumpSpeed;
                jumpBufferTimer = 0f;
                activePlatform = null; // Detach on jump
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleHorizontalMovement()
    {
        float currentSpeed = moveSpeed;

        if (isCrouching && isJumpHeld)
        {
            currentSpeed *= chargeMoveMultiplier;
        }

        Vector3 targetVelocity = moveDirection * currentSpeed;

        currentHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );
    }

    private void HandleRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}