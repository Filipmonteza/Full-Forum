namespace FullForum_WebApi.Contracts.Auth;


/// <summary>
/// Api response for successful login
/// </summary>
public sealed record AuthResponse(
    string Token,
    Guid UserId,
    string? Email,
    string? DisplayName,
    IEnumerable<string> Roles
);