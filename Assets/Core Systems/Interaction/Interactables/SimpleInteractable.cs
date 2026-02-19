using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class SimpleInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactMessage;

        public string GetInteractText()
        {
            return $"Interact with {gameObject.name}";
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool Interact(Interactor interactor)
        {
            Debug.Log(interactMessage);
            return true;
        }
    }
}
