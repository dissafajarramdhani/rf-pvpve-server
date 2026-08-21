namespace RF.Server.Api.Models;

public sealed record CreateGuildRequest(long AccountId, long CharacterId, string Name, string Tag);
public sealed record JoinGuildRequest(long AccountId, long CharacterId, long GuildId);
public sealed record GuildResponse(long Id, string Name, string Tag, long FounderAccountId, long FounderCharacterId, int Level, int Experience, DateTime CreatedAt);
public sealed record GuildMemberResponse(long GuildId, long CharacterId, long AccountId, string CharacterName, string Role, DateTime JoinedAt);
public sealed record GuildInviteRequest(long AccountId, long CharacterId, long GuildId, long InviteeCharacterId);
