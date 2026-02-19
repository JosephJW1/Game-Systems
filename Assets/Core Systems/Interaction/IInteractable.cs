using UnityEngine;

public interface IInteractable
{
    bool Interact(Interactor interactor);
    string GetInteractText();
    Transform GetTransform();
}
