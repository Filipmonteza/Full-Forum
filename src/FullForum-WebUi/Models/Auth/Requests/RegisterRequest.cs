namespace FullForum_WebUi.Models.Auth.Requests;

// DTOs for authentication-related operations
public sealed record RegisterRequest(
    string Email,
    string Password,
    string Username 
);