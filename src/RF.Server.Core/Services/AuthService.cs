using System.Security.Cryptography;
using System.Text;
using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class AuthService
{
    private readonly Dictionary<long, Account> _accountsById = new();
    private readonly Dictionary<string, Account> _accountsByUsername = new(StringComparer.OrdinalIgnoreCase);
    private long _nextAccountId = 1L;

    public Account Register(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        if (_accountsByUsername.ContainsKey(username))
            throw new InvalidOperationException($"Account '{username}' already exists.");

        var account = new Account(_nextAccountId++, username, email, HashPassword(password));
        _accountsById[account.Id] = account;
        _accountsByUsername[account.Username] = account;

        return account;
    }

    public Account? Login(string username, string password)
    {
        if (!_accountsByUsername.TryGetValue(username, out var account))
            return null;

        if (!VerifyPassword(password, account.PasswordHash))
            return null;

        if (account.IsBanned)
            return null;

        account.LastLoginAt = DateTime.UtcNow;
        return account;
    }

    public Account? GetAccount(long id) => _accountsById.TryGetValue(id, out var account) ? account : null;

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return HashPassword(password) == passwordHash;
    }
}
