using UnityEngine;
using UnityEngine.Events;

public class Debugger : MonoBehaviour
{
    [SerializeField] private UnityEvent onPickup;

    public void NotifyInventoryUpdate()
    {
        Debug.Log("<color=green>EVENT: Inventory Updated!</color>");
    }
}