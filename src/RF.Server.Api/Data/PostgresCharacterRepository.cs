using Npgsql;
using RF.Server.Core.Models;

namespace RF.Server.Api.Data;

public sealed class PostgresCharacterRepository : ICharacterRepository
{
    private readonly string _connectionString;
    private readonly InMemoryCharacterRepository _fallback = new();

    public PostgresCharacterRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Character>> GetByAccountIdAsync(long accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureSchemaAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                @"SELECT id, account_id, name, class_code, level, exp, health, mana, strength, intelligence, vitality, agility, pos_x, pos_y, pos_z
                  FROM characters WHERE account_id = @accountId ORDER BY id",
                connection);
            command.Parameters.AddWithValue("accountId", accountId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var results = new List<Character>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var classCode = reader.GetString(3);
                var character = new Character(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2), CharacterClassCatalog.Get(classCode))
                {
                    Level = reader.GetInt32(4),
                    Exp = reader.GetInt64(5),
                    Health = reader.GetInt32(6),
                    Mana = reader.GetInt32(7),
                    Strength = reader.GetInt32(8),
                    Intelligence = reader.GetInt32(9),
                    Vitality = reader.GetInt32(10),
                    Agility = reader.GetInt32(11),
                    Position = new WorldPosition(reader.GetDouble(12), reader.GetDouble(13), reader.GetDouble(14))
                };
                results.Add(character);
            }

            return results.AsReadOnly();
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.GetByAccountIdAsync(accountId, cancellationToken);
        }
    }

    public async Task<Character?> GetByIdAsync(long characterId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureSchemaAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                @"SELECT id, account_id, name, class_code, level, exp, health, mana, strength, intelligence, vitality, agility, pos_x, pos_y, pos_z
                  FROM characters WHERE id = @characterId LIMIT 1",
                connection);
            command.Parameters.AddWithValue("characterId", characterId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var classCode = reader.GetString(3);
            return new Character(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2), CharacterClassCatalog.Get(classCode))
            {
                Level = reader.GetInt32(4),
                Exp = reader.GetInt64(5),
                Health = reader.GetInt32(6),
                Mana = reader.GetInt32(7),
                Strength = reader.GetInt32(8),
                Intelligence = reader.GetInt32(9),
                Vitality = reader.GetInt32(10),
                Agility = reader.GetInt32(11),
                Position = new WorldPosition(reader.GetDouble(12), reader.GetDouble(13), reader.GetDouble(14))
            };
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.GetByIdAsync(characterId, cancellationToken);
        }
    }

    public async Task<Character> CreateAsync(long accountId, string classCode, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureSchemaAsync(connection, cancellationToken);

            var classDefinition = CharacterClassCatalog.Get(classCode);
            await using var command = new NpgsqlCommand(
                @"INSERT INTO characters (account_id, name, class_code, level, exp, health, mana, strength, intelligence, vitality, agility, pos_x, pos_y, pos_z)
                  VALUES (@accountId, @name, @classCode, @level, @exp, @health, @mana, @strength, @intelligence, @vitality, @agility, 0, 0, 0)
                  RETURNING id",
                connection);
            command.Parameters.AddWithValue("accountId", accountId);
            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("classCode", classDefinition.Code);
            command.Parameters.AddWithValue("level", 1);
            command.Parameters.AddWithValue("exp", 0L);
            command.Parameters.AddWithValue("health", 100);
            command.Parameters.AddWithValue("mana", 50);
            command.Parameters.AddWithValue("strength", classDefinition.BaseStrength);
            command.Parameters.AddWithValue("intelligence", classDefinition.BaseIntelligence);
            command.Parameters.AddWithValue("vitality", classDefinition.BaseVitality);
            command.Parameters.AddWithValue("agility", classDefinition.BaseAgility);

            var id = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new InvalidOperationException("No character id returned."));
            return new Character(id, accountId, name, classDefinition)
            {
                Health = 100,
                Mana = 50,
                Strength = classDefinition.BaseStrength,
                Intelligence = classDefinition.BaseIntelligence,
                Vitality = classDefinition.BaseVitality,
                Agility = classDefinition.BaseAgility
            };
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.CreateAsync(accountId, classCode, name, cancellationToken);
        }
    }

    public async Task<Character?> UpdatePositionAsync(long characterId, double x, double y, double z, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureSchemaAsync(connection, cancellationToken);

            await using var command = new NpgsqlCommand(
                @"UPDATE characters SET pos_x = @x, pos_y = @y, pos_z = @z WHERE id = @characterId RETURNING id, account_id, name, class_code, level, exp, health, mana, strength, intelligence, vitality, agility, pos_x, pos_y, pos_z",
                connection);
            command.Parameters.AddWithValue("characterId", characterId);
            command.Parameters.AddWithValue("x", x);
            command.Parameters.AddWithValue("y", y);
            command.Parameters.AddWithValue("z", z);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var classCode = reader.GetString(3);
            return new Character(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2), CharacterClassCatalog.Get(classCode))
            {
                Level = reader.GetInt32(4),
                Exp = reader.GetInt64(5),
                Health = reader.GetInt32(6),
                Mana = reader.GetInt32(7),
                Strength = reader.GetInt32(8),
                Intelligence = reader.GetInt32(9),
                Vitality = reader.GetInt32(10),
                Agility = reader.GetInt32(11),
                Position = new WorldPosition(reader.GetDouble(12), reader.GetDouble(13), reader.GetDouble(14))
            };
        }
        catch (Exception ex) when (IsDbUnavailable(ex))
        {
            return await _fallback.UpdatePositionAsync(characterId, x, y, z, cancellationToken);
        }
    }

    private static async Task EnsureSchemaAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            @"CREATE TABLE IF NOT EXISTS characters (
                id BIGSERIAL PRIMARY KEY,
                account_id BIGINT NOT NULL,
                name VARCHAR(64) NOT NULL,
                class_code VARCHAR(32) NOT NULL,
                level INTEGER NOT NULL DEFAULT 1,
                exp BIGINT NOT NULL DEFAULT 0,
                health INTEGER NOT NULL DEFAULT 100,
                mana INTEGER NOT NULL DEFAULT 50,
                strength INTEGER NOT NULL DEFAULT 0,
                intelligence INTEGER NOT NULL DEFAULT 0,
                vitality INTEGER NOT NULL DEFAULT 0,
                agility INTEGER NOT NULL DEFAULT 0,
                pos_x DOUBLE PRECISION NOT NULL DEFAULT 0,
                pos_y DOUBLE PRECISION NOT NULL DEFAULT 0,
                pos_z DOUBLE PRECISION NOT NULL DEFAULT 0,
                UNIQUE (account_id, name)
             )",
            connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static bool IsDbUnavailable(Exception ex)
    {
        return ex is NpgsqlException or TimeoutException or InvalidOperationException;
    }
}
