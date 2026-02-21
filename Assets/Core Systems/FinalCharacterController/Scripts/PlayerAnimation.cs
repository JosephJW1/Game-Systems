using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GinjaGaming.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private BaseMover _mover;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private PlayerState _playerState;
        private PlayerActionsInput _playerActionsInput;

        // Locomotion Hashes
        private static readonly int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static readonly int isGroundedHash = Animator.StringToHash("isGrounded");
        private static readonly int isFallingHash = Animator.StringToHash("isFalling");
        private static readonly int isJumpingHash = Animator.StringToHash("isJumping");
        private static readonly int isCrouchingHash = Animator.StringToHash("isCrouching");

        // Action Hashes (Keeping your existing action setup)
        private static readonly int isAttackingHash = Animator.StringToHash("isAttacking");
        private static readonly int isGatheringHash = Animator.StringToHash("isGathering");

        private Vector3 _currentBlendInput;

        private void Awake()
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            if (_mover == null) _mover = GetComponentInParent<BaseMover>();

            _playerState = GetComponentInParent<PlayerState>();
            _playerActionsInput = GetComponentInParent<PlayerActionsInput>();
        }

        private void Update()
        {
            if (_mover == null) return;

            UpdateLocomotionAnimations();
            UpdateActionAnimations();
        }

        private void UpdateLocomotionAnimations()
        {
            // Velocity magnitude on the XZ plane for blending
            Vector3 horizontalVelocity = new Vector3(_mover.Velocity.x, 0, _mover.Velocity.z);
            float speedPercent = horizontalVelocity.magnitude / _mover.CurrentMaxSpeed;

            _animator.SetFloat(inputMagnitudeHash, speedPercent, 0.1f, Time.deltaTime);

            // Ground and Air States
            _animator.SetBool(isGroundedHash, _mover.IsGrounded);
            _animator.SetBool(isFallingHash, !_mover.IsGrounded && _mover.Velocity.y < 0f);
            _animator.SetBool(isJumpingHash, !_mover.IsGrounded && _mover.Velocity.y > 0f);

            // Crouch state from BaseMover
            _animator.SetBool(isCrouchingHash, _mover.IsCrouching);
        }

        private void UpdateActionAnimations()
        {
            if (_playerActionsInput == null) return;

            // Retaining your original action states mapping
            _animator.SetBool(isAttackingHash, _playerActionsInput.AttackPressed);
            _animator.SetBool(isGatheringHash, _playerActionsInput.GatherPressed);
        }
    }
}