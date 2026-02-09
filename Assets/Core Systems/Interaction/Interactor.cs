using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private int _bufferSize = 3;

    [Header("Events")]
    public UnityEvent<IInteractable> OnInteractableFound;
    public UnityEvent OnInteractableLost;

    private Collider[] _colliders;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _colliders = new Collider[_bufferSize];
    }

    private void Update()
    {
        if (Time.frameCount % 5 == 0) // Optimization: Don't scan every single frame
        {
            ScanForInteractables();
        }
    }

    private void ScanForInteractables()
    {
        Vector3 origin = _interactionPoint != null ? _interactionPoint.position : transform.position;
        int numFound = Physics.OverlapSphereNonAlloc(origin, _interactionRange, _colliders, _interactionLayer);

        IInteractable foundInteractable = null;

        if (numFound > 0)
        {
            // Find the closest or most relevant interactable
            // For now, just pick the first valid one
            var interactable = _colliders[0].GetComponent<IInteractable>();
            if (interactable != null)
            {
                foundInteractable = interactable;
            }
        }

        if (foundInteractable != _currentInteractable)
        {
            _currentInteractable = foundInteractable;
            
            if (_currentInteractable != null)
            {
                OnInteractableFound?.Invoke(_currentInteractable);
            }
            else
            {
                OnInteractableLost?.Invoke();
            }
        }
    }

    public void TryInteract()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact(this);
        }
    }

    public IInteractable GetCurrentInteractable()
    {
        return _currentInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = _interactionPoint != null ? _interactionPoint.position : transform.position;
        Gizmos.DrawWireSphere(origin, _interactionRange);
    }
}
