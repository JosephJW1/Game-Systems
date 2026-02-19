using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Economy/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Details")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;

    [Header("World Representation")]
    [Tooltip("The physical object spawned when dropped.")]
    [SerializeField] private GameObject _pickupPrefab;

    [Header("Stacking")]
    [SerializeField] private int _maxStack = 99;

    // Public properties for your UI and Pickup scripts to read safely
    public string itemName => _itemName;
    public Sprite icon => _icon;
    public GameObject pickupPrefab => _pickupPrefab;
    public int maxStack => _maxStack;
}