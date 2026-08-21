using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class CombatService
{
    public CombatResult ResolveAttack(Character attacker, Monster target, int baseDamage)
    {
        var rawDamage = baseDamage + attacker.Strength - target.Defense;
        var damage = Math.Max(1, rawDamage);

        target.CurrentHealth = Math.Max(0, target.CurrentHealth - damage);

        return new CombatResult(target.Name, damage, target.IsAlive == false, target.CurrentHealth);
    }
}

public sealed record CombatResult(
    string TargetName,
    int Damage,
    bool TargetDefeated,
    int RemainingHealth);
