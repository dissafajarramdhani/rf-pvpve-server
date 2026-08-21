using RF.Server.Api.Models;
using RF.Server.Core.Models;
using RF.Server.Core.Services;

namespace RF.Server.Api.Services;

public sealed class CombatAppService
{
    private readonly CharacterAppService _characterAppService;
    private readonly CombatService _combatService;
    private readonly AntiCheatService _antiCheatService;

    public CombatAppService(CharacterAppService characterAppService, CombatService combatService, AntiCheatService antiCheatService)
    {
        _characterAppService = characterAppService;
        _combatService = combatService;
        _antiCheatService = antiCheatService;
    }

    public async Task<CombatResultResponse?> AttackAsync(long accountId, long characterId, string monsterName, int monsterMaxHealth, int monsterAttack, int monsterDefense, int baseDamage, CancellationToken cancellationToken = default)
    {
        var character = (await _characterAppService.GetCharactersAsync(accountId, cancellationToken))
            .FirstOrDefault(c => c.Id == characterId);

        if (character is null)
            return null;

        _antiCheatService.ValidateCombat(characterId, -1, baseDamage);

        var monster = new Monster(monsterName, monsterMaxHealth, monsterAttack, monsterDefense);
        var combatResult = _combatService.ResolveAttack(character, monster, baseDamage);

        return new CombatResultResponse(
            accountId,
            characterId,
            monster.Name,
            combatResult.Damage,
            combatResult.RemainingHealth,
            combatResult.TargetDefeated,
            combatResult.TargetDefeated ? "Target defeated." : "Attack landed.");
    }
}
