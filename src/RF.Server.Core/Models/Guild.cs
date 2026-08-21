namespace RF.Server.Core.Models;

public sealed class Guild
{
    public long Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public long FounderAccountId { get; init; }
    public long FounderCharacterId { get; init; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Guild(long id, string name, string tag, long founderAccountId, long founderCharacterId)
    {
        Id = id;
        Name = name;
        Tag = tag;
        FounderAccountId = founderAccountId;
        FounderCharacterId = founderCharacterId;
    }
}
