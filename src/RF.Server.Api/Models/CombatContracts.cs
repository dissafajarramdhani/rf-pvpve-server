namespace RF.Server.Api.Models;

public sealed record AttackRequest(long AccountId, long CharacterId, string MonsterName, int MonsterMaxHealth, int MonsterAttack, int MonsterDefense, int BaseDamage);
public sealed record CombatResultResponse(long AccountId, long CharacterId, string MonsterName, int Damage, int RemainingHealth, bool Defeated, string? StatusMessage);
