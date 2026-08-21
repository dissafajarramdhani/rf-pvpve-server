using Microsoft.AspNetCore.HttpOverrides;
using RF.Server.Api.Data;
using RF.Server.Api.Models;
using RF.Server.Api.Services;
using RF.Server.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IAccountRepository repository = string.IsNullOrWhiteSpace(connectionString)
    ? new InMemoryAccountRepository()
    : new PostgresAccountRepository(connectionString);

ICharacterRepository characterRepository = string.IsNullOrWhiteSpace(connectionString)
    ? new InMemoryCharacterRepository()
    : new PostgresCharacterRepository(connectionString);
IInventoryRepository inventoryRepository = new InMemoryInventoryRepository();

builder.Services.AddSingleton(repository);
builder.Services.AddSingleton(characterRepository);
builder.Services.AddSingleton(inventoryRepository);
builder.Services.AddSingleton<AntiCheatService>();
builder.Services.AddSingleton<AccountAuthService>();
builder.Services.AddSingleton<CharacterAppService>();
builder.Services.AddSingleton<CombatAppService>();
builder.Services.AddSingleton<InventoryAppService>();
builder.Services.AddSingleton<DungeonService>();
builder.Services.AddSingleton<DungeonAppService>();
builder.Services.AddSingleton<PvpArenaService>();
builder.Services.AddSingleton<ArenaAppService>();
builder.Services.AddSingleton<GuildAppService>();
builder.Services.AddSingleton(new CombatService());

var app = builder.Build();

