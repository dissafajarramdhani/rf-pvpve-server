using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItem>> GetByCharacterIdAsync(long characterId, CancellationToken cancellationToken = default);
    Task<InventoryItem> AddItemAsync(long characterId, string itemCode, CancellationToken cancellationToken = default);
    Task<InventoryItem?> EquipItemAsync(long characterId, long itemId, string slotName, CancellationToken cancellationToken = default);
}
