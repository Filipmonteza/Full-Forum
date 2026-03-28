namespace FullForum_WebApi.Contracts.Auth;

/// <summary>
/// Request Dto for user login
/// </summary>
public sealed record LoginRequest(
    string Email,
    string Password);