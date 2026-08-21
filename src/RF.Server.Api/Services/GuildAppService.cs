using RF.Server.Api.Data;
using RF.Server.Api.Models;
using RF.Server.Core.Models;

namespace RF.Server.Api.Services;

public sealed class GuildAppService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly GuildRepository _guildRepository;

    public GuildAppService(ICharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
        _guildRepository = new GuildRepository();
    }

    public GuildResponse CreateGuild(long accountId, long characterId, string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Guild name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Guild tag is required.", nameof(tag));

        var character = _characterRepository.GetByIdAsync(characterId).GetAwaiter().GetResult();
        if (character is null || character.AccountId != accountId)
            throw new InvalidOperationException("Character not found for the account.");

        var guild = _guildRepository.CreateGuild(name, tag, accountId, characterId);
        _guildRepository.AddMember(guild.Id, characterId, accountId, character.Name, GuildRole.Leader);

        return new GuildResponse(
            guild.Id,
            guild.Name,
            guild.Tag,
            guild.FounderAccountId,
            guild.FounderCharacterId,
            guild.Level,
            guild.Experience,
            guild.CreatedAt);
    }

    public GuildResponse? JoinGuild(long accountId, long characterId, long guildId)
    {
        var character = _characterRepository.GetByIdAsync(characterId).GetAwaiter().GetResult();
        if (character is null || character.AccountId != accountId)
            return null;

        var guild = _guildRepository.GetGuild(guildId);
        if (guild is null)
            return null;

        if (_guildRepository.HasMember(guildId, characterId))
            throw new InvalidOperationException("Character is already a member of this guild.");

        _guildRepository.AddMember(guildId, characterId, accountId, character.Name, GuildRole.Member);
        return new GuildResponse(
            guild.Id,
            guild.Name,
            guild.Tag,
            guild.FounderAccountId,
            guild.FounderCharacterId,
            guild.Level,
            guild.Experience,
            guild.CreatedAt);
    }

    public IReadOnlyList<GuildMemberResponse> GetMembers(long guildId)
    {
        return _guildRepository.GetMembers(guildId)
            .Select(m => new GuildMemberResponse(m.GuildId, m.CharacterId, m.AccountId, m.CharacterName, m.Role.ToString(), m.JoinedAt))
            .ToList();
    }

    public GuildResponse? GetGuild(long guildId)
    {
        var guild = _guildRepository.GetGuild(guildId);
        if (guild is null)
            return null;

        return new GuildResponse(
            guild.Id,
            guild.Name,
            guild.Tag,
            guild.FounderAccountId,
            guild.FounderCharacterId,
            guild.Level,
            guild.Experience,
            guild.CreatedAt);
    }
}

public sealed class GuildRepository
{
    private readonly Dictionary<long, Guild> _guilds = new();
    private readonly Dictionary<long, List<GuildMember>> _members = new();
    private long _nextGuildId = 1;

    public Guild CreateGuild(string name, string tag, long founderAccountId, long founderCharacterId)
    {
        var guild = new Guild(_nextGuildId++, name.Trim(), tag.Trim(), founderAccountId, founderCharacterId)
        {
            Name = name.Trim(),
            Tag = tag.Trim()
        };

        _guilds[guild.Id] = guild;
        _members[guild.Id] = new List<GuildMember>();
        return guild;
    }

    public Guild? GetGuild(long guildId) => _guilds.TryGetValue(guildId, out var guild) ? guild : null;

    public void AddMember(long guildId, long characterId, long accountId, string characterName, GuildRole role)
    {
        if (!_members.ContainsKey(guildId))
            _members[guildId] = new List<GuildMember>();

        _members[guildId].Add(new GuildMember(guildId, characterId, accountId, characterName)
        {
            Role = role
        });
    }

    public IReadOnlyList<GuildMember> GetMembers(long guildId)
    {
        return _members.TryGetValue(guildId, out var members)
            ? members.AsReadOnly()
            : Array.Empty<GuildMember>();
    }

    public bool HasMember(long guildId, long characterId)
    {
        return _members.TryGetValue(guildId, out var members) && members.Any(m => m.CharacterId == characterId);
    }
}
