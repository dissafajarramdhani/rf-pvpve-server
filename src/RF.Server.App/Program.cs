using RF.Server.Core.Models;
using RF.Server.Core.Services;

var authService = new AuthService();
var characterService = new CharacterService();
var worldService = new WorldService();
var combatService = new CombatService();

var account = authService.Register("player1", "player1@example.com", "password123");
var loggedInAccount = authService.Login("player1", "password123");

if (loggedInAccount is null)
{
    Console.WriteLine("Login failed.");
    return;
}

var character = characterService.CreateCharacter(loggedInAccount.Id, "warrior", "Ares");
worldService.SpawnCharacter(character, new WorldPosition(10, 0, 0));

Console.WriteLine($"Logged in as {loggedInAccount.Username}");
Console.WriteLine($"Created character: {character.Name} ({character.Class.Name}) at {character.Position.X}, {character.Position.Y}, {character.Position.Z}");

var wolf = new Monster("Wolf", 60, 8, 5);
var combatResult = combatService.ResolveAttack(character, wolf, 12);

Console.WriteLine($"Attack result: {combatResult.Damage} damage dealt to {wolf.Name}.");
Console.WriteLine($"Target remaining health: {combatResult.RemainingHealth}.");
Console.WriteLine($"Target defeated: {combatResult.TargetDefeated}");

var moved = worldService.MoveCharacter(character, 25, 0, 0);
Console.WriteLine($"Position updated: {moved.X}, {moved.Y}, {moved.Z}");