app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var elapsed = DateTime.UtcNow - start;
    if (context.Response.StatusCode >= 400)
    {
        Console.WriteLine($"[{DateTime.UtcNow:O}] HTTP {context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} ({elapsed.TotalMilliseconds} ms)");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var requireHttps = builder.Configuration.GetValue<bool>("AppSettings:RequireHttps", false);
if (requireHttps)
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/health");
app.MapGet("/api/auth/health", () => Results.Ok(new { status = "ok", message = "RF auth API is running." }));
app.MapGet("/api/health", () =>
{
    var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
    return Results.Ok(new
    {
        status = "ok",
        environment = app.Environment.EnvironmentName,
        uptimeSeconds = Math.Round(uptime.TotalSeconds, 2),
        timestampUtc = DateTime.UtcNow,
        databaseConfigured = !string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection"))
    });
});
app.MapGet("/api/security/rules", (AntiCheatService antiCheatService) => Results.Ok(antiCheatService.GetSummary()));

app.MapPost("/api/auth/register", async (RegisterRequest request, AccountAuthService authService) =>
{
    try
    {
        var account = await authService.RegisterAsync(request.Username, request.Email, request.Password);
        var token = authService.IssueToken(account);

        return Results.Ok(new AuthResponse(account.Id, account.Username, account.Email, token));
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/auth/login", async (LoginRequest request, AccountAuthService authService) =>
{
    try
    {
        var account = await authService.LoginAsync(request.Username, request.Password);
        if (account is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new AuthResponse(account.Id, account.Username, account.Email, authService.IssueToken(account)));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/characters/{accountId:long}", async (long accountId, CharacterAppService characterService) =>
{
    var characters = await characterService.GetCharactersAsync(accountId);
    var result = characters.Select(c => new CharacterResponse(
        c.Id,
        c.AccountId,
        c.Name,
        c.Class.Name,
        c.Level,
        c.Position.X,
        c.Position.Y,
        c.Position.Z,
        c.Health,
        c.Mana));

    return Results.Ok(result);
});

app.MapPost("/api/characters/create", async (CreateCharacterRequest request, CharacterAppService characterService) =>
{
    try
    {
        var character = await characterService.CreateCharacterAsync(request.AccountId, request.ClassCode, request.Name);
        return Results.Ok(new CharacterResponse(
            character.Id,
            character.AccountId,
            character.Name,
            character.Class.Name,
            character.Level,
            character.Position.X,
            character.Position.Y,
            character.Position.Z,
            character.Health,
            character.Mana));
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/world/move", async (MoveCharacterRequest request, CharacterAppService characterService) =>
{
    try
    {
        var moved = await characterService.MoveCharacterAsync(request.AccountId, request.CharacterId, request.X, request.Y, request.Z);
        if (moved is null)
        {
            return Results.NotFound(new { error = "Character not found or not belongs to account." });
        }

        return Results.Ok(new WorldPositionResponse(moved.Id, moved.Position.X, moved.Position.Y, moved.Position.Z));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/combat/attack", async (AttackRequest request, CombatAppService combatService) =>
{
    try
    {
        var result = await combatService.AttackAsync(
            request.AccountId,
            request.CharacterId,
            request.MonsterName,
            request.MonsterMaxHealth,
            request.MonsterAttack,
            request.MonsterDefense,
            request.BaseDamage);

        if (result is null)
        {
            return Results.NotFound(new { error = "Character not found for account." });
        }

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/dungeon/rooms", (DungeonAppService dungeonService) =>
{
    var rooms = dungeonService.GetRooms();
    return Results.Ok(rooms);
});

app.MapGet("/api/dungeon/encounter/{roomName}", (string roomName, DungeonAppService dungeonService) =>
{
    try
    {
        var encounter = dungeonService.GetEncounter(roomName);
        return Results.Ok(encounter);
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapGet("/api/dungeon/boss/{roomName}", (string roomName, DungeonAppService dungeonService) =>
{
    try
    {
        var boss = dungeonService.GetBoss(roomName);
        return Results.Ok(boss);
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/api/dungeon/clear", (DungeonClearRequest request, DungeonAppService dungeonService) =>
{
    try
    {
        var reward = dungeonService.ResolveClear(request.RoomName, request.CharacterLevel);
        return Results.Ok(reward);
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapGet("/api/arena/rules", (ArenaAppService arenaService) => Results.Ok(arenaService.GetRules()));

app.MapPost("/api/arena/duel", async (ArenaMatchRequest request, ArenaAppService arenaService) =>
{
    try
    {
        var result = await arenaService.ResolveBattleAsync(
            request.AttackerCharacterId,
            request.DefenderCharacterId,
            request.AttackerBaseDamage,
            request.DefenderBaseDamage);

        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/guilds/create", (CreateGuildRequest request, GuildAppService guildService) =>
{
    try
    {
        var guild = guildService.CreateGuild(request.AccountId, request.CharacterId, request.Name, request.Tag);
        return Results.Ok(guild);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/guilds/join", (JoinGuildRequest request, GuildAppService guildService) =>
{
    try
    {
        var guild = guildService.JoinGuild(request.AccountId, request.CharacterId, request.GuildId);
        if (guild is null)
        {
            return Results.NotFound(new { error = "Guild or character not found." });
        }

        return Results.Ok(guild);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/guilds/{guildId:long}/members", (long guildId, GuildAppService guildService) =>
{
    var members = guildService.GetMembers(guildId);
    return Results.Ok(members);
});

app.MapGet("/api/guilds/{guildId:long}", (long guildId, GuildAppService guildService) =>
{
    var guild = guildService.GetGuild(guildId);
    if (guild is null)
    {
        return Results.NotFound(new { error = "Guild not found." });
    }

    return Results.Ok(guild);
});

app.MapGet("/api/inventory/{accountId:long}/{characterId:long}", async (long accountId, long characterId, InventoryAppService inventoryService) =>
{
    var items = await inventoryService.GetInventoryAsync(accountId, characterId);
    return Results.Ok(items);
});

app.MapPost("/api/inventory/add-item", async (AddInventoryItemRequest request, InventoryAppService inventoryService) =>
{
    try
    {
        var item = await inventoryService.AddItemAsync(request.AccountId, request.CharacterId, request.ItemCode);
        return Results.Ok(item);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/inventory/equip", async (EquipItemRequest request, InventoryAppService inventoryService) =>
{
    try
    {
        var item = await inventoryService.EquipItemAsync(request.AccountId, request.CharacterId, request.ItemId, request.SlotName);
        if (item is null)
        {
            return Results.NotFound(new { error = "Inventory item not found for character." });
        }

        return Results.Ok(item);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
