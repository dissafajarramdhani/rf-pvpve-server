using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class WorldService
{
    public WorldPosition SpawnCharacter(Character character, WorldPosition position)
    {
        character.Position = position;
        return character.Position;
    }

    public WorldPosition MoveCharacter(Character character, double x, double y, double z)
    {
        character.Position = new WorldPosition(x, y, z);
        return character.Position;
    }
}
