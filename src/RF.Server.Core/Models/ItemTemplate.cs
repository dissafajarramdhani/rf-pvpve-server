namespace RF.Server.Core.Models;

public sealed class ItemTemplate
{
    public long Id { get; init; }
    public string Code { get; init; }
    public string Name { get; init; }
    public string ItemType { get; init; }
    public string Rarity { get; init; }
    public int LevelRequirement { get; init; }
    public int MaxStack { get; init; }
    public int AttackBonus { get; init; }
    public int DefenseBonus { get; init; }
    public int HealthBonus { get; init; }

    public ItemTemplate(long id, string code, string name, string itemType, string rarity, int levelRequirement, int maxStack, int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0)
    {
        Id = id;
        Code = code;
        Name = name;
        ItemType = itemType;
        Rarity = rarity;
        LevelRequirement = levelRequirement;
        MaxStack = maxStack;
        AttackBonus = attackBonus;
        DefenseBonus = defenseBonus;
        HealthBonus = healthBonus;
    }
}

public static class ItemCatalog
{
    private static readonly List<ItemTemplate> Templates =
    [
        new ItemTemplate(1, "iron_sword", "Iron Sword", "weapon", "common", 1, 1, 8, 0, 0),
        new ItemTemplate(2, "leather_armor", "Leather Armor", "armor", "common", 1, 1, 0, 5, 10),
        new ItemTemplate(3, "healing_potion", "Healing Potion", "consumable", "common", 1, 10, 0, 0, 25),
        new ItemTemplate(4, "steel_axe", "Steel Axe", "weapon", "rare", 5, 1, 15, 0, 0),
        new ItemTemplate(5, "guardian_vest", "Guardian Vest", "armor", "rare", 5, 1, 0, 12, 20)
    ];

    public static IReadOnlyList<ItemTemplate> All => Templates;

    public static ItemTemplate Get(string code) => Templates.FirstOrDefault(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentOutOfRangeException(nameof(code), $"Unknown item code: {code}");

    public static ItemTemplate GetById(long id) => Templates.FirstOrDefault(x => x.Id == id)
        ?? throw new ArgumentOutOfRangeException(nameof(id), $"Unknown item id: {id}");
}
