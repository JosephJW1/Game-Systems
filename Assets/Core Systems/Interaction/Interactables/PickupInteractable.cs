using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class PickupInteractable : MonoBehaviour, IInteractable
    {
        [Header("Pickup Settings")]
        [SerializeField] private ItemData item;
        [SerializeField] private int amount = 1;

        // Allows the TransactionManager to set the item and quantity when dropped
        public void Setup(ItemData newItem, int newAmount)
        {
            this.item = newItem;
            this.amount = newAmount;
        }

        public string GetInteractText()
        {
            if (item == null) return "Empty Pickup";
            if (amount > 1) return $"Pick Up {amount} {item.itemName}s";
            else return $"Pick Up {item.itemName}";
        }

        public ItemData GetInteractItem()
        {
            if (item != null) return item;
            return null;
        }

        public bool Interact(Interactor interactor)
        {
            var inventory = interactor.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning($"Interactor {interactor.name} has no Inventory component!");
                return false;
            }

            if (item != null)
            {
                int leftovers = inventory.AddItem(item, amount);

                if (leftovers == 0)
                {
                    Debug.Log($"Picked up {amount} x {item.itemName}");
                    Destroy(gameObject);
                    return true;
                }
                else
                {
                    int pickedUp = amount - leftovers;
                    if (pickedUp > 0)
                    {
                        amount = leftovers;
                        Debug.Log($"Picked up partial: {pickedUp} x {item.itemName}, remaining: {amount}");
                        return true;
                    }

                    Debug.Log("Inventory full!");
                    return false;
                }
            }
            return false;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}