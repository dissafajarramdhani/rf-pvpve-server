using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public sealed class InMemoryCharacterRepository : ICharacterRepository
{
    private readonly Dictionary<long, Character> _charactersById = new();
    private readonly Dictionary<long, List<Character>> _charactersByAccount = new();
    private long _nextCharacterId = 1;

    public Task<IReadOnlyList<Character>> GetByAccountIdAsync(long accountId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_charactersByAccount.TryGetValue(accountId, out var characters))
        {
            return Task.FromResult<IReadOnlyList<Character>>(characters.ToList().AsReadOnly());
        }

        return Task.FromResult<IReadOnlyList<Character>>(Array.Empty<Character>());
    }

    public Task<Character?> GetByIdAsync(long characterId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_charactersById.TryGetValue(characterId, out var character) ? character : null);
    }

    public Task<Character> CreateAsync(long accountId, string classCode, string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDefinition = CharacterClassCatalog.Get(classCode);
        var existingIds = _charactersByAccount.TryGetValue(accountId, out var characters) ? characters : new List<Character>();
        if (existingIds.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Character '{name}' already exists for account '{accountId}'.");
        }

        var character = new Character(_nextCharacterId++, accountId, name, classDefinition)
        {
            Position = new WorldPosition(0, 0, 0),
            Health = 100,
            Mana = 50,
            Level = 1
        };

        _charactersById[character.Id] = character;
        if (!_charactersByAccount.TryGetValue(accountId, out var list))
        {
            list = new List<Character>();
            _charactersByAccount[accountId] = list;
        }

        list.Add(character);
        return Task.FromResult(character);
    }

    public Task<Character?> UpdatePositionAsync(long characterId, double x, double y, double z, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_charactersById.TryGetValue(characterId, out var character))
        {
            return Task.FromResult<Character?>(null);
        }

        character.Position = new WorldPosition(x, y, z);
        return Task.FromResult<Character?>(character);
    }
}
