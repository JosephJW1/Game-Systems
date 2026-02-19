using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : BaseMover
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 30f;
    public float acceleration = 50f;

    [Header("Physics")]
    public float gravity = -20f;
    public float jumpSpeed = 8f;

    [Header("Moving Platforms")]
    public LayerMask groundLayer = ~0;
    public float platformRayDistance = 0.5f;
    public bool rotateWithPlatform = true;

    [Header("Jump Buffering")]
    public float jumpBufferTime = 0.2f;

    [Header("Charged Leap")]
    public float maxLeapForce = 20f;
    public float maxChargeTime = 1.5f;
    public float leapUpwardBias = 0.5f;
    [Range(0f, 1f)] public float chargeMoveMultiplier = 0.3f;

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
    private Quaternion activeGlobalRotation; // FIXED: Restored this variable
    private Vector3 currentPlatformVelocity;

    // --- IMPLEMENTATION ---
    public override Vector3 Velocity => controller.velocity - currentPlatformVelocity;
    public override bool IsGrounded => controller.isGrounded;
    public override bool IsCrouching => isCrouching;
    public override bool IsCharging => isCharging;
    public override float CurrentMaxSpeed => (isCrouching && isJumpHeld) ? moveSpeed * chargeMoveMultiplier : moveSpeed;
    // -----------------------

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
            lookDirection = direction.normalized;
    }

    public override void SetJumpInput(bool held)
    {
        if (isCrouching) { if (isJumpHeld && !held) PerformLeap(); }
        else if (held && !isJumpHeld) Jump();
        isJumpHeld = held;
    }

    public override void SetCrouchInput(bool crouching)
    {
        isCrouching = crouching;
        if (!isCrouching) ResetCharge();
    }

    private void Jump()
    {
        if (!isCrouching)
        {
            if (controller.isGrounded)
            {
                verticalVelocity = jumpSpeed;
                activePlatform = null;
                currentPlatformVelocity = Vector3.zero;
            }
            else jumpBufferTimer = jumpBufferTime;
        }
    }

    void Update()
    {
        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;

        // Charge Logic
        if (isCrouching && isJumpHeld)
        {
            jumpChargeTimer += Time.deltaTime;
            isCharging = true;
            OnChargeProgress?.Invoke(Mathf.Clamp01(jumpChargeTimer / maxChargeTime));
        }
        else if (isCharging) ResetCharge();

        HandleGravity();
        HandleHorizontalMovement();
        HandleRotation();

        // Platform Logic
        Vector3 finalMove = currentHorizontalVelocity + (Vector3.up * verticalVelocity);
        Vector3 frameMove = finalMove * Time.deltaTime;
        Vector3 platformMove = Vector3.zero;

        if (controller.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, (controller.height / 2f) + platformRayDistance, groundLayer))
            {
                if (activePlatform == hit.transform)
                {
                    // 1. Calculate Position Delta
                    Vector3 newGlobalPoint = activePlatform.TransformPoint(activeLocalPoint);
                    platformMove = newGlobalPoint - activeGlobalPoint;

                    if (Time.deltaTime > 0) currentPlatformVelocity = platformMove / Time.deltaTime;

                    // 2. Calculate Rotation Delta (FIXED)
                    if (rotateWithPlatform)
                    {
                        Quaternion newGlobalRotation = activePlatform.rotation;
                        Quaternion deltaRotation = newGlobalRotation * Quaternion.Inverse(activeGlobalRotation);
                        transform.Rotate(0, deltaRotation.eulerAngles.y, 0);
                    }
                }
                else
                {
                    currentPlatformVelocity = Vector3.zero;
                }

                // Update Trackers
                activePlatform = hit.transform;
                activeLocalPoint = activePlatform.InverseTransformPoint(transform.position);
                activeGlobalPoint = transform.position;
                activeGlobalRotation = activePlatform.rotation; // FIXED: Save rotation for next frame comparison
            }
        }
        else
        {
            activePlatform = null;
            currentPlatformVelocity = Vector3.zero;
        }

        controller.Move(frameMove + platformMove);
    }

    private void HandleHorizontalMovement()
    {
        float targetSpeed = moveSpeed;
        if (isCrouching && isJumpHeld) targetSpeed *= chargeMoveMultiplier;
        if (moveDirection.magnitude < 0.001f) targetSpeed = 0f;

        // 1. Calculate Magnitude (Smooth)
        float currentSpeed = currentHorizontalVelocity.magnitude;
        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // 2. Apply Direction (Instant Snap to prevent sliding)
        if (moveDirection.magnitude > 0.001f)
        {
            currentHorizontalVelocity = moveDirection * newSpeed;
        }
        else
        {
            currentHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -5f;
            if (!isCrouching && jumpBufferTimer > 0f)
            {
                verticalVelocity = jumpSpeed;
                jumpBufferTimer = 0f;
                activePlatform = null;
                currentPlatformVelocity = Vector3.zero;
            }
        }
        else verticalVelocity += gravity * Time.deltaTime;
    }

    private void PerformLeap()
    {
        float ratio = Mathf.Clamp01(jumpChargeTimer / maxChargeTime);
        float multiplier = Mathf.Pow(ratio, 2);

        if (multiplier > 0.1f)
        {
            Vector3 leapDir = (lookDirection + Vector3.up * leapUpwardBias).normalized;
            currentHorizontalVelocity = leapDir * maxLeapForce * multiplier;
            currentHorizontalVelocity.y = 0;
            verticalVelocity = leapDir.y * maxLeapForce * multiplier;
            transform.rotation = Quaternion.LookRotation(lookDirection);
            activePlatform = null;
            currentPlatformVelocity = Vector3.zero;
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
}