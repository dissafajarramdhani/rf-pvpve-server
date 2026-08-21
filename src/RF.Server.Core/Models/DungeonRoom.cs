namespace RF.Server.Core.Models;

public sealed class DungeonRoom
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int RecommendedLevel { get; init; }
    public int MonsterCount { get; init; }
    public string[] MonsterNames { get; init; }

    public DungeonRoom(string name, string description, int recommendedLevel, int monsterCount, params string[] monsterNames)
    {
        Name = name;
        Description = description;
        RecommendedLevel = recommendedLevel;
        MonsterCount = monsterCount;
        MonsterNames = monsterNames;
    }
}
