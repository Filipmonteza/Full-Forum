namespace FullForum_WebUi.Models.Auth.Responses;

/// <summary>
/// Represents the currently authenticated user's information.
/// </summary>
public sealed class MeResponse
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; } 
    public List<string> Roles { get; set; } = new();
}