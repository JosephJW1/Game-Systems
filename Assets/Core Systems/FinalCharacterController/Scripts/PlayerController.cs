using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GinjaGaming.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : BaseMover
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingToTarget { get; private set; } = false;

        [Header("Base Movement")]
        public float walkAcceleration = 25f;
        public float walkSpeed = 2f;
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float sprintAcceleration = 50f;
        public float sprintSpeed = 7f;
        public float inAirAcceleration = 25f;
        public float drag = 20f;
        public float inAirDrag = 5f;
        public float gravity = 25f;
        public float terminalVelocity = 50f;
        public float jumpSpeed = 0.8f;
        public float movingThreshold = 0.01f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.67f;

        [Header("Environment Details")]
        [SerializeField] private LayerMask _groundLayers;

        private PlayerState _playerState;

        // Input States from BaseMover
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector3.forward;
        private bool _isJumpHeld = false;
        private bool _isCrouching = false;
        private bool _isCharging = false;

        private float _verticalVelocity = 0f;
        private Vector3 _currentHorizontalVelocity = Vector3.zero;
        #endregion

        #region BaseMover Implementation
        public override Vector3 Velocity => _characterController.velocity;
        public override bool IsGrounded => _characterController.isGrounded;
        public override bool IsCrouching => _isCrouching;
        public override bool IsCharging => _isCharging;
        public override float CurrentMaxSpeed => runSpeed; // Can be dynamic based on your states

        public override void Move(Vector3 direction)
        {
            _moveDirection = direction.normalized;
        }

        public override void SetLookDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.001f)
                _lookDirection = direction.normalized;
        }

        public override void SetJumpInput(bool held)
        {
            if (held && !_isJumpHeld && IsGrounded)
            {
                Jump();
            }
            _isJumpHeld = held;
        }

        public override void SetCrouchInput(bool crouching)
        {
            _isCrouching = crouching;
        }
        #endregion

        #region Startup
        private void Awake()
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            _playerState = GetComponent<PlayerState>();
            _lookDirection = transform.forward;
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            HandleGravity();
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            // Determine target speed based on your PlayerState logic or input magnitude
            float targetSpeed = runSpeed;
            if (_moveDirection.magnitude < 0.001f) targetSpeed = 0f;

            float currentSpeed = _currentHorizontalVelocity.magnitude;
            float acceleration = IsGrounded ? runAcceleration : inAirAcceleration;
            float currentDrag = IsGrounded ? drag : inAirDrag;

            if (_moveDirection.magnitude > 0.001f)
            {
                float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                _currentHorizontalVelocity = _moveDirection * newSpeed;
            }
            else
            {
                _currentHorizontalVelocity = Vector3.MoveTowards(_currentHorizontalVelocity, Vector3.zero, currentDrag * Time.deltaTime);
            }

            Vector3 finalMovement = _currentHorizontalVelocity + (Vector3.up * _verticalVelocity);
            _characterController.Move(finalMovement * Time.deltaTime);
        }

        private void HandleRotation()
        {
            if (_moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerModelRotationSpeed * Time.deltaTime);
                IsRotatingToTarget = true;
            }
            else
            {
                IsRotatingToTarget = false;
            }
        }

        private void HandleGravity()
        {
            if (IsGrounded)
            {
                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f; // Stick to ground
            }
            else
            {
                _verticalVelocity -= gravity * Time.deltaTime;
                if (_verticalVelocity < -terminalVelocity)
                    _verticalVelocity = -terminalVelocity;
            }
        }

        private void Jump()
        {
            _verticalVelocity = Mathf.Sqrt(jumpSpeed * 2f * gravity);
            // Optionally update PlayerState here
        }
        #endregion
    }
}