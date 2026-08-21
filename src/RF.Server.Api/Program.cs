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

builder.Services.AddSingleton(repository);
builder.Services.AddSingleton<AccountAuthService>();

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

app.Run();
