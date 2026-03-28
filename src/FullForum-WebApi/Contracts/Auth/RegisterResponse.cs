namespace FullForum_WebApi.Contracts.Auth;

/// <summary>
/// Response dto for successful user registration
/// </summary>
public sealed record RegisterResponse(
    string Message,
    Guid UserId
);