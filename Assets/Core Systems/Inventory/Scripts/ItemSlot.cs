using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    // Serialized backing fields
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _quantity;

    // Public Properties (ItemData, capital I) to satisfy UI reflection
    public ItemData ItemData
    {
        get { return _itemData; }
        set { _itemData = value; }
    }

    // Public Property for quantity
    public int quantity
    {
        get { return _quantity; }
        set { _quantity = value; }
    }
}