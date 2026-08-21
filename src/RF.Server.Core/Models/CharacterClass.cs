namespace RF.Server.Core.Models;

public sealed class CharacterClass
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int BaseStrength { get; init; }
    public int BaseIntelligence { get; init; }
    public int BaseVitality { get; init; }
    public int BaseAgility { get; init; }

    public CharacterClass(string code, string name, int baseStrength, int baseIntelligence, int baseVitality, int baseAgility)
    {
        Code = code;
        Name = name;
        BaseStrength = baseStrength;
        BaseIntelligence = baseIntelligence;
        BaseVitality = baseVitality;
        BaseAgility = baseAgility;
    }
}

public static class CharacterClassCatalog
{
    public static readonly CharacterClass Warrior = new("warrior", "Warrior", 12, 4, 10, 6);
    public static readonly CharacterClass Mage = new("mage", "Mage", 4, 12, 7, 8);
    public static readonly CharacterClass Archer = new("archer", "Archer", 6, 5, 8, 12);

    public static CharacterClass Get(string code) => code.ToLowerInvariant() switch
    {
        "warrior" => Warrior,
        "mage" => Mage,
        "archer" => Archer,
        _ => throw new ArgumentOutOfRangeException(nameof(code), $"Unsupported class code: {code}")
    };
}
