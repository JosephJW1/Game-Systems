using UnityEngine;

namespace CoreSystems.Interaction.Examples
{
    public class PickupInteractable : Interactable
    {
        [Header("Pickup Settings")]
        [SerializeField] private ItemData _item;
        [SerializeField] private int _amount = 1;

        public override bool Interact(Interactor interactor)
        {
            // Try to find an Inventory on the interactor
            var inventory = interactor.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning($"Interactor {interactor.name} has no Inventory component!");
                return false;
            }

            if (_item != null)
            {
                int leftovers = inventory.AddItem(_item, _amount);
                
                if (leftovers == 0)
                {
                    Debug.Log($"Picked up {_amount} x {_item.itemName}");
                    Destroy(gameObject); // Fully picked up
                    return true;
                }
                else
                {
                     // Partially picked up? For now, we only support full pickup or nothing logic modification in future if needed.
                     // But if we want to handle partial pickup, we should update _amount.
                     int pickedUp = _amount - leftovers;
                     if (pickedUp > 0)
                     {
                        _amount = leftovers;
                        Debug.Log($"Picked up partial: {pickedUp} x {_item.itemName}, remaining: {_amount}");
                        // Don't destroy, just update amount
                        return true;
                     }
                     
                     Debug.Log("Inventory full!");
                     return false;
                }
            }
            return false;
        }
    }
}
