using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionPrompt = "Interact";

    public string InteractionPrompt => _interactionPrompt;

    public abstract bool Interact(Interactor interactor);
}
