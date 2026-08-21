using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public interface ICharacterRepository
{
    Task<IReadOnlyList<Character>> GetByAccountIdAsync(long accountId, CancellationToken cancellationToken = default);
    Task<Character?> GetByIdAsync(long characterId, CancellationToken cancellationToken = default);
    Task<Character> CreateAsync(long accountId, string classCode, string name, CancellationToken cancellationToken = default);
    Task<Character?> UpdatePositionAsync(long characterId, double x, double y, double z, CancellationToken cancellationToken = default);
}
