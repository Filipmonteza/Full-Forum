namespace FullForum_WebApi.Contracts.Comments.CommentRequest;

/// Api request for updating content of existing comment
public sealed record UpdateCommentRequest(
    string CommentContent
);

    
