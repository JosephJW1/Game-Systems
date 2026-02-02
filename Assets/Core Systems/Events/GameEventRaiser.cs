using UnityEngine;

public class GameEventRaiser : MonoBehaviour
{
    [Tooltip("The Event Channel asset to raise this event on.")]
    [SerializeField] private GameEvent eventChannel;

    public void RaiseEvent()
    {
        if (eventChannel != null)
        {
            eventChannel.Raise();
        }
        else
        {
            Debug.LogWarning("GameEventRaiser is missing an Event Channel!", this);
        }
    }
}