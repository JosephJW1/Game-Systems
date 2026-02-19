using UnityEngine;
using UnityEngine.Events;

public abstract class BaseMover : MonoBehaviour
{
    // --- 1. CORE INPUTS ---
    public abstract void Move(Vector3 direction);
    public abstract void SetLookDirection(Vector3 direction);
    public abstract void SetJumpInput(bool held);
    public abstract void SetCrouchInput(bool crouching);

    // --- 2. DATA CONTRACT ---
    public abstract Vector3 Velocity { get; }
    public abstract float CurrentMaxSpeed { get; }
    public abstract bool IsGrounded { get; }
    public abstract bool IsCrouching { get; }
    public abstract bool IsCharging { get; }

    // --- 3. SHARED EVENTS ---
    [HideInInspector] public UnityEvent OnLeapPerformed = new UnityEvent();
    [HideInInspector] public UnityEvent<float> OnChargeProgress = new UnityEvent<float>();
    [HideInInspector] public UnityEvent OnChargeStopped = new UnityEvent();
}