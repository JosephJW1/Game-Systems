using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [field: SerializeField]
    public List<ItemSlot> items { get; private set; } = new List<ItemSlot>();

    [SerializeField] public UnityEvent onInventoryUpdated;

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

    public void AddItem(ItemData itemToAdd, int amountToAdd)
    {
        bool changed = false;
        int remainingAmount = amountToAdd;

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
    }

    public void RemoveItem(ItemSlot preferredSlot, int amountToRemove)
    {
        if (!items.Contains(preferredSlot))
        {
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