using UnityEngine;
using UnityEngine.Events;

public abstract class GenericGameEvent<T> : ScriptableObject
{
    private UnityAction<T> onEventRaised;

    public void RegisterListener(UnityAction<T> listener) => onEventRaised += listener;
    public void UnregisterListener(UnityAction<T> listener) => onEventRaised -= listener;

    public void Raise(T value) => onEventRaised?.Invoke(value);
}
