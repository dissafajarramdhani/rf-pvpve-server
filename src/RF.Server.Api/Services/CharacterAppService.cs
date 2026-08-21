using RF.Server.Api.Data;
using RF.Server.Core.Models;
using RF.Server.Core.Services;

namespace RF.Server.Api.Services;

public sealed class CharacterAppService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly AntiCheatService _antiCheatService;

    public CharacterAppService(ICharacterRepository characterRepository, AntiCheatService antiCheatService)
    {
        _characterRepository = characterRepository;
        _antiCheatService = antiCheatService;
    }

    public async Task<IReadOnlyList<Character>> GetCharactersAsync(long accountId, CancellationToken cancellationToken = default)
    {
        return await _characterRepository.GetByAccountIdAsync(accountId, cancellationToken);
    }

    public async Task<Character> CreateCharacterAsync(long accountId, string classCode, string name, CancellationToken cancellationToken = default)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId), "Account id must be valid.");

        if (string.IsNullOrWhiteSpace(classCode))
            throw new ArgumentException("Class code is required.", nameof(classCode));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name is required.", nameof(name));

        return await _characterRepository.CreateAsync(accountId, classCode, name, cancellationToken);
    }

    public async Task<Character?> MoveCharacterAsync(long accountId, long characterId, double x, double y, double z, CancellationToken cancellationToken = default)
    {
        var character = await _characterRepository.GetByIdAsync(characterId, cancellationToken);
        if (character is null || character.AccountId != accountId)
        {
            return null;
        }

        var previousPosition = character.Position;
        var nextPosition = new WorldPosition(x, y, z);
        _antiCheatService.ValidateMovement(accountId, characterId, previousPosition, nextPosition);

        return await _characterRepository.UpdatePositionAsync(characterId, x, y, z, cancellationToken);
    }
}
