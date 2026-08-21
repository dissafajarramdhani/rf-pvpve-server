namespace RF.Server.Core.Models;

public sealed class LootDrop
{
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string Rarity { get; init; } = "common";
    public double DropChance { get; init; }
    public int MinQuantity { get; init; }
    public int MaxQuantity { get; init; }

    public LootDrop(string itemCode, string itemName, string rarity, double dropChance, int minQuantity, int maxQuantity)
    {
        ItemCode = itemCode;
        ItemName = itemName;
        Rarity = rarity;
        DropChance = dropChance;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
    }
}
