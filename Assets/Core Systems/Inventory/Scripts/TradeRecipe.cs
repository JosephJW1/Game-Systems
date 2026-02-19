using UnityEngine;

[CreateAssetMenu(fileName = "New Trade Recipe", menuName = "Economy/Trade Recipe")]
public class TradeRecipe : ScriptableObject
{
    [Header("Cost")]
    public ItemData currencyItem;
    public int currencyAmount = 1;

    [Header("Reward")]
    public ItemData productItem;
    public int productAmount = 1;
}