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
}
