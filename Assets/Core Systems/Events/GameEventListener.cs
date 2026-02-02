using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("The event channel to listen to.")]
    [SerializeField] private GameEvent eventChannel;

    [Tooltip("The response to invoke when the event is raised.")]
    [SerializeField] private UnityEvent response;

    private void OnEnable()
    {
        if (eventChannel != null)
            eventChannel.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (eventChannel != null)
            eventChannel.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }
}