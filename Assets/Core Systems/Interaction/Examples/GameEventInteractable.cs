using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class GameEventInteractable : Interactable
    {
        [Header("Game Event Settings")]
        [SerializeField] private GameEvent _gameEvent;

        public override bool Interact(Interactor interactor)
        {
            if (_gameEvent != null)
            {
                _gameEvent.Raise();
                Debug.Log($"Raised event {_gameEvent.name} from {gameObject.name}");
                return true;
            }
            return false;
        }
    }
}
