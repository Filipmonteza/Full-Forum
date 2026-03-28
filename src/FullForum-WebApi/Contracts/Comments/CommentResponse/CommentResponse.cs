namespace FullForum_WebApi.Contracts.Comments.CommentResponse;

/// Api response representing a comment, including display name and soft delete
public sealed record CommentResponse(
    Guid Id,
    Guid ThreadId,
    Guid ApplicationUserId,
    Guid? ParentCommentId,
    string DisplayName,
    DateTime? DateCreated,
    bool IsDeleted
    );