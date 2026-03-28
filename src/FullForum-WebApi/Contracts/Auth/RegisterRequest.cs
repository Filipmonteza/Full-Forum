namespace FullForum_WebApi.Contracts.Auth;

/// <summary>
/// Request Dto for user registration
/// </summary>
public sealed record RegisterRequest(
    string Email,
    string Password,
    string Username
    );