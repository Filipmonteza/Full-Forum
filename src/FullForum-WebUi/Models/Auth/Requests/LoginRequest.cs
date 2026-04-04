namespace FullForum_WebUi.Models.Auth.Requests;

// DTO for login request
public sealed record LoginRequest(
    string Email,
    string Password
);