namespace RF.Server.Api.Models;

public sealed record RegisterRequest(string Username, string Email, string Password);
public sealed record LoginRequest(string Username, string Password);
public sealed record AuthResponse(long AccountId, string Username, string Email, string Token);
public sealed record AccountSummary(long AccountId, string Username, string Email, bool IsBanned);
