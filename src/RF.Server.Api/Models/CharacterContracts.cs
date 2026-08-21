namespace RF.Server.Api.Models;

public sealed record CreateCharacterRequest(long AccountId, string ClassCode, string Name);
public sealed record CharacterResponse(long Id, long AccountId, string Name, string ClassName, int Level, double X, double Y, double Z, int Health, int Mana);
public sealed record MoveCharacterRequest(long AccountId, long CharacterId, double X, double Y, double Z);
public sealed record WorldPositionResponse(long CharacterId, double X, double Y, double Z);
