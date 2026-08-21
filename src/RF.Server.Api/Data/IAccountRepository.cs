using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public interface IAccountRepository
{
    Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Account> CreateAsync(string username, string email, string passwordHash, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);
}
