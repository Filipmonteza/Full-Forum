namespace FullForum_Application.DTOs;

/// <summary>
/// DTO representing a comment, used in comment endpoints and thread responses
/// </summary>
public sealed record CommentDto(Guid Id,
    string Content,
    Guid ThreadId,
    Guid UserId,
    Guid? ParentCommentId,
    DateTime CreatedAt
    );
    
