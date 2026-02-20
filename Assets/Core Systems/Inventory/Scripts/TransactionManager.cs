using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class TransactionManager : MonoBehaviour
{
    [Header("Identity")]
    public Inventory myInventory;

    [Header("UI Binding")]
    [Tooltip("The ObjectListUI should target this script and read this list!")]
    public List<TradeOption> activeTradeOptions = new List<TradeOption>();

    [Header("Events")]
    public UnityEvent onShopOpened;
    public UnityEvent onTradeEnded;
    public UnityEvent onTradeUpdated;

    private Inventory _currentPartnerInventory;
    public Inventory CurrentPartnerInventory => _currentPartnerInventory;

    private void Start()
    {
        if (myInventory == null) myInventory = GetComponent<Inventory>();
    }

    public void BeginTrade(Inventory partnerInventory, List<TradeOption> options)
    {
        _currentPartnerInventory = partnerInventory;

        // CRITICAL FIX: Clear and AddRange so the UI doesn't lose its reflection reference
        activeTradeOptions.Clear();
        activeTradeOptions.AddRange(options);

        // Evaluate CanAfford / IsInStock immediately
        EvaluateAllOptions();

        if (onShopOpened != null) onShopOpened.Invoke();
        if (onTradeUpdated != null) onTradeUpdated.Invoke();
    }

    public void EndTrade()
    {
        _currentPartnerInventory = null;
        activeTradeOptions.Clear();
        if (onTradeEnded != null) onTradeEnded.Invoke();
    }

    public void ExecuteTradeOption(object optionObj)
    {
        TradeOption option = optionObj as TradeOption;

        if (option == null || !option.IsValid) return;

        if (!myInventory.HasEnough(option.currencyItem, option.currencyAmount) ||
            !_currentPartnerInventory.HasEnough(option.productItem, option.productAmount)) return;

        // Exchange Items
        myInventory.RemoveItem(option.currencyItem, option.currencyAmount);
        _currentPartnerInventory.AddItem(option.currencyItem, option.currencyAmount);

        _currentPartnerInventory.RemoveItem(option.productItem, option.productAmount);
        int leftovers = myInventory.AddItem(option.productItem, option.productAmount);

        if (leftovers > 0) _currentPartnerInventory.AddItem(option.productItem, leftovers);

        // Re-evaluate UI options
        EvaluateAllOptions();

        // Tell UI to refresh
        if (onTradeUpdated != null) onTradeUpdated.Invoke();
    }

    private void EvaluateAllOptions()
    {
        if (_currentPartnerInventory == null) return;

        foreach (var option in activeTradeOptions)
        {
            option.CanAfford = myInventory.HasEnough(option.currencyItem, option.currencyAmount);
            option.IsInStock = _currentPartnerInventory.HasEnough(option.productItem, option.productAmount);
        }
    }
}