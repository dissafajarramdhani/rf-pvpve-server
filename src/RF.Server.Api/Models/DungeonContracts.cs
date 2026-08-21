namespace RF.Server.Api.Models;

public sealed class DungeonRoomResponse
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RecommendedLevel { get; set; }
    public int MonsterCount { get; set; }
    public List<string> MonsterNames { get; set; } = new();
}

public sealed class DungeonEncounterRequest
{
    public string RoomName { get; set; } = string.Empty;
}

public sealed class DungeonEncounterResponse
{
    public string RoomName { get; set; } = string.Empty;
    public string MonsterName { get; set; } = string.Empty;
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int GoldReward { get; set; }
    public int ExperienceReward { get; set; }
    public string Description { get; set; } = string.Empty;
}
