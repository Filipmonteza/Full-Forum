namespace FullForum_WebUi.Models.Users.Activity;

// DTO for user activity, including threads and comments created by the user
public sealed record UserActivityResponse(
    List<UserThreadResponse> Threads,
    List<UserCommentResponse> Comments);