namespace FullForum_WebApi.Identity;

public class JwtSettings
{
    public string SecretKey { get; set; } = "replace-with-a-strong-key-at-least-32-chars";
    public string Issuer { get; set; } = "FullForum";
    public string Audience { get; set; } = "FullForumClient";
    public int ExpiresInMinutes { get; set; } = 60;
}