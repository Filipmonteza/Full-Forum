namespace FullForum_WebUi.Models.Users.Profile.Request;

// DTO for changing password
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);