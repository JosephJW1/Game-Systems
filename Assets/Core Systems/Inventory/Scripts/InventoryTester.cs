using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    public Inventory playerInventory; // Drag your Player object here
    public ItemData potionItem; // Drag your Potion ItemData asset here
    public ItemData coinItem;   // Drag your Coin ItemData asset here

    void Update()
    {
        // Press 'P' to add 5 potions
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            playerInventory.AddItem(potionItem, 5);
            Debug.Log("Added 5 Potions");
        }

        // Press 'C' to add 50 coins
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            playerInventory.AddItem(coinItem, 50);
            Debug.Log("Added 50 Coins");
        }

        // Press 'R' to remove 2 potions
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            playerInventory.RemoveItem(potionItem, 2);
            Debug.Log("Removed 2 Potions");
        }
    }
}