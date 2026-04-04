namespace FullForum_WebUi.Models.Users.Activity;

// DTO for a thread created by the user, used in the activity overview
public sealed record UserThreadResponse(
    Guid Id,
    string ThreadTitle,
    Guid CategoryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int CommentCount);