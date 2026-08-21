using RF.Server.Api.Models;
using RF.Server.Core.Models;
using RF.Server.Core.Services;

namespace RF.Server.Api.Services;

public sealed class DungeonAppService
{
    private readonly DungeonService _dungeonService;

    public DungeonAppService(DungeonService dungeonService)
    {
        _dungeonService = dungeonService;
    }

    public List<DungeonRoomResponse> GetRooms()
    {
        return _dungeonService
            .GetAvailableRooms()
            .Select(room => new DungeonRoomResponse
            {
                Name = room.Name,
                Description = room.Description,
                RecommendedLevel = room.RecommendedLevel,
                MonsterCount = room.MonsterCount,
                MonsterNames = room.MonsterNames.ToList()
            })
            .ToList();
    }

    public DungeonEncounterResponse GetEncounter(string roomName)
    {
        var room = _dungeonService.GetRoomByName(roomName);
        var monster = _dungeonService.GenerateEncounter(room.Name);

        return new DungeonEncounterResponse
        {
            RoomName = room.Name,
            MonsterName = monster.Name,
            Health = monster.MaxHealth,
            Attack = monster.Attack,
            Defense = monster.Defense,
            GoldReward = monster.GoldReward,
            ExperienceReward = monster.ExperienceReward,
            Description = monster.Description
        };
    }

    public DungeonBossResponse GetBoss(string roomName)
    {
        var room = _dungeonService.GetRoomByName(roomName);
        var boss = _dungeonService.GetBossForRoom(room.Name);
        var lootTable = _dungeonService.GetLootTable(room.Name).Select(x => x.ItemName).ToList();

        return new DungeonBossResponse
        {
            RoomName = room.Name,
            BossName = boss.Name,
            Description = boss.Description,
            Health = boss.MaxHealth,
            Attack = boss.Attack,
            Defense = boss.Defense,
            GoldReward = boss.GoldReward,
            ExperienceReward = boss.ExperienceReward,
            LootTable = lootTable
        };
    }

    public DungeonClearResponse ResolveClear(string roomName, int characterLevel)
    {
        var room = _dungeonService.GetRoomByName(roomName);
        var boss = _dungeonService.GetBossForRoom(room.Name);
        var reward = _dungeonService.ResolveBossClear(room.Name, characterLevel);

        return new DungeonClearResponse
        {
            RoomName = room.Name,
            BossName = boss.Name,
            GoldReward = reward.gold,
            ExperienceReward = reward.xp,
            ItemCode = reward.itemCode,
            ItemName = reward.itemName,
            ItemRarity = reward.rarity,
            ItemDropped = !string.IsNullOrWhiteSpace(reward.itemCode)
        };
    }
}
