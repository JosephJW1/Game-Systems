using UnityEngine;

namespace CoreSystems.Interaction.Test
{
    /// <summary>
    /// A simple test script to trigger interaction via Key input.
    /// Add this to the same object as the Interactor.
    /// </summary>
    public class InteractionTester : MonoBehaviour
    {
        [SerializeField] private Interactor _interactor;
        [SerializeField] private KeyCode _interactKey = KeyCode.E;

        private void Reset()
        {
            _interactor = GetComponent<Interactor>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_interactKey))
            {
                if (_interactor != null)
                {
                    _interactor.TryInteract();
                }
            }
        }
    }
}
