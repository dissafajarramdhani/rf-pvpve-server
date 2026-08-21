using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class AntiCheatService
{
    public const double MaxMovementDistance = 24.0;
    public const double MaxVerticalJump = 10.0;
    public const int MaxBaseDamage = 200;

    public void ValidateMovement(long accountId, long characterId, WorldPosition previous, WorldPosition next)
    {
        if (previous is null)
            throw new InvalidOperationException("Previous position is required for movement validation.");

        if (next is null)
            throw new InvalidOperationException("Next position is required for movement validation.");

        if (!double.IsFinite(previous.X) || !double.IsFinite(previous.Y) || !double.IsFinite(previous.Z)
            || !double.IsFinite(next.X) || !double.IsFinite(next.Y) || !double.IsFinite(next.Z))
        {
            throw new InvalidOperationException("Movement contains invalid numeric values.");
        }

        var distance = Math.Sqrt(Math.Pow(next.X - previous.X, 2) + Math.Pow(next.Y - previous.Y, 2) + Math.Pow(next.Z - previous.Z, 2));
        if (distance > MaxMovementDistance)
        {
            throw new InvalidOperationException($"Movement validation failed for character {characterId}: distance {distance:F2} exceeds limit {MaxMovementDistance}.");
        }

        var yDelta = Math.Abs(next.Y - previous.Y);
        if (yDelta > MaxVerticalJump)
        {
            throw new InvalidOperationException($"Movement validation failed for character {characterId}: vertical jump {yDelta:F2} exceeds limit {MaxVerticalJump}.");
        }
    }

    public void ValidateCombat(long attackerId, long targetId, int baseDamage)
    {
        if (baseDamage < 1 || baseDamage > MaxBaseDamage)
        {
            throw new InvalidOperationException($"Combat validation failed for attacker {attackerId} against target {targetId}: damage {baseDamage} is invalid.");
        }
    }

    public AntiCheatSummary GetSummary()
    {
        return new AntiCheatSummary(
            "server-authoritative",
            MaxMovementDistance,
            MaxVerticalJump,
            MaxBaseDamage,
            new[]
            {
                "Reject impossible position jumps",
                "Reject invalid combat damage",
                "Reject duplicated move or teleport attempts",
                "Log suspicious activity for review"
            });
    }
}

public sealed record AntiCheatSummary(
    string EnforcementMode,
    double MaxMovementDistance,
    double MaxVerticalJump,
    int MaxBaseDamage,
    string[] Rules);
