namespace RF.Server.Api.Models;

public sealed record ArenaMatchRequest(long AttackerCharacterId, long DefenderCharacterId, int AttackerBaseDamage, int DefenderBaseDamage);

public sealed record ArenaMatchResponse(
    long WinnerCharacterId,
    long LoserCharacterId,
    string WinnerName,
    string LoserName,
    int AttackerDamage,
    int DefenderDamage,
    int AttackerRemainingHealth,
    int DefenderRemainingHealth,
    int RewardGold,
    int RatingDelta,
    string Summary);

public sealed record ArenaRulesResponse(
    string ArenaName,
    string MatchMode,
    bool IsServerAuthoritative,
    bool IsNoPayToWin,
    int MaxParticipants,
    string RewardPolicy,
    string Notes);
