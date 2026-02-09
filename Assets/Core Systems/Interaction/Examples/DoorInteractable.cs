using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class DoorInteractable : Interactable
    {
        [SerializeField] private bool _isOpen;
        [SerializeField] private float _openAngle = 90f;
        [SerializeField] private float _closeAngle = 0f;
        [SerializeField] private float _speed = 2f;
        
        private float _targetAngle;
        private Quaternion _initialRotation;

        private void Start()
        {
            _initialRotation = transform.localRotation;
            _targetAngle = _isOpen ? _openAngle : _closeAngle;
            // Immediate set for clean start
             if (_isOpen)
                SetRotation(_openAngle);
        }

        private void Update()
        {
            // Simple robust linear interpolation for demo
            float currentY = transform.localEulerAngles.y;
            float newY = Mathf.MoveTowardsAngle(currentY, _targetAngle, Time.deltaTime * _speed * 50f); // *50 for reasonable speed vs angle
            SetRotation(newY);
        }

        private void SetRotation(float angle)
        {
             Vector3 euler = transform.localEulerAngles;
             euler.y = angle;
             transform.localEulerAngles = euler;
        }

        public override bool Interact(Interactor interactor)
        {
            _isOpen = !_isOpen;
            _targetAngle = _isOpen ? _openAngle : _closeAngle;
            return true;
        }
    }
}
