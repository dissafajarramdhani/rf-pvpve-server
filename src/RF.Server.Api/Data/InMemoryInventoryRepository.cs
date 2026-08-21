using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public sealed class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly Dictionary<long, List<InventoryItem>> _inventoryByCharacter = new();
    private long _nextItemId = 1;

    public Task<IReadOnlyList<InventoryItem>> GetByCharacterIdAsync(long characterId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_inventoryByCharacter.TryGetValue(characterId, out var items))
            return Task.FromResult<IReadOnlyList<InventoryItem>>(items.ToList().AsReadOnly());

        return Task.FromResult<IReadOnlyList<InventoryItem>>(Array.Empty<InventoryItem>());
    }

    public Task<InventoryItem> AddItemAsync(long characterId, string itemCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var template = ItemCatalog.Get(itemCode);
        var list = _inventoryByCharacter.GetValueOrDefault(characterId, new List<InventoryItem>());
        var existing = list.FirstOrDefault(x => x.Template.Code == itemCode && !x.IsEquipped);

        if (existing is not null && template.MaxStack > 1)
        {
            existing.Quantity += 1;
            return Task.FromResult(existing);
        }

        var item = new InventoryItem(_nextItemId++, characterId, template, 1)
        {
            Slot = string.Empty,
            IsEquipped = false
        };

        list.Add(item);
        _inventoryByCharacter[characterId] = list;

        return Task.FromResult(item);
    }

    public Task<InventoryItem?> EquipItemAsync(long characterId, long itemId, string slotName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_inventoryByCharacter.TryGetValue(characterId, out var items))
            return Task.FromResult<InventoryItem?>(null);

        var item = items.FirstOrDefault(x => x.Id == itemId && x.CharacterId == characterId);
        if (item is null)
            return Task.FromResult<InventoryItem?>(null);

        if (!string.IsNullOrWhiteSpace(slotName))
        {
            item.IsEquipped = true;
            item.Slot = slotName;
        }

        return Task.FromResult<InventoryItem?>(item);
    }
}
