using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class CharacterService
{
    private readonly Dictionary<long, List<Character>> _charactersByAccount = new();
    private long _nextCharacterId = 1L;

    public Character CreateCharacter(long accountId, string classCode, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name is required.", nameof(name));

        if (!_charactersByAccount.TryGetValue(accountId, out var characters))
        {
            characters = new List<Character>();
            _charactersByAccount[accountId] = characters;
        }

        if (characters.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Character '{name}' already exists for this account.");

        var characterClass = CharacterClassCatalog.Get(classCode);
        var character = new Character(_nextCharacterId++, accountId, name, characterClass)
        {
            Position = new WorldPosition(0, 0, 0)
        };

        characters.Add(character);
        return character;
    }

    public IReadOnlyCollection<Character> GetCharactersForAccount(long accountId)
    {
        return _charactersByAccount.TryGetValue(accountId, out var characters)
            ? characters.AsReadOnly()
            : Array.Empty<Character>();
    }

    public Character? GetCharacter(long accountId, long characterId)
    {
        return _charactersByAccount.TryGetValue(accountId, out var characters)
            ? characters.FirstOrDefault(c => c.Id == characterId)
            : null;
    }
}
