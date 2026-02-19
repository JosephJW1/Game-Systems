using NUnit.Framework;
using System.Collections.Generic;
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
        if (Time.frameCount % 5 == 0)
        {
            ScanForInteractables();
        }
    }

    private void ScanForInteractables()
    {
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, _colliders, _interactionLayer);

        IInteractable closestInteractable = null;

        for (int i = 0; i < numFound; i++)
        {
            Collider col = _colliders[i];

            if (col.TryGetComponent(out IInteractable interactable))
            {
                if (closestInteractable == null)
                {
                    closestInteractable = interactable;
                }

                else if (
                    Vector3.Distance(transform.position, (interactable as Component).transform.position) <
                    Vector3.Distance(transform.position, (closestInteractable as Component).transform.position))
                {
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != _currentInteractable)
        {
            _currentInteractable = closestInteractable;

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
        _currentInteractable?.Interact(this);
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
