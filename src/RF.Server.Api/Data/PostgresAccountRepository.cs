using Npgsql;
using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public sealed class PostgresAccountRepository : IAccountRepository
{
    private readonly string _connectionString;
    private readonly InMemoryAccountRepository _fallback = new();

    public PostgresAccountRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT id, username, email, password_hash, status, is_banned, ban_until, last_login_at FROM accounts WHERE username = @username LIMIT 1",
                connection);
            command.Parameters.AddWithValue("username", username);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var account = new Account(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3));

            account.Status = Enum.TryParse<AccountStatus>(reader.GetString(4), true, out var status)
                ? status
                : AccountStatus.Active;
            account.IsBanned = reader.GetBoolean(5);
            account.BanUntil = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
            account.LastLoginAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7);

            return account;
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.GetByUsernameAsync(username, cancellationToken);
        }
    }

    public async Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT id, username, email, password_hash, status, is_banned, ban_until, last_login_at FROM accounts WHERE id = @id LIMIT 1",
                connection);
            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var account = new Account(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3));

            account.Status = Enum.TryParse<AccountStatus>(reader.GetString(4), true, out var status)
                ? status
                : AccountStatus.Active;
            account.IsBanned = reader.GetBoolean(5);
            account.BanUntil = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
            account.LastLoginAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7);

            return account;
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.GetByIdAsync(id, cancellationToken);
        }
    }

    public async Task<Account> CreateAsync(string username, string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(
                "INSERT INTO accounts (username, email, password_hash, status, is_banned, created_at) VALUES (@username, @email, @password_hash, @status, false, NOW()) RETURNING id",
                connection);
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("password_hash", passwordHash);
            command.Parameters.AddWithValue("status", AccountStatus.Active.ToString());

            var id = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new InvalidOperationException("No account id returned."));
            return new Account(id, username, email, passwordHash)
            {
                Status = AccountStatus.Active,
                IsBanned = false
            };
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.CreateAsync(username, email, passwordHash, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(
                "SELECT EXISTS(SELECT 1 FROM accounts WHERE username = @username)",
                connection);
            command.Parameters.AddWithValue("username", username);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is bool exists && exists || result is string s && bool.TryParse(s, out var value) && value;
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.ExistsAsync(username, cancellationToken);
        }
    }

    private static bool IsDbUnavailable(Exception ex)
    {
        return ex is NpgsqlException or TimeoutException or InvalidOperationException;
    }
}
