namespace RF.Server.Core.Models;

public sealed class Monster
{
    public string Name { get; init; }
    public int MaxHealth { get; init; }
    public int CurrentHealth { get; set; }
    public int Attack { get; init; }
    public int Defense { get; init; }
    public bool IsAlive => CurrentHealth > 0;

    public Monster(string name, int maxHealth, int attack, int defense)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Attack = attack;
        Defense = defense;
    }
}
