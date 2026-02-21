using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private BaseMover mover;
    private Animator animator;

    // Hashes
    private static readonly int HorizontalVelocityHash = Animator.StringToHash("HorizontalVelocity");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");
    private static readonly int IsChargingHash = Animator.StringToHash("IsCharging");
    private static readonly int LeapHash = Animator.StringToHash("LeapTrigger");
    private static readonly int ChargeAmountHash = Animator.StringToHash("ChargeAmount");

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (mover == null) mover = GetComponentInParent<BaseMover>();
    }

    void OnEnable()
    {
        if (mover != null)
        {
            mover.OnLeapPerformed.AddListener(TriggerLeap);
            mover.OnChargeProgress.AddListener(UpdateChargeAmount);
        }
    }

    void OnDisable()
    {
        if (mover != null)
        {
            mover.OnLeapPerformed.RemoveListener(TriggerLeap);
            mover.OnChargeProgress.RemoveListener(UpdateChargeAmount);
        }
    }

    void Update()
    {
        if (mover == null) return;

        // 1. Grounded & Vertical
        animator.SetBool(IsGroundedHash, mover.IsGrounded);
        animator.SetFloat(VerticalVelocityHash, mover.Velocity.y);

        // 2. Speed
        float horizontalVelocity = mover.Velocity.x;
        float verticalVelocity = mover.Velocity.z;

        animator.SetFloat(HorizontalVelocityHash, horizontalVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat(VerticalVelocityHash, verticalVelocity, 0.1f, Time.deltaTime);

        // 3. States
        animator.SetBool(IsCrouchingHash, mover.IsCrouching);
        animator.SetBool(IsChargingHash, mover.IsCharging);
    }

    private void UpdateChargeAmount(float amount)
    {
        animator.SetFloat(ChargeAmountHash, amount);
    }

    private void TriggerLeap()
    {
        animator.SetTrigger(LeapHash);
    }
}