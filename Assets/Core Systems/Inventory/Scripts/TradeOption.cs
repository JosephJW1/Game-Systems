using UnityEngine;

[System.Serializable] // Allows you to edit Cost/Reward in the Inspector
public class TradeOption
{
    [Header("Cost")]
    public ItemData currencyItem;
    public int currencyAmount = 1;

    [Header("Reward")]
    public ItemData productItem;
    public int productAmount = 1;

    // --- RUNTIME DATA (Used by the UI, hidden from Inspector) ---
    public bool CanAfford { get; set; }
    public bool IsInStock { get; set; }

    // The property your ObjectListUI will look for to enable/disable the button!
    public bool IsValid => CanAfford && IsInStock;

    // Helper strings for ObjectListUI
    public string CostText => $"{currencyAmount}x {(currencyItem != null ? currencyItem.itemName : "Unknown")}";
    public string RewardText => $"{productAmount}x {(productItem != null ? productItem.itemName : "Unknown")}";
}