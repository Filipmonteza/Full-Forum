namespace FullForum_WebApi.Contracts.Auth;

/// <summary>
/// Api response for current authenticated user
/// </summary>
public sealed record CurrentUserResponse(
    Guid UserId,
    string? Email,
    string? DisplayName,
    IEnumerable<string> Roles
);