namespace RF.Server.Core.Models;

public sealed class Account
{
    public long Id { get; init; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsBanned { get; set; }
    public DateTime? BanUntil { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public Account(long id, string username, string email, string passwordHash)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }
}

public enum AccountStatus
{
    Active = 0,
    Suspended = 1,
    Banned = 2
}
