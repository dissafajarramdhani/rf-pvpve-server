namespace RF.Server.Core.Models;

public sealed class GuildMember
{
    public long GuildId { get; init; }
    public long CharacterId { get; init; }
    public long AccountId { get; init; }
    public string CharacterName { get; init; } = string.Empty;
    public GuildRole Role { get; set; } = GuildRole.Member;
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;

    public GuildMember(long guildId, long characterId, long accountId, string characterName)
    {
        GuildId = guildId;
        CharacterId = characterId;
        AccountId = accountId;
        CharacterName = characterName;
    }
}

public enum GuildRole
{
    Leader = 0,
    Officer = 1,
    Member = 2
}
