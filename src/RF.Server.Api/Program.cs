using RF.Server.Api.Data;
using RF.Server.Api.Models;
using RF.Server.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IAccountRepository repository = string.IsNullOrWhiteSpace(connectionString)
    ? new InMemoryAccountRepository()
    : new PostgresAccountRepository(connectionString);

ICharacterRepository characterRepository = new InMemoryCharacterRepository();

builder.Services.AddSingleton(repository);
builder.Services.AddSingleton(characterRepository);
builder.Services.AddSingleton<AccountAuthService>();
builder.Services.AddSingleton<CharacterAppService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/auth/health", () => Results.Ok(new { status = "ok", message = "RF auth API is running." }));

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

app.Run();
