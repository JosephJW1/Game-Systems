using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class GameEventInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameEvent gameEvent;
        [SerializeField] private string eventDescription;

        public string GetInteractText()
        {
            return eventDescription;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool Interact(Interactor interactor)
        {
            if (gameEvent != null)
            {
                gameEvent.Raise();
                return true;
            }
            return false;
        }
    }
}
