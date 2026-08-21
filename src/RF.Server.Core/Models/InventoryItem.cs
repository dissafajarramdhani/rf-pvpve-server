namespace RF.Server.Core.Models;

public sealed class InventoryItem
{
    public long Id { get; init; }
    public long CharacterId { get; init; }
    public ItemTemplate Template { get; init; }
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public string Slot { get; set; } = string.Empty;

    public InventoryItem(long id, long characterId, ItemTemplate template, int quantity = 1)
    {
        Id = id;
        CharacterId = characterId;
        Template = template;
        Quantity = quantity;
    }
}
