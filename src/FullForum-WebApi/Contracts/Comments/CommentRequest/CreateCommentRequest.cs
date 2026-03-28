namespace FullForum_WebApi.Contracts.Comments.CommentRequest;

/// Api request for creating new comment. ParentCommentId is null for main comments
public sealed record CreateCommentRequest(
    string CommentContent,
    Guid ThreadId,
    Guid? ParentCommentId
);

    
