// TO BE DELETED AFTER UI

using UnityEngine;

namespace CoreSystems.Interaction.Test
{
    public class InteractionTester : MonoBehaviour
    {
        public void TriggerInteractUI(IInteractable interactable)
        {
            Debug.Log(interactable.GetInteractText());
        }

        public void CancelInteractUI()
        {
            Debug.Log("Nothing to interact with.");
        }
    }
}
