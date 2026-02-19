using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Inventory))]
public class TradeInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    [SerializeField] private string interactPrompt = "Loot/Trade";
    [SerializeField] private UnityEvent onInteract;

    [Header("Permissions (Free Trade)")]
    public bool allowFreeTake = true;
    public bool allowFreeDeposit = true;

    [Header("Trade Rules (Shop)")]
    [SerializeField] private List<TradeRecipe> acceptableTrades = new List<TradeRecipe>();

    private Inventory myInventory;

    private void Awake()
    {
        myInventory = GetComponent<Inventory>();
    }

    public string GetInteractText() => interactPrompt;
    public Transform GetTransform() => transform;

    public bool Interact(Interactor interactor)
    {
        if (onInteract != null) onInteract.Invoke();

        var manager = interactor.GetComponent<TransactionManager>();

        if (manager != null)
        {
            manager.BeginTrade(myInventory, acceptableTrades, allowFreeTake, allowFreeDeposit);
            return true;
        }

        Debug.LogWarning($"Interaction failed: {interactor.name} has no TransactionManager.");
        return false;
    }
}