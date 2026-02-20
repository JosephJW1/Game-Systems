using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Inventory))]
public class TradeInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    [SerializeField] private string interactPrompt = "Browse Shop";
    [SerializeField] private UnityEvent onInteract;

    [Header("Trade Rules (Shop)")]
    // You define the costs and rewards right here in the NPC's Inspector
    [SerializeField] private List<TradeOption> shopInventory = new List<TradeOption>();

    private Inventory myInventory;

    private void Awake() => myInventory = GetComponent<Inventory>();

    public string GetInteractText() => interactPrompt;
    public Transform GetTransform() => transform;

    public bool Interact(Interactor interactor)
    {
        if (onInteract != null) onInteract.Invoke();

        var manager = interactor.GetComponent<TransactionManager>();
        if (manager != null)
        {
            manager.BeginTrade(myInventory, shopInventory);
            return true;
        }
        return false;
    }
}