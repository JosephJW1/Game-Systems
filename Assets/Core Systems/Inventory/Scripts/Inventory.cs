using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [field: SerializeField]
    public List<ItemSlot> items { get; private set; } = new List<ItemSlot>();

    [SerializeField] public UnityEvent onInventoryUpdated;

    // Optional: Add a capacity limit later
    // [SerializeField] private int maxSlots = 20;

    public bool HasEnough(ItemData itemToFind, int amountNeeded)
    {
        int totalAmount = 0;
        foreach (ItemSlot slot in items)
        {
            if (slot.ItemData == itemToFind)
            {
                totalAmount += slot.quantity;
            }
        }
        return totalAmount >= amountNeeded;
    }

    /// <summary>
    /// Adds items to the inventory.
    /// </summary>
    /// <returns>The amount of items that COULD NOT be added (leftovers).</returns>
    public int AddItem(ItemData itemToAdd, int amountToAdd)
    {
        bool changed = false;
        int remainingAmount = amountToAdd;

        // 1. Try to stack on existing slots first
        foreach (ItemSlot slot in items)
        {
            if (slot.ItemData == itemToAdd && slot.quantity < slot.ItemData.maxStack)
            {
                int spaceLeft = slot.ItemData.maxStack - slot.quantity;
                int amountToMove = Mathf.Min(remainingAmount, spaceLeft);
                if (amountToMove > 0)
                {
                    slot.quantity += amountToMove;
                    remainingAmount -= amountToMove;
                    changed = true;
                }
                if (remainingAmount == 0) break;
            }
        }

        // 2. Create new slots for remaining amount
        // Note: If you implement a maxSlots check, put it here:
        // while (remainingAmount > 0 && items.Count < maxSlots)
        while (remainingAmount > 0)
        {
            ItemSlot newSlot = new ItemSlot { ItemData = itemToAdd };
            int amountToMove = Mathf.Min(remainingAmount, itemToAdd.maxStack);
            newSlot.quantity = amountToMove;
            remainingAmount -= amountToMove;
            items.Add(newSlot);
            changed = true;
        }

        if (changed)
        {
            onInventoryUpdated.Invoke();
        }

        return remainingAmount;
    }

    public void RemoveItem(ItemSlot preferredSlot, int amountToRemove)
    {
        // Safety Check: Verify the list actually contains this specific object instance
        if (!items.Contains(preferredSlot))
        {
            Debug.LogWarning($"Inventory: Preferred slot for {preferredSlot.ItemData.name} not found by reference. Falling back to type-based removal.");
            RemoveItem(preferredSlot.ItemData, amountToRemove);
            return;
        }

        int remainingAmount = amountToRemove;

        int amountFromThisSlot = Mathf.Min(remainingAmount, preferredSlot.quantity);
        if (amountFromThisSlot > 0)
        {
            preferredSlot.quantity -= amountFromThisSlot;
            remainingAmount -= amountFromThisSlot;

            if (preferredSlot.quantity == 0)
            {
                items.Remove(preferredSlot);
            }
        }

        // If the preferred slot wasn't enough, continue removing from others
        if (remainingAmount > 0)
        {
            RemoveItem(preferredSlot.ItemData, remainingAmount);
        }
        else
        {
            onInventoryUpdated.Invoke();
        }
    }

    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        bool changed = false;
        int remainingAmount = amountToRemove;

        // Iterate backwards to safely remove items while looping
        for (int i = items.Count - 1; i >= 0; i--)
        {
            ItemSlot slot = items[i];
            if (slot.ItemData == itemToRemove)
            {
                int amountToMove = Mathf.Min(remainingAmount, slot.quantity);
                if (amountToMove > 0)
                {
                    slot.quantity -= amountToMove;
                    remainingAmount -= amountToMove;
                    changed = true;
                }
                if (slot.quantity == 0)
                {
                    items.RemoveAt(i);
                }
                if (remainingAmount == 0) break;
            }
        }

        if (changed)
        {
            onInventoryUpdated.Invoke();
        }
    }
}