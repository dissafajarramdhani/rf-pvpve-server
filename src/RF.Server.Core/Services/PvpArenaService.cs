using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class PvpArenaService
{
    public ArenaBattleResult ResolveBattle(Character attacker, Character defender, int attackerBaseDamage, int defenderBaseDamage)
    {
        if (attacker is null)
            throw new ArgumentNullException(nameof(attacker));

        if (defender is null)
            throw new ArgumentNullException(nameof(defender));

        if (attacker.Id == defender.Id)
            throw new InvalidOperationException("A character cannot duel itself.");

        var attackerDefense = defender.Vitality + Math.Max(1, defender.Agility / 2);
        var defenderDefense = attacker.Vitality + Math.Max(1, attacker.Agility / 2);

        var attackerDamage = Math.Max(1, attackerBaseDamage + attacker.Strength + attacker.Level - attackerDefense);
        var defenderDamage = Math.Max(1, defenderBaseDamage + defender.Strength + defender.Level - defenderDefense);

        var attackerRemainingHealth = Math.Max(0, attacker.Health - defenderDamage);
        var defenderRemainingHealth = Math.Max(0, defender.Health - attackerDamage);

        var winner = attackerRemainingHealth >= defenderRemainingHealth ? attacker : defender;
        var loser = ReferenceEquals(winner, attacker) ? defender : attacker;

        var rewardGold = winner == attacker ? 25 + attacker.Level * 3 : 25 + defender.Level * 3;
        var ratingDelta = winner == attacker ? 18 : -12;

        var summary = winner == attacker
            ? $"{attacker.Name} wins by outlasting {defender.Name} in the arena."
            : $"{defender.Name} wins by outlasting {attacker.Name} in the arena.";

        return new ArenaBattleResult(
            winner.Id,
            loser.Id,
            winner.Name,
            loser.Name,
            attackerDamage,
            defenderDamage,
            attackerRemainingHealth,
            defenderRemainingHealth,
            rewardGold,
            ratingDelta,
            summary);
    }
}

public sealed record ArenaBattleResult(
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
