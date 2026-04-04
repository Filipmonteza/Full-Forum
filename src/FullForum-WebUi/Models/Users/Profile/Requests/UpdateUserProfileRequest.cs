namespace FullForum_WebUi.Models.Users.Profile.Request;

// DTO for user profile information
public sealed record UpdateUserProfileRequest(
    string? DisplayName = null,
    string? Email = null
);