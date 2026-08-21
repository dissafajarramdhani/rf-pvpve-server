namespace RF.Server.Core.Models;

public sealed class Monster
{
    public string Name { get; init; }
    public int MaxHealth { get; init; }
    public int CurrentHealth { get; set; }
    public int Attack { get; init; }
    public int Defense { get; init; }
    public int MinLevel { get; init; }
    public int DangerLevel { get; init; }
    public int GoldReward { get; init; }
    public int ExperienceReward { get; init; }
    public string Description { get; init; }
    public bool IsAlive => CurrentHealth > 0;

    public Monster(string name, int maxHealth, int attack, int defense)
        : this(name, maxHealth, attack, defense, 1, 1, 0, 0, string.Empty)
    {
    }

    public Monster(string name, int maxHealth, int attack, int defense, int minLevel, int dangerLevel, int goldReward, int experienceReward, string description)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Attack = attack;
        Defense = defense;
        MinLevel = minLevel;
        DangerLevel = dangerLevel;
        GoldReward = goldReward;
        ExperienceReward = experienceReward;
        Description = description;
    }
}
