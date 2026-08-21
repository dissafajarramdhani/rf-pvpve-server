namespace RF.Server.Core.Models;

public sealed class Character
{
    public long Id { get; init; }
    public long AccountId { get; init; }
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
    public int Level { get; set; } = 1;
    public long Exp { get; set; }
    public int Health { get; set; } = 100;
    public int Mana { get; set; } = 50;
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Vitality { get; set; }
    public int Agility { get; set; }
    public WorldPosition Position { get; set; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Character(long id, long accountId, string name, CharacterClass characterClass)
    {
        Id = id;
        AccountId = accountId;
        Name = name;
        Class = characterClass;
        Strength = characterClass.BaseStrength;
        Intelligence = characterClass.BaseIntelligence;
        Vitality = characterClass.BaseVitality;
        Agility = characterClass.BaseAgility;
    }
}
