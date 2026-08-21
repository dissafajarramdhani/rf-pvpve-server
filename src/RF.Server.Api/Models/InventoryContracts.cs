namespace RF.Server.Api.Models;

public sealed record AddInventoryItemRequest(long AccountId, long CharacterId, string ItemCode);
public sealed record EquipItemRequest(long AccountId, long CharacterId, long ItemId, string SlotName);
public sealed record InventoryItemResponse(long Id, long CharacterId, string ItemCode, string ItemName, string ItemType, string Rarity, int Quantity, bool IsEquipped, string Slot);
