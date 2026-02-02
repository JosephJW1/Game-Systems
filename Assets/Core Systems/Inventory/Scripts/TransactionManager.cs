using UnityEngine;

public class TransactionManager : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private Inventory playerInventory;

    private Inventory currentNpcInventory;
    private int currentTransferAmount = 1;

    public void SetTransferAmount(string amountString)
    {
        if (int.TryParse(amountString, out int amount) && amount > 0)
        {
            this.currentTransferAmount = amount;
        }
        else
        {
            this.currentTransferAmount = 1;
        }
    }

    public void TestDebugItem(object item)
    {
        ItemSlot itemSlot = item as ItemSlot;
        if (itemSlot == null)
        {
            Debug.LogError("Item was not an ItemSlot!", this);
            return;
        }

        Debug.Log(itemSlot.ItemData.name);
    }

    public void TestRemoveCurrentAmount(object item)
    {
        ItemSlot itemSlot = item as ItemSlot;
        if (itemSlot == null)
        {
            Debug.LogError("Item was not an ItemSlot!", this);
            return;
        }

        int amountToRemove = this.currentTransferAmount;

        if (playerInventory.HasEnough(itemSlot.ItemData, amountToRemove))
        {
            playerInventory.RemoveItem(itemSlot, amountToRemove);
        }
        else
        {
            Debug.LogWarning($"Not enough {itemSlot.ItemData.name} to remove.", this);
        }
    }

    public void OnGiveItemToNPC(object item)
    {
        ItemSlot itemSlot = item as ItemSlot;
        if (itemSlot == null) return;
        if (currentNpcInventory == null)
        {
            Debug.LogWarning("No NPC is active!");
            return;
        }

        TransferItem(itemSlot, playerInventory, currentNpcInventory, this.currentTransferAmount);
    }

    public void OnTakeItemFromNPC(object item)
    {
        ItemSlot itemSlot = item as ItemSlot;
        if (itemSlot == null) return;
        if (currentNpcInventory == null) return;

        TransferItem(itemSlot, currentNpcInventory, playerInventory, this.currentTransferAmount);
    }

    public void TransferItem(ItemSlot item, Inventory from, Inventory to, int amount)
    {
        ItemData data = item.ItemData;

        if (from.HasEnough(data, amount))
        {
            from.RemoveItem(item, amount);

            to.AddItem(data, amount);
        }
    }

    public void SetActiveNPC(Inventory npcInventory)
    {
        this.currentNpcInventory = npcInventory;
    }
}