using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<ItemSlot> slots = new List<ItemSlot>();

    [Header("UI Updates")]
    public UnityEvent onInventoryChanged; // THIS was the missing piece!

    public bool HasEnough(ItemData data, int amountToCheck)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.ItemData == data) total += slot.amount;
        }
        return total >= amountToCheck;
    }

    public int AddItem(ItemData data, int amountToAdd)
    {
        foreach (var slot in slots)
        {
            if (slot.ItemData == data)
            {
                slot.amount += amountToAdd;
                onInventoryChanged?.Invoke(); // Tell UI to refresh!
                return 0;
            }
        }
        slots.Add(new ItemSlot(data, amountToAdd));
        onInventoryChanged?.Invoke(); // Tell UI to refresh!
        return 0;
    }

    public void RemoveItem(ItemSlot specificSlot, int amountToRemove)
    {
        if (slots.Contains(specificSlot))
        {
            specificSlot.amount -= amountToRemove;
            if (specificSlot.amount <= 0) slots.Remove(specificSlot);
            onInventoryChanged?.Invoke(); // Tell UI to refresh!
        }
    }

    public void RemoveItem(ItemData data, int amountToRemove)
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].ItemData == data)
            {
                if (slots[i].amount >= amountToRemove)
                {
                    slots[i].amount -= amountToRemove;
                    if (slots[i].amount <= 0) slots.RemoveAt(i);
                    onInventoryChanged?.Invoke(); // Tell UI to refresh!
                    return;
                }
                else
                {
                    amountToRemove -= slots[i].amount;
                    slots.RemoveAt(i);
                }
            }
        }
        onInventoryChanged?.Invoke(); // Tell UI to refresh!
    }
}