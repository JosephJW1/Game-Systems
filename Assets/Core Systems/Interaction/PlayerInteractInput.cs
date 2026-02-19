using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractInput : MonoBehaviour
{
    private Interactor interactor;

    void Start()
    {
        interactor = GetComponent<Interactor>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactor.TryInteract();
        }
    }
}
