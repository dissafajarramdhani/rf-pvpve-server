using RF.Server.Core.Models;

namespace RF.Server.Core.Services;

public sealed class DungeonService
{
    private readonly Random _random = new();

    private readonly List<DungeonRoom> _rooms =
    [
        new("Forest Cleft", "A dark glade overrun by beasts.", 1, 3, "Wolf", "Slime", "Boar"),
        new("Ancient Crypt", "An abandoned shrine with restless undead.", 5, 4, "Skeleton", "Ghost", "Zombie", "Wraith"),
        new("Ashen Mine", "A dangerous mining tunnel full of monsters.", 10, 5, "Goblin", "Bat", "Spider", "Mineral Golem", "Wisp")
    ];

    public IReadOnlyList<DungeonRoom> GetAvailableRooms() => _rooms.AsReadOnly();

    public DungeonRoom GetRoomByName(string name)
    {
        return _rooms.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Dungeon room '{name}' not found.");
    }

    public Monster GenerateEncounter(string roomName)
    {
        var room = GetRoomByName(roomName);
        var monsterName = room.MonsterNames[_random.Next(room.MonsterNames.Length)];

        return monsterName switch
        {
            "Wolf" => new Monster("Wolf", 25, 12, 7, 2, 1, 10, 3, "Ferocious hunter"),
            "Slime" => new Monster("Slime", 18, 8, 5, 1, 1, 7, 2, "Sticky blob"),
            "Boar" => new Monster("Boar", 30, 15, 9, 2, 1, 12, 4, "Charging tusker"),
            "Skeleton" => new Monster("Skeleton", 35, 18, 10, 2, 2, 14, 5, "Bone warrior"),
            "Ghost" => new Monster("Ghost", 30, 14, 12, 3, 2, 13, 6, "Phantom warden"),
            "Zombie" => new Monster("Zombie", 40, 16, 11, 2, 2, 15, 5, "Slow but relentless"),
            "Wraith" => new Monster("Wraith", 42, 20, 15, 3, 3, 18, 7, "Spectral terror"),
            "Goblin" => new Monster("Goblin", 28, 11, 7, 2, 1, 11, 3, "Trickster raider"),
            "Bat" => new Monster("Bat", 22, 10, 6, 2, 1, 10, 3, "Winged ambusher"),
            "Spider" => new Monster("Spider", 32, 14, 8, 2, 2, 12, 4, "Venomous crawler"),
            "Mineral Golem" => new Monster("Mineral Golem", 55, 22, 15, 3, 2, 20, 7, "Stone guardian"),
            "Wisp" => new Monster("Wisp", 26, 13, 13, 4, 2, 12, 6, "Arcane flicker"),
            _ => new Monster(monsterName, 30, 12, 10, 2, 2, 12, 5, "Dungeon beast")
        };
    }
}
