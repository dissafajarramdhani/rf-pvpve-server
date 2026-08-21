using RF.Server.Api.Data;
using RF.Server.Api.Models;
using RF.Server.Core.Services;

namespace RF.Server.Api.Services;

public sealed class ArenaAppService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly PvpArenaService _pvpArenaService;
    private readonly AntiCheatService _antiCheatService;

    public ArenaAppService(ICharacterRepository characterRepository, PvpArenaService pvpArenaService, AntiCheatService antiCheatService)
    {
        _characterRepository = characterRepository;
        _pvpArenaService = pvpArenaService;
        _antiCheatService = antiCheatService;
    }

    public async Task<ArenaMatchResponse> ResolveBattleAsync(long attackerCharacterId, long defenderCharacterId, int attackerBaseDamage, int defenderBaseDamage, CancellationToken cancellationToken = default)
    {
        var attacker = await _characterRepository.GetByIdAsync(attackerCharacterId, cancellationToken);
        var defender = await _characterRepository.GetByIdAsync(defenderCharacterId, cancellationToken);

        if (attacker is null || defender is null)
        {
            throw new InvalidOperationException("Both duel participants must exist.");
        }

        if (attacker.Id == defender.Id)
        {
            throw new InvalidOperationException("A character cannot duel itself.");
        }

        _antiCheatService.ValidateCombat(attackerCharacterId, defenderCharacterId, attackerBaseDamage);
        _antiCheatService.ValidateCombat(defenderCharacterId, attackerCharacterId, defenderBaseDamage);

        var result = _pvpArenaService.ResolveBattle(attacker, defender, attackerBaseDamage, defenderBaseDamage);

        return new ArenaMatchResponse(
            result.WinnerCharacterId,
            result.LoserCharacterId,
            result.WinnerName,
            result.LoserName,
            result.AttackerDamage,
            result.DefenderDamage,
            result.AttackerRemainingHealth,
            result.DefenderRemainingHealth,
            result.RewardGold,
            result.RatingDelta,
            result.Summary);
    }

    public ArenaRulesResponse GetRules()
    {
        return new ArenaRulesResponse(
            "PvPvE Arena",
            "1v1 Ranked Duel",
            true,
            true,
            2,
            "Gold and arena rating only; no gear power paywall.",
            "Combat is server-authoritative and rewards skill, not monetization.");
    }
}
