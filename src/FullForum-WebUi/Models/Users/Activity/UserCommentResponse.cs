namespace FullForum_WebUi.Models.Users.Activity;

// DTO for a comment created by the user, used in the activity overview
public sealed record UserCommentResponse(
    Guid Id,
    string CommentContent,
    string ThreadTitle,
    Guid ThreadId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);