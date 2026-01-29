using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Details")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;

    [Header("Stacking")]
    [SerializeField] private int _maxStack = 1;

    // Public Properties for external reading (like UI reflection)
    public string itemName => _itemName;
    public Sprite icon => _icon;
    public int maxStack => _maxStack;
}