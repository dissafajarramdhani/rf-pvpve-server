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

    private readonly Dictionary<string, Monster> _bosses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Forest Cleft"] = new Monster("Ancient Stalker", 82, 20, 11, 3, 2, 35, 18, "Alpha predator of the grove."),
        ["Ancient Crypt"] = new Monster("Grave Warden", 108, 26, 15, 4, 3, 60, 34, "A restless sentinel guarding the tomb."),
        ["Ashen Mine"] = new Monster("Forge Titan", 145, 34, 20, 5, 4, 95, 58, "A giant miner fused with volcanic metal.")
    };

    private readonly Dictionary<string, List<LootDrop>> _bossLoot = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Forest Cleft"] =
        [
            new LootDrop("iron_sword", "Iron Sword", "common", 0.65, 1, 1),
            new LootDrop("leather_armor", "Leather Armor", "common", 0.5, 1, 1),
            new LootDrop("steel_axe", "Steel Axe", "rare", 0.25, 1, 1),
            new LootDrop("healing_potion", "Healing Potion", "common", 0.9, 1, 3)
        ],
        ["Ancient Crypt"] =
        [
            new LootDrop("steel_axe", "Steel Axe", "rare", 0.7, 1, 1),
            new LootDrop("guardian_vest", "Guardian Vest", "rare", 0.35, 1, 1),
            new LootDrop("healing_potion", "Healing Potion", "common", 0.85, 2, 4),
            new LootDrop("iron_sword", "Iron Sword", "common", 0.5, 1, 1)
        ],
        ["Ashen Mine"] =
        [
            new LootDrop("guardian_vest", "Guardian Vest", "rare", 0.75, 1, 1),
            new LootDrop("steel_axe", "Steel Axe", "rare", 0.5, 1, 1),
            new LootDrop("healing_potion", "Healing Potion", "common", 0.9, 2, 5),
            new LootDrop("leather_armor", "Leather Armor", "common", 0.55, 1, 1)
        ]
    };

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

    public Monster GetBossForRoom(string roomName)
    {
        if (_bosses.TryGetValue(roomName, out var boss))
            return boss;

        throw new InvalidOperationException($"Boss for room '{roomName}' not found.");
    }

    public IReadOnlyList<LootDrop> GetLootTable(string roomName)
    {
        if (_bossLoot.TryGetValue(roomName, out var loot))
            return loot.AsReadOnly();

        return Array.Empty<LootDrop>();
    }

    public (int gold, int xp, string itemCode, string itemName, string rarity) ResolveBossClear(string roomName, int characterLevel)
    {
        var room = GetRoomByName(roomName);
        var boss = GetBossForRoom(roomName);
        var gold = boss.GoldReward + Math.Max(0, characterLevel * 4);
        var xp = boss.ExperienceReward + Math.Max(0, characterLevel * 8);

        var loot = GetLootTable(roomName);
        string itemCode = string.Empty;
        string itemName = string.Empty;
        string rarity = string.Empty;

        if (loot.Count > 0)
        {
            var roll = _random.NextDouble();
            double cumulative = 0;
            foreach (var drop in loot)
            {
                cumulative += drop.DropChance;
                if (roll <= cumulative)
                {
                    itemCode = drop.ItemCode;
                    itemName = drop.ItemName;
                    rarity = drop.Rarity;
                    break;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(itemCode) && loot.Count > 0)
        {
            var fallback = loot[_random.Next(loot.Count)];
            itemCode = fallback.ItemCode;
            itemName = fallback.ItemName;
            rarity = fallback.Rarity;
        }

        return (gold, xp, itemCode, itemName, rarity);
    }
}
