using RF.Server.Api.Data;
using RF.Server.Api.Models;
using RF.Server.Core.Models;

namespace RF.Server.Api.Services;

public sealed class InventoryAppService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryAppService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<InventoryItemResponse>> GetInventoryAsync(long accountId, long characterId, CancellationToken cancellationToken = default)
    {
        var items = await _inventoryRepository.GetByCharacterIdAsync(characterId, cancellationToken);
        return items.Select(item => new InventoryItemResponse(
            item.Id,
            item.CharacterId,
            item.Template.Code,
            item.Template.Name,
            item.Template.ItemType,
            item.Template.Rarity,
            item.Quantity,
            item.IsEquipped,
            item.Slot)).ToList();
    }

    public async Task<InventoryItemResponse> AddItemAsync(long accountId, long characterId, string itemCode, CancellationToken cancellationToken = default)
    {
        var item = await _inventoryRepository.AddItemAsync(characterId, itemCode, cancellationToken);
        return new InventoryItemResponse(
            item.Id,
            item.CharacterId,
            item.Template.Code,
            item.Template.Name,
            item.Template.ItemType,
            item.Template.Rarity,
            item.Quantity,
            item.IsEquipped,
            item.Slot);
    }

    public async Task<InventoryItemResponse?> EquipItemAsync(long accountId, long characterId, long itemId, string slotName, CancellationToken cancellationToken = default)
    {
        var item = await _inventoryRepository.EquipItemAsync(characterId, itemId, slotName, cancellationToken);
        if (item is null)
            return null;

        return new InventoryItemResponse(
            item.Id,
            item.CharacterId,
            item.Template.Code,
            item.Template.Name,
            item.Template.ItemType,
            item.Template.Rarity,
            item.Quantity,
            item.IsEquipped,
            item.Slot);
    }
}
