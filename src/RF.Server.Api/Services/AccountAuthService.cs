using System.Security.Cryptography;
using System.Text;
using RF.Server.Api.Data;
using RF.Server.Core.Models;

namespace RF.Server.Api.Services;

public sealed class AccountAuthService
{
    private readonly IAccountRepository _accountRepository;

    public AccountAuthService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        if (await _accountRepository.ExistsAsync(username, cancellationToken))
            throw new InvalidOperationException($"Account '{username}' already exists.");

        var passwordHash = HashPassword(password);
        return await _accountRepository.CreateAsync(username, email, passwordHash, cancellationToken);
    }

    public async Task<Account?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var existingUser = await _accountRepository.GetByUsernameAsync(username, cancellationToken);
        if (existingUser is null)
            return null;

        if (!VerifyPassword(password, existingUser.PasswordHash))
            return null;

        if (existingUser.IsBanned)
            return null;

        return existingUser;
    }

    public string IssueToken(Account account)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{account.Id}:{account.Username}:{DateTime.UtcNow:O}")));
    }

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
