namespace FullForum_Application.DTOs;

/// <summary>
/// DTO representinga a users activity, including threads & comments
/// </summary>
public sealed record UserActivityDto(
    List<UserThreadDto> Threads,
    List<UserCommentDto> Comments
);

/// <summary>
/// DTO representing a thread created by a user, used in activity overview
/// </summary>
public sealed record UserThreadDto(
    Guid Id,
    string ThreadTitle,
    Guid CategoryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int CommentCount
);

/// <summary>
/// DTO representing a comment created by a user, used in activity overview
/// </summary>
public sealed record UserCommentDto(
    Guid Id,
    string CommentContent,
    string ThreadTitle,
    Guid ThreadId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsDeleted
);
    
