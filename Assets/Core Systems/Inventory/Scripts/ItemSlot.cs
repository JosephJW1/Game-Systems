[System.Serializable]
public class ItemSlot
{
    public ItemData ItemData;
    public int amount;

    public ItemSlot(ItemData data, int amount)
    {
        this.ItemData = data;
        this.amount = amount;
    }
}