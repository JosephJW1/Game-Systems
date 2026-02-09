using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class SimpleInteractable : Interactable
    {
        public override bool Interact(Interactor interactor)
        {
            Debug.Log($"Interacted with {gameObject.name} by {interactor.gameObject.name}");
            return true;
        }
    }
}
