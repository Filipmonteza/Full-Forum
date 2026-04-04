namespace FullForum_WebUi.Models.Auth.Responses;

// DTO for login response, containing the JWT token and user information
public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; } 
    public List<string> Roles { get; set; } = new();
}