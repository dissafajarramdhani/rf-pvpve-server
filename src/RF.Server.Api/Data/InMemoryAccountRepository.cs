using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<long, Account> _accountsById = new();
    private readonly Dictionary<string, Account> _accountsByUsername = new(StringComparer.OrdinalIgnoreCase);
    private long _nextId = 1;

    public Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_accountsByUsername.TryGetValue(username, out var account) ? account : null);
    }

    public Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_accountsById.TryGetValue(id, out var account) ? account : null);
    }

    public Task<Account> CreateAsync(string username, string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_accountsByUsername.ContainsKey(username))
            throw new InvalidOperationException($"Account '{username}' already exists.");

        var account = new Account(_nextId++, username, email, passwordHash)
        {
            Status = AccountStatus.Active,
            IsBanned = false
        };

        _accountsById[account.Id] = account;
        _accountsByUsername[account.Username] = account;

        return Task.FromResult(account);
    }

    public Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_accountsByUsername.ContainsKey(username));
    }
}
