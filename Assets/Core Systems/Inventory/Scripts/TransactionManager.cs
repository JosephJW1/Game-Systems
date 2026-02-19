using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using CoreSystems.Interaction.Examples; // Namespace for PickupInteractable

public class TransactionManager : MonoBehaviour
{
    [Header("Identity")]
    public Inventory myInventory;

    [Header("Events")]
    public UnityEvent<List<TradeRecipe>> onTradeStarted;
    public UnityEvent onTradeEnded;

    // --- STATE ---
    private Inventory _currentPartnerInventory;
    public Inventory CurrentPartnerInventory => _currentPartnerInventory;

    private bool _canTakeFreely;
    private bool _canDepositFreely;

    public bool CanTakeFreely => _canTakeFreely;
    public bool CanDepositFreely => _canDepositFreely;

    private void Start()
    {
        if (myInventory == null) myInventory = GetComponent<Inventory>();
    }

    public void BeginTrade(Inventory partnerInventory, List<TradeRecipe> partnerRecipes, bool allowTake, bool allowDeposit)
    {
        _currentPartnerInventory = partnerInventory;
        _canTakeFreely = allowTake;
        _canDepositFreely = allowDeposit;

        if (onTradeStarted != null) onTradeStarted.Invoke(partnerRecipes);
    }

    public void EndTrade()
    {
        _currentPartnerInventory = null;
        if (onTradeEnded != null) onTradeEnded.Invoke();
    }

    public void GiveItemToPartner(ItemSlot slot, int amount = 1)
    {
        if (_currentPartnerInventory == null) return;

        if (!_canDepositFreely)
        {
            Debug.LogWarning("You are not allowed to deposit items into this container.");
            return;
        }

        TransferFreeItem(slot, myInventory, _currentPartnerInventory, amount);
    }

    public void TakeItemFromPartner(ItemSlot slot, int amount = 1)
    {
        if (_currentPartnerInventory == null) return;

        if (!_canTakeFreely)
        {
            Debug.LogWarning("You are not allowed to take items from this container freely.");
            return;
        }

        TransferFreeItem(slot, _currentPartnerInventory, myInventory, amount);
    }

    public void ExecuteRecipe(TradeRecipe recipe)
    {
        if (recipe == null || _currentPartnerInventory == null) return;

        bool iHaveCurrency = myInventory.HasEnough(recipe.currencyItem, recipe.currencyAmount);
        bool partnerHasProduct = _currentPartnerInventory.HasEnough(recipe.productItem, recipe.productAmount);

        if (iHaveCurrency && partnerHasProduct)
        {
            // Execute the Trade
            myInventory.RemoveItem(recipe.currencyItem, recipe.currencyAmount);
            _currentPartnerInventory.AddItem(recipe.currencyItem, recipe.currencyAmount);

            _currentPartnerInventory.RemoveItem(recipe.productItem, recipe.productAmount);
            int leftovers = myInventory.AddItem(recipe.productItem, recipe.productAmount);

            if (leftovers > 0)
            {
                _currentPartnerInventory.AddItem(recipe.productItem, leftovers);
                Debug.LogWarning("Your inventory is full! Could not take the item.");
            }
            else
            {
                Debug.Log($"<color=green>SUCCESS:</color> Bought {recipe.productAmount} {recipe.productItem.itemName}!");
            }
        }
        else if (!iHaveCurrency)
        {
            Debug.LogWarning($"<color=orange>FAILED:</color> You do not have enough {recipe.currencyItem.itemName} to afford this!");
        }
        else if (!partnerHasProduct)
        {
            Debug.LogWarning($"<color=orange>FAILED:</color> The Merchant does not have enough {recipe.productItem.itemName} in stock!");
        }
    }

    public void DropItem(object itemObj, int amount)
    {
        ItemSlot slot = itemObj as ItemSlot;
        if (slot == null || slot.ItemData.pickupPrefab == null) return;

        if (myInventory.HasEnough(slot.ItemData, amount))
        {
            myInventory.RemoveItem(slot, amount);

            GameObject droppedObj = Instantiate(slot.ItemData.pickupPrefab, transform.position + transform.forward + Vector3.up, Random.rotation);
            var pickup = droppedObj.GetComponent<PickupInteractable>();
            if (pickup != null) pickup.Setup(slot.ItemData, amount);
        }
    }

    private void TransferFreeItem(ItemSlot slot, Inventory from, Inventory to, int amount)
    {
        if (from.HasEnough(slot.ItemData, amount))
        {
            from.RemoveItem(slot, amount);
            int leftovers = to.AddItem(slot.ItemData, amount);
            if (leftovers > 0) from.AddItem(slot.ItemData, leftovers);
        }
    }
}